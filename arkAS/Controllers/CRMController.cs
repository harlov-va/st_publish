using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.Models;
using System.Web.Security;
using System.Collections;
using System.Data;
using Dapper;
using Newtonsoft.Json;


namespace arkAS.Controllers
{
    public class CRMController : Controller
    {
        // GET: CRM             
        public ActionResult Index()
        {           
            //var res = Membership.GetUser(name).GetPassword();
            var mng = new CRMManager();
            /*
             * Выборка через EF
             * ViewBag.statusClientName = mng.GetClientStatuses();
             * ViewBag.sourceClientName = mng.GetClientSources();
             */
            ViewBag.statusClientName = mng.GetSQLData<crm_clientStatuses>("GetCRMClientStatuses");
            ViewBag.sourceClientName = mng.GetSQLData<crm_sources>("GetCRMSources");

            return View();
        }
        [Authorize]
        public ActionResult Clients()
        {
            var mng = new CRMManager();
            ViewBag.statusClientName = mng.GetSQLData<crm_clientStatuses>("GetCRMClientStatuses");
            ViewBag.sourceClientName = mng.GetSQLData<crm_sources>("GetCRMSources");
            return View();
        }
        public ActionResult Clients_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();
            var text = "";
            var sourceID = 0;
            var needActive = -1;
            List<int?> statusID = new List<int?>();

