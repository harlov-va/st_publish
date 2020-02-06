using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.Finance;
using arkAS.BLL.HR;
using arkAS.BLL.CRM;
using arkAS.Models;
using System.Collections;
using System.Linq.Expressions;

namespace arkAS.Controllers
{
    public class FinancesController : Controller
    {
        // GET: Finances
        public ActionResult Index()
        {
            var mng = new FinanceManager();
            var mng_hr = new HRManager();
            var mng_crm = new CRMManager();
            ViewBag.contragentName = mng.GetFinContragents();
            ViewBag.typeName = mng.GetFinTypes();
            ViewBag.projectName = mng.GetProjects();
            ViewBag.statusName = mng.GetFinStatuses();
            ViewBag.hrName = mng_hr.GetHumans();
            ViewBag.crmName = mng_crm.GetClients();
            return View();            
        }

        public ActionResult Finances_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new FinanceManager();
            var items = mng.GetFinFinances().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {                
                var fromID = parameters.filter.ContainsKey("fromName") ? RDL.Convert.StrToInt(parameters.filter["fromName"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (fromID == 0 || x.fromID == fromID)
                );

                var toID = parameters.filter.ContainsKey("toName") ? RDL.Convert.StrToInt(parameters.filter["toName"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (toID == 0 || x.toID == toID)
                );

                var projectID = parameters.filter.ContainsKey("projectName") ? RDL.Convert.StrToInt(parameters.filter["projectName"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (projectID == 0 || x.projectID == projectID)
                );

                var typeID = parameters.filter.ContainsKey("typeName") ? RDL.Convert.StrToInt(parameters.filter["typeName"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (typeID == 0 || x.typeID == typeID)
                );
               
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusIDs = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                items = items.Where(x =>                    
                    (statusIDs.Count == 0 || statusIDs.Contains(x.statusID)) 
                );
               
                DateTime createdMin = DateTime.MinValue, createdMax = DateTime.MaxValue;
                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), DateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), DateTime.MaxValue);
                    }
                }
                items = items.Where(x =>                  
                   (createdMin <= x.created && x.created <= createdMax)
               );
                                
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            IOrderedQueryable<fin_finances> orderedItems = items.OrderByDescending(x => x.created);

            switch (sort1)
            {
                case "fromName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.fin_contragents.name);
                    else orderedItems = items.OrderByDescending(x => x.fin_contragents.name);
                    break;
                case "toName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.fin_contragents1.name);
                    else orderedItems = items.OrderByDescending(x => x.fin_contragents1.name);
                    break;
                case "sum":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.sum);
                    else orderedItems = items.OrderByDescending(x => x.sum);
                    break;
                case "desc":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.desc);
                    else orderedItems = items.OrderByDescending(x => x.desc);
                    break;
                case "projectName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.tt_projects.name);
                    else orderedItems = items.OrderByDescending(x => x.tt_projects.name);
                    break;
                case "typeName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.fin_types.name);
                    else orderedItems = items.OrderByDescending(x => x.fin_types.name);
                    break;
                case "statusName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.fin_statuses.name);
                    else orderedItems = items.OrderByDescending(x => x.fin_statuses.name);
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
                    case "fromName":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.fin_contragents.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.fin_contragents.name);
                        break;
                    case "toName":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.fin_contragents1.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.fin_contragents1.name);
                        break;
                    case "sum":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.sum);
                        else orderedItems = orderedItems.ThenByDescending(x => x.sum);
                        break;
                    case "desc":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.desc);
                        else orderedItems = orderedItems.ThenByDescending(x => x.desc);
                        break;
                    case "projectName":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.tt_projects.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.tt_projects.name);
                        break;
                    case "typeName":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.fin_types.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.fin_types.name);
                        break;
                    case "statusName":
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.fin_statuses.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.fin_statuses.name);
                        break;
                    default:
                        if (direction1 == "up") orderedItems = orderedItems.ThenBy(x => x.created);
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
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                    x.fromID,
                    fromName=x.fin_contragents !=null ? x.fin_contragents.name :"",
                    x.toID,
                    toName = x.fin_contragents1 != null ? x.fin_contragents1.name : "",
                    sum=x.sum ?? 0,
                    desc=x.desc ?? "",
                    x.typeID,
                    typeName = x.fin_types != null ? x.fin_types.name : "",
                    x.projectID,
                    projectName = x.tt_projects != null ? x.tt_projects.name : "",
                    x.statusID,
                    statusName = x.fin_statuses != null ? x.fin_statuses.name : ""
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateFinance(string desc, int fromID, int toID, string sum, int typeID, int projectID, int statusID)
        {
            var mng = new FinanceManager();

            int? fromID_ = null;
            if (fromID != 0) fromID_ = fromID;

            int? toID_ = null;
            if (toID != 0) toID_ = toID;
           
            int? typeID_ = null;
            if (typeID != 0) typeID_ = typeID;

            int? projectID_ = null;
            if (projectID != 0) projectID_ = projectID;

             int? statusID_ = null;
            if (statusID != 0) statusID_ = statusID;

            var item = new fin_finances
            {
                id = 0,
                created = DateTime.Now,
                fromID=fromID_,
                toID=toID_,
                desc=desc,
                channelID=null,
                sum=RDL.Convert.StrToDecimal(sum,0),
                projectID=projectID_,
                typeID=typeID_,
                statusID=statusID_              
            };
            mng.SaveFinFinance(item);

            return Json(new
            {
                result = item.id > 0,
                financeID = item.id
            });
        }


        public ActionResult CreateContragent(string name, int humanID, int clientID)
        {
            var mng = new FinanceManager();

            int? humanID_ = null;
            if (humanID != 0) humanID_ = humanID;

            int? clientID_ = null;
            if (clientID != 0) clientID_ = clientID;           

            var item = new fin_contragents
            {
                id = 0,
                humanID=humanID_,
                clientID=clientID_,
                name=name
            };
            mng.SaveFinContragents(item);

            return Json(new
            {
                result = item.id > 0,
                contragentID = item.id
            });
        }

        public ActionResult FinancesInline(int pk, string value, string name)
        {
            var mng = new FinanceManager();
            mng.EditFinanceField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Finances_remove(int id)
        {
            var res = false;
            var mng = new FinanceManager();
            var item = mng.GetFinFinance(id);
            var msg = "";
            if (item != null)
            {
                mng.DeleteFinFinance(id);
                msg = "Транзакция удалена!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        public ActionResult Finances_getComments(int itemID)
        {
            var mng = new CommentManager();
            var res = true;
            return Json(new
            {
                result = res,
                items = mng.GetComments("finances", itemID.ToString()).Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    x.username,
                    x.text
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Finances_addComment(string itemID, string text)
        {
            var mng = new CommentManager();
            var item = mng.AddComment("finances", itemID, text);

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

        //GET: Contractors
        public ActionResult Contragents()
        {
            var crmManager = new CRMManager();
            ViewBag.Clients = crmManager.GetClients();

            var hrManager = new HRManager();
            ViewBag.Humans = hrManager.GetHumans();

            return View();
        }

        public ActionResult Contragents_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new FinanceManager();
            var items = mng.GetFinContragents().AsQueryable();

            string nameFilter;
            if (GetStringFilter(parameters.filter, "name", out nameFilter))
                items = items.Where(i => i.name.Contains(nameFilter));

            int clientId;
            if (GetIntFilter(parameters.filter, "clientName", out clientId))
                items = items.Where(i => i.clientID == clientId);

            int humanId;
            if (GetIntFilter(parameters.filter, "humanName", out humanId))
                items = items.Where(i => i.humanID == humanId);

            var orderedItems = items.OrderByDescending(x => x.name);
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (sorts.Length > 0 && directions.Length > 0)
            {
                var sortExpression = GetSortExpression(sorts[0]);
                if (sortExpression != null)
                {
                    orderedItems = (directions[0] == "up")
                        ? orderedItems.OrderBy(sortExpression)
                        : orderedItems.OrderByDescending(sortExpression);
                }
            }

            var total = orderedItems.Count();
            var res2 = orderedItems.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    x.name,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    x.humanID,
                    humanName = x.hr_humans != null ? x.hr_humans.fio : ""
                }),
                total
            }, JsonRequestBehavior.AllowGet);
        }

        private Expression<Func<fin_contragents, string>> GetSortExpression(string sort)
        {
            switch (sort)
            {
                case "name":
                    return i => i.name;
                case "clientName":
                    return i => i.crm_clients != null ? i.crm_clients.fio : "";
                case "humanName":
                    return i => i.hr_humans != null ? i.hr_humans.fio : "";
                default:
                    return null;
            }
        }

        private bool GetStringFilter(Dictionary<string, object> filter, string filterName, out string filterValue)
        {
            filterValue = "";
            object filterObject;
            if (filter.TryGetValue(filterName, out filterObject))
            {
                filterValue = filterObject.ToString();
                if (!String.IsNullOrWhiteSpace(filterValue))
                {
                    return true;
                }
            }
            return false;
        }

        private bool GetIntFilter(Dictionary<string, object> filter, string filterName, out int filterValue)
        {
            filterValue = 0;
            object filterObject;
            if (filter.TryGetValue(filterName, out filterObject))
            {
                filterValue = RDL.Convert.StrToInt(filterObject.ToString(), 0);
                if (filterValue > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult ContragentInline(int pk, string value, string name)
        {
            var mng = new FinanceManager();
            mng.EditFinContragentField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Contragents_remove(int id)
        {
            var res = false;
            var mng = new FinanceManager();
            var item = mng.GetFinContragent(id);
            var msg = "";
            if (item != null)
            {
                res = mng.DeleteFinContragent(id);
                msg = res ? "Контрагент удален!" : "Чтобы удалить данного контрагента, вначале необходимо удалить все его транзакции";                
            }

            return Json(new
            {
                result = res,
                msg
            });
        }
    }
}