            DateTime? nextContactMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime? nextContactMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                needActive = parameters.filter.ContainsKey("needActive") ? RDL.Convert.StrToInt(parameters.filter["needActive"].ToString(), -1) : -1;
                sourceID = parameters.filter.ContainsKey("sourceName") ? RDL.Convert.StrToInt(parameters.filter["sourceName"].ToString(), 0) : 0;
                statusID = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusID = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                if (parameters.filter.ContainsKey("nextContact") && parameters.filter["nextContact"] != null)
                {
                    var dates = parameters.filter["nextContact"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        nextContactMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        nextContactMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                    }
                }
            }
            
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("text", text);
            p.Add("sourceID", (sourceID == 0) ? "" : Convert.ToString(sourceID));
            p.Add("statusID", String.Join(",", statusID));
            p.Add("needActive", (needActive == -1) ? "" : Convert.ToString(needActive));
            p.Add("nextContactMin", nextContactMin);
            p.Add("nextContactMax", nextContactMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetCRMClientsTable", p, CommandType.StoredProcedure);
         
            var total = p.Get<int>("total");
            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            var res1 = json.Replace("\"needActive\":null", "\"needActive\":\"Не проработано\"");
            var res2 = res1.Replace("\"needActive\":true", "\"needActive\":\"Да\"");
            var res3 = res2.Replace("\"needActive\":false", "\"needActive\":\"Нет\"");
            var res4 = res3.Replace("null", "\"\"");
            return Content(res4, "application/json");
        }
        public ActionResult OLD_Clients_getItems()
        {           
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();
            //var items = mng.GetClients().AsQueryable();
            var items = mng.GetSQLClients("GetCRMClients").AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
               
                var sourceID = parameters.filter.ContainsKey("sourceName") ? RDL.Convert.StrToInt(parameters.filter["sourceName"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (sourceID == 0 || x.sourceID == sourceID)
                );
                
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusIDs = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                items = items.Where(x =>
                    (statusIDs.Count == 0 || statusIDs.Contains(x.statusID))
                );


                var needActive = parameters.filter.ContainsKey("needActive") ? RDL.Convert.StrToInt(parameters.filter["needActive"].ToString(), -1) : -1;
                items = items.Where(x =>
                    //фильтр в этом месте не срабатывает для needActive=NULL
                    (needActive == -1 || x.needActive == (needActive==1 ? true : false))
                );

                DateTime nextContactMin = DateTime.MinValue, nextContactMax = DateTime.MaxValue;
                if (parameters.filter.ContainsKey("nextContact") && parameters.filter["nextContact"] != null)
                {
                    var dates = parameters.filter["nextContact"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        nextContactMin = RDL.Convert.StrToDateTime(dates[0].Trim(), DateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        nextContactMax = RDL.Convert.StrToDateTime(dates[1].Trim(), DateTime.MaxValue).AddDays(1);
                    }
                    items = items.Where(x =>
                        (nextContactMin <= x.nextContact && x.nextContact <= nextContactMax));
                }
               
                if (text != "")
                {
                    items = items.ToList().Where(x =>
                        x.fio != null && x.fio.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.city != null && x.city.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.note != null && x.note.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.username != null && x.username.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.addedBy != null && x.addedBy.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 
                        ).AsQueryable();
                }
            }
            
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            IOrderedQueryable<crm_clients> orderedItems = items.OrderByDescending(p=>p.created);

            switch (sort1)
            {
                case "fio":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.fio);
                    else orderedItems = items.OrderByDescending(x => x.fio);
                    break;
                case "city":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.city);
                    else orderedItems = items.OrderByDescending(x => x.city);
                    break;
                case "note":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.note);
                    else orderedItems = items.OrderByDescending(x => x.note);
                    break;
                case "addedBy":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.addedBy);
                    else orderedItems = items.OrderByDescending(x => x.addedBy);
                    break;
                case "statusName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.crm_clientStatuses.name);
                    else orderedItems = items.OrderByDescending(x => x.crm_clientStatuses.name);
                    break;
                case "sourceName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.crm_sources.name);
                    else orderedItems = items.OrderByDescending(x => x.crm_sources.name);
                    break;
                case "subchannel":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.subchannel);
                    else orderedItems = items.OrderByDescending(x => x.subchannel);
                    break;
                case "username":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.username);
                    else orderedItems = items.OrderByDescending(x => x.username);
                    break;
                case "needActive":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.needActive);
                    else orderedItems = items.OrderByDescending(x => x.needActive);
                    break;
                case "nextContact":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.nextContact);
                    else orderedItems = items.OrderByDescending(x => x.nextContact);
                    break;

                default:
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.created);
                    else orderedItems = items.OrderByDescending(x => x.created);
                    break;
            }

            if (sort2 != "")
            {
                switch (sort2)
                {
                    case "fio":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.fio);
                        else orderedItems = orderedItems.ThenByDescending(x => x.fio);
                        break;
                    case "city":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.city);
                        else orderedItems = orderedItems.ThenByDescending(x => x.city);
                        break;
                    case "note":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.note);
                        else orderedItems = orderedItems.ThenByDescending(x => x.note);
                        break;
                    case "addedBy":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.addedBy);
                        else orderedItems = orderedItems.ThenByDescending(x => x.addedBy);
                        break;
                    case "statusName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.crm_clientStatuses.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.crm_clientStatuses.name);
                        break;
                    case "sourceName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.crm_sources.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.crm_sources.name);
                        break;
                    case "subchannel":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.subchannel);
                        else orderedItems = orderedItems.ThenByDescending(x => x.subchannel);
                        break;
                    case "username":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.username);
                        else orderedItems = orderedItems.ThenByDescending(x => x.username);
                        break;
                    case "needActive":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.needActive);
                        else orderedItems = orderedItems.ThenByDescending(x => x.needActive);
                        break;
                    case "nextContact":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.nextContact);
                        else orderedItems = orderedItems.ThenByDescending(x => x.nextContact);
                        break;

                    default:
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.created);
                        else orderedItems = orderedItems.ThenByDescending(x => x.created);
                        break;
                }
            }

            var total = orderedItems.Count();
            var res2 = orderedItems.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    fio = x.fio ?? "",
                    city = x.city ?? "",
                    note = x.note ?? "",
                    x.sourceID,
                    sourceName = x.crm_sources != null ? x.crm_sources.name : "",
                    x.statusID,
                    statusName = x.crm_clientStatuses != null ? x.crm_clientStatuses.name : "",
                    addedBy = x.addedBy ?? "",

                    created =  x.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                    nextContact = x.nextContact != null ? x.nextContact.GetValueOrDefault().ToString("dd.MM.yyyy") : "",
                    subchannel = x.subchannel ?? "",
                    username = x.username ?? "",
                    needActive=x.needActive.HasValue && x.needActive==true ? "Да" : "Нет"
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateClient(string fio,  string note, int sourceID, int statusID)
        {
            var mng = new CRMManager();

            int? sourceID_ = null;
            if (sourceID != 0) sourceID_ = sourceID;

            int? statusID_ = null;
            if (statusID != 0) statusID_=statusID;
            var user = HttpContext.User.Identity.Name;
         
            var item = new crm_clients { 
                id = 0, fio = fio, city=null, note = note, sourceID =sourceID_, statusID = statusID_, addedBy = user,
                created = DateTime.Now, nextContact = DateTime.Now,  subchannel = null,   username = null, needActive = null
            };

            mng.SaveClient(item);

            return Json(new
            {
                result = item.id > 0,
                clientID = item.id
            });
        }

        public ActionResult ClientsInline(int pk, string value, string name)
        {
            var mng = new CRMManager();
            mng.EditClientField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Clients_remove(int id)
        {            
            var res = false;           
            var mng = new CRMManager();
            var item = mng.GetClient(id);
            var msg = "";
            if (item != null)
            {
                mng.DeleteClient(id);
                msg = "Клиент удален!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }
       
        public ActionResult Clients_getComments(int itemID)
        {
            var mng = new CommentManager();
            var res = true;
            return Json(new
            {
                result = res,
                items = mng.GetComments("clients", itemID.ToString()).Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    x.username,
                    x.text
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Clients_addComment(string itemID, string text)
        {
            var mng = new CommentManager();
            var item = mng.AddComment("clients", itemID, text);

            var res = true;
            return Json(new
            {
                result = res,
                item = new
                {
                    item.id,
                    created = item.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    item.username,
                    item.text
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientStatuses() 
        {
            return View();
        }

        public ActionResult ClientStatuses_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();
            var items = mng.GetClientStatuses();

            var res = items.Select(item => new crm_clientStatuses
            {
                id = item.id,
                name = item.name,
                code = item.code,
                color = item.color,
                state = item.state
            }).AsQueryable();

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;

                case "code":
                    if (direction1 == "up") res = res.OrderBy(x => x.code);
                    else res = res.OrderByDescending(x => x.code);
                    break;
                case "color":
                    if (direction1 == "up") res = res.OrderBy(x => x.color);
                    else res = res.OrderByDescending(x => x.color);
                    break;
                case "state":
                    if (direction1 == "up") res = res.OrderBy(x => x.state);
                    else res = res.OrderByDescending(x => x.state);
                    break;
                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }

            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res2.Select(x => new 
                {
                    x.id,
                    x.name,
                    x.code,
                    x.state,
                    x.color,
                    
                }),
                total = items.Count
            
            },JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientStatuses_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMManager();
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newClientStatuses = new crm_clientStatuses
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = (AjaxModel.GetValueFromSaveField("name", fields)),
                    code = (AjaxModel.GetValueFromSaveField("code", fields)),
                    color = (AjaxModel.GetValueFromSaveField("color", fields)),
                    state = (AjaxModel.GetValueFromSaveField("state", fields))
                };

                mng.SaveClientStatus(newClientStatuses);
                return Json(new
                {
                    result = true,
                    id = newClientStatuses.id,
                    mng = "Операция прошла успешно"
                },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    id = 0,
                    mng = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ClientStatuses_remove(string id) 
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMManager();

            try
            {
                if (mng.GetClientStatus(int.Parse(id)).crm_clients.Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        mng = "Статус связан с клиентом, сначало требуется снять данный статус со всех клиентов"

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeleteClientStatus(int.Parse(id));
                    return Json(new
                    {
                        result = true,
                        mng = "Оперция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new 
                {
                    result = false,
                    mng = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OrderStatuses()
        {
            return View();

        }

        public ActionResult OrderStatuses_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();
            var items = mng.GetOrderStatuses();

            var res = items.Select(item => new crm_orderStatuses
            {
                id = item.id,
                name = item.name,
                code = item.code,
                color = item.color
            }).AsQueryable();

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;

                case "code":
                    if (direction1 == "up") res = res.OrderBy(x => x.code);
                    else res = res.OrderByDescending(x => x.code);
                    break;
                case "color":
                    if (direction1 == "up") res = res.OrderBy(x => x.color);
                    else res = res.OrderByDescending(x => x.color);
                    break;
                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }

            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res2.Select(x => new 
                {
                    x.id,
                    x.name,
                    x.code,
                    x.color
                }),
                total = items.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderStatuses_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMRepository();

            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x=>x as Dictionary<string, object>).ToList();
                var newOrderStatuses = new crm_orderStatuses
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = (AjaxModel.GetValueFromSaveField("name", fields)),
                    code = (AjaxModel.GetValueFromSaveField("code", fields)),
                    color = (AjaxModel.GetValueFromSaveField("color", fields))
                };
                mng.SaveOrderStatus(newOrderStatuses);
                return Json(new 
                {
                    result = true,
                    id = newOrderStatuses.id,
                    mng = "Операция прошла успешно"
                });
                
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new 
                {
                    result = false,
                    id = 0,
                    mng = "Ошибка",
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult OrderStatuses_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMRepository();
            try
            {
                if (mng.GetOrderStatuses().Single(x => x.id == int.Parse(id)).crm_orders.Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        mng = "Статус привязан к ордерам!",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeleteOrderStatus(int.Parse(id));
                    return Json(new
                    {
                        result = true,
                        mng = "Операция прошла успешно"
                    });
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new 
                {
                    result = false,
                    mng = "Ошибка"
                });
            }
        }

        public ActionResult Sources()
        {
            return View();
        }

        public ActionResult Sources_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();
            var items = mng.GetClientSources();

            var res = items.Select(item => new crm_sources
            {
                id = item.id,
                name = item.name,
                code = item.code,
                desc = item.desc
            }).AsQueryable();

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;

                case "code":
                    if (direction1 == "up") res = res.OrderBy(x => x.code);
                    else res = res.OrderByDescending(x => x.code);
                    break;
                case "desc":
                    if (direction1 == "up") res = res.OrderBy(x => x.desc);
                    else res = res.OrderByDescending(x => x.desc);
                    break;
                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }

            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new 
            {
                items =res2.Select(x=> new
                {
                    x.id,
                    x.name,
                    x.code,
                    x.desc,
                }),
                total = items.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sources_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMManager();

            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newSources = new crm_sources
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = (AjaxModel.GetValueFromSaveField("name", fields)),
                    code = (AjaxModel.GetValueFromSaveField("code", fields)),
                    desc = (AjaxModel.GetValueFromSaveField("desc", fields))

                };
                mng.SaveClientSources(newSources);
                return Json(new 
                {
                    result = true,
                    id = newSources.id,
                    mng = "Операция прошла успешно"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new 
                {
                    result = false,
                    id = 0,
                    mng = "Ошибка"
                });
            }
        }

        public ActionResult Sources_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMManager();

            try
            {
                if (mng.GetClientSource(int.Parse(id)).crm_clients.Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        mng = "Не удается удалить",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeleteClientSources(int.Parse(id));
                    return Json(new 
                    {
                        result = true,
                        mng = "Операция прошла успешно"
                    });
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new 
                {
                    result = false,
                    mng = "Ошибка"
                });
            }
        }
       
        [Authorize(Roles = "admin")]
        public ActionResult Reclamations()
        {
            var mng = new CRMManager();
            ViewBag.ReclamationStatuses = mng.GetReclamationStatuses();
            ViewBag.Projects = mng.GetProjects();
            ViewBag.Projects.Insert(0, new tt_projects { id = 0, name = "Не выбран" });

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Reclamations_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            var haveWOW = 0;
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            int y = 0, m = 0, d = 0;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                p.Add("statusIDs", String.Join(",", statusIDs));

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var date = parameters.filter["created"].ToString();
                    if (date.Length == 10)
                    {
                        y = Convert.ToInt32(date.Substring(6));
                        m = Convert.ToInt32(date.Substring(3, 2));
                        d = Convert.ToInt32(date.Substring(0, 2));
                        p.Add("createdDate", new DateTime(y, m, d));
                    }
                }
                
                haveWOW = parameters.filter.ContainsKey("haveWOWname") ? RDL.Convert.StrToInt(parameters.filter["haveWOWname"].ToString(), 0) : -1;
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            p.Add("haveWOWname", haveWOW - 1);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("GetCRMReclamationsTable", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            foreach (var item in items)
            {
                if (item.name == null) item.name = string.Empty;
                if (item.customerText == null) item.customerText = string.Empty;
                if (item.whatToDo == null) item.whatToDo = string.Empty;
                if (item.statusName == null) item.statusName = string.Empty;
                if (item.created == null) item.created = string.Empty;
                if (item.addedBy == null) item.addedBy = string.Empty;
                if (item.reportDate == null) item.reportDate = string.Empty;
                if (item.projectName == null) item.projectName = string.Empty;
                if (item.haveWOW == 1) item.haveWOWname = "Да"; else item.haveWOWname = "Нет";
            }

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        [Authorize(Roles = "admin")]
        public ActionResult ReclamationsInline(int pk, string value, string name)
        {
            var mng = new CRMManager();
            mng.EditReclamationField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        [Authorize(Roles = "admin")]
        public ActionResult CreateReclamation(string name, int statusID, string customerText, int haveWOW, int projectID, string reportDate)
        {
            var mng = new CRMManager();
            int y = Convert.ToInt32(reportDate.Substring(6));
            int m = Convert.ToInt32(reportDate.Substring(3, 2));
            int d = Convert.ToInt32(reportDate.Substring(0, 2));
            var item = new recl_items
            {
                id = 0,
                addedBy = User.Identity.Name,
                name = name,
                statusID = statusID,
                created = DateTime.Now.Date,
                haveWOW = haveWOW == 1 ? true : false,
                customerText = customerText,
                projectID = projectID,
                reportDate = new DateTime(y, m, d)
            };
            mng.SaveReclamation(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}