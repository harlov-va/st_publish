using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.Models;
using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Data;
using System.Web.Script.Serialization;
using arkAS.BLL.Calc;
using Newtonsoft.Json;
using System.Net;
using Glimpse.Core.Extensions;
using arkAS.BLL.Calendar;

namespace arkAS.Controllers
{
    public class DemoController : Controller
    {
        static string PathChapter = @"/uploads/Images/users_avatar/";
        static string FileStandart = @"avatar.png";
        static string GuiDefault = @"83e9d28d-e644-460e-9fc6-44999afb80c2";//hecrus@mail.ru
        public DemoController()
        {
            string PathCurrent = AppDomain.CurrentDomain.BaseDirectory + PathChapter;
            if (!Directory.Exists(PathCurrent))
            {
                Directory.CreateDirectory(PathCurrent);
            }
        }

        public ActionResult Kanban()
        {
            var mng = new SettingsManager();
            SettingsCategoriesViewMdels model = new SettingsCategoriesViewMdels();
            model.settingCategories  = mng.db.GetSettingCategories();            
            model.settings = new List<as_settings>();
            foreach(var item in model.settingCategories)
            {
                foreach(var it in item.as_settings)
                {
                    model.settings.Add(it);
                }
            }
            return View(model);
        }

        public ActionResult SaveSetting(string code, string value,
            int category, string value2, string name)
        {
            var mng = new SettingsManager();

            as_settings setting = new as_settings
            {
                code = code,
                value=value,
                categoryID=category,
                value2=value2,
                name=name
            };

            var result = mng.SaveSetting(setting);

            return Json(new
            {
                result
            });
        }

        public ActionResult EditSetting(int id, string fieldValue, string fieldName)
        {
            var mng = new SettingsManager();
            bool result = mng.EditSettingField(id, fieldName, fieldValue);
            return Json(new
            {
                result
            });
        }

        public ActionResult Categories()
        {
            var mng = new CategoryManager();
            ViewBag.Categories = mng.GetCategories();
            return View();

        }

        public ActionResult Statuses()
        {
            var mng = new StatusesManager();
            ViewBag.Statuses = mng.db.GetStatuses();
            return View();

        }



        public ActionResult SettingCategories()
        {
            var mng = new SettingsManager();
            ViewBag.SettingCategories = mng.db.GetSettingCategories();
            return View();

        }
        // LESS
        public ActionResult Less()
        {
            return View();
        }

        //Socials Network
        public ActionResult SocialNetworks()
        {
            return View();
        }

        //Post via vk api
        public ActionResult VkPost()
        {
            return View();
        }

        public ActionResult SqlCrud()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/Account/Login/?demoType=SqlCrud");

            }

        }

        

        public ActionResult GetSqlCommand()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var code = parameters["code"].ToStringOrDefault();
            var pageSize = RDL.Convert.StrToInt(parameters["pageSize"].ToString(), 1);
            var page = RDL.Convert.StrToInt(parameters["page"].ToString(), 1);
            var mng = new SqlCrudManager();
            var sqlID = mng.GetSqlCruds().Where(x => x.code == code).Select(x => x.id).FirstOrDefault();
            var rol = mng.GetSqlCruds().Where(x => x.as_sqlRole.Any(y => y.sqlID==sqlID)).ToList();
            if (rol.Count == 0)
            {
                return Json(new
                {
                    S="sql"
                }
            );
            }
            var ww = rol[0].as_sqlRole.Select(x=>x.role);
            
            var edr = Roles.GetRolesForUser(Membership.GetUser().UserName);
            var fd = ww.Intersect(edr).ToList();
            if (fd.Count!=0)
            {
                var command = mng.GetSqlCruds().Where(x => x.code == code).Select(x => x.sql).FirstOrDefault();


               
                var dt = mng.GetSqlCrud(command, null);
                var res = dt.AsEnumerable().Skip(pageSize*(page - 1)).Take(pageSize).ToList();
                return Json(new
                {

                    Table = new
                    {
                        columns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName),
                        rows = res.Select(x => new { fields = x.ItemArray }),
                        rowCount = dt.Rows.Count,
                        fullRows=dt.Rows.Cast<DataRow>().Select(x=>x.ItemArray)
                    }
                });
            }
            else
            {
                return Json(new{});
            }


        }

        public ActionResult Googlemap(){
            return View();
        }

        public ActionResult ExampleLess()
        {
            return View();
        }

        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        // ----------------------- Simple Table --------------------------
        public ActionResult SimpleTable()
        {
            return View();
        }

        public ActionResult SimpleTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetSimpleTableDemoTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult SimpleTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- Filter Table --------------------------
        public ActionResult FilterTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }


        public ActionResult FilterTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var orderNum = "";
            var clientID = 0;
            List<int?> statusIDs = new List<int?>();

            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                orderNum = parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "";
                clientID = parameters.filter.ContainsKey("clientName") ? RDL.Convert.StrToInt(parameters.filter["clientName"].ToString(), 0) : 0;
                statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusIDs = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                    }
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";
            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("orderNum", orderNum);
            p.Add("clientID", clientID);
            p.Add("statusIDs", String.Join(",", statusIDs));
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("sort2", sort2);
            p.Add("direction2", direction2);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetFilterDemoTable", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");
            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult FilterTable_getItems_oldLINQVariant()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var orderNum = parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "";
                var clientID = parameters.filter.ContainsKey("clientName") ? RDL.Convert.StrToInt(parameters.filter["clientName"].ToString(), 0) : 0;
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusIDs = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
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
                    (clientID == 0 || x.clientID == clientID) &&
                    (statusIDs.Count == 0 || statusIDs.Contains(x.statusID)) &&
                    (createdMin <= x.created && x.created <= createdMax)
                );
                if (orderNum != "")
                {
                    items = items.ToList().Where(x => x.orderNum != null && x.orderNum.IndexOf(orderNum, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- Inline Edit Table --------------------------
        public ActionResult InlineEditTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();

            return View();
        }

        public ActionResult InlineEditTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetInlineEditTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult InlineEditTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var orderNum = parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "";
                var clientID = parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0;
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                items = items.Where(x =>
                    (clientID == 0 || x.clientID == clientID) &&
                    (statusIDs.Count == 0 || statusIDs.Contains(x.statusID))
                );
                if (orderNum != "")
                {
                    items = items.ToList().Where(x => x.orderNum != null && x.orderNum.IndexOf(orderNum, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InlineEditTableInline(int pk, string value, string name)
        {
            var mng = new CRMManager();
            mng.EditOrderField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }



        // ----------------------- Group Operation Table --------------------------
        public ActionResult GroupOperationTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult GroupOperationTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }


            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetGroupOperationTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult GroupOperationTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }

            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }
        // ----------------------- Double Sort Table --------------------------
        public ActionResult DoubleSortTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult DoubleSortTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            string sort1 = sorts.Length > 0 ? sorts[0] : "";

            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            string sort2 = sorts.Length > 1 ? sorts[1] : "";
            string direction2 = directions.Length > 1 ? directions[1] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("SortField2", sort2);
            p.Add("Directions2", direction2);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetDoubleSortTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult DoubleSortTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            IOrderedQueryable<crm_orders> orderedItems = items.OrderByDescending(p => p.created);
            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.orderNum);
                    else orderedItems = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.crm_orderStatuses.name);
                    else orderedItems = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.crm_clients.fio);
                    else orderedItems = items.OrderByDescending(x => x.crm_clients.fio);
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
                    case "orderNum":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.orderNum);
                        else orderedItems = orderedItems.ThenByDescending(x => x.orderNum);
                        break;
                    case "statusName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.crm_orderStatuses.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.crm_orderStatuses.name);
                        break;
                    case "clientName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.crm_clients.fio);
                        else orderedItems = orderedItems.ThenByDescending(x => x.crm_clients.fio);
                        break;
                    default:
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.created);
                        else orderedItems = orderedItems.ThenByDescending(x => x.created);
                        break;
                }
            }

            var total = orderedItems.Count();
            var res = orderedItems.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();
            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- RowButtons Table --------------------------
        public ActionResult RowButtonsTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult RowButtonsTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetRowButtonsTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult RowButtonsTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- ColSettings Table --------------------------
        public ActionResult ColSettingsTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult ColSettingsTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetColSettingsTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");


            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult ColSettingsTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- Comments Table --------------------------
        public ActionResult CommentsTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult CommentsTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetCommentsTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CommentsTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommentsTable_getComments(int itemID)
        {
            var mng = new CommentManager();
            var res = true;

            return Json(new
            {
                result = res,
                items = mng.GetComments("customer", itemID.ToString()).Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    x.username,
                    x.text,
                    audio = PathMap(x.audio)
                })
            }, JsonRequestBehavior.AllowGet);
        }

        private static string PathMap(string path)
        {
            if (path != null)
            {
                string approot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath.TrimEnd('\\');
                return path.Replace(approot, string.Empty).Replace('\\', '/');
            }
            else
            {
                return path;
            }
        }

        public static void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }

        public ActionResult CommentsTable_getAudio(int id)
        {
            var mng = new CommentManager();
            string path = mng.GetComment(id).audio;
            return File(path, "audio/vnd.wave", "voice_" + id + Path.GetExtension(path));
        }

        public ActionResult CommentsTable_addComment(string itemID, string text, HttpPostedFileBase audioBlob)
        {
            var mng = new CommentManager();

            string audio = string.Empty;

            if (audioBlob != null)
            {
                var folderPath = System.Web.HttpContext.Current.Server.MapPath(@"/uploads/comm_voice");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                audio = Path.Combine(folderPath, Guid.NewGuid() + ".mp3");

                //http://stackoverflow.com/questions/20088743/mvc4-app-unable-to-load-dll-libmp3lame-32-dll
                CheckAddBinPath();

                using (var writer = new NAudio.Lame.LameMP3FileWriter(audio, new NAudio.Wave.WaveFormat(44100, 16, 2), NAudio.Lame.LAMEPreset.STANDARD))
                {
                    audioBlob.InputStream.CopyTo(writer);
                }

                //audio = Path.Combine(folderPath, Guid.NewGuid() + ".wav");
                //audioBlob.SaveAs(audio);
            }

            var item = mng.AddComment("customer", itemID, text, audio);

            var res = true;
            return Json(new
            {
                result = res,
                item = new
                {
                    item.id,
                    created = item.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    item.username,
                    item.text,
                    audio = PathMap(item.audio)
                }
            }, JsonRequestBehavior.AllowGet);
        }


        // ----------------------- Export Excel Table --------------------------
        public ActionResult ExportExcelTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult ExportExcelTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var modeType = parameters.mode["type"].ToString();
            if (modeType != "")
            {
                parameters.page = 1;
                parameters.pageSize = int.MaxValue;
            }
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }


            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetExportExcelTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var url = "";
            if (modeType != "")
            {

                url = export(items, parameters.mode, modeType);
            }

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total,
                url = url
            });

            return Content(json, "application/json");
        }

        private void СleaningDirectory(string path)
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string pathFile in files)
            {
                System.IO.File.Delete(pathFile);
            }
        }

        private string export(List<object> items, Dictionary<string, object> mode, string modeType)
        {
            var res = "";
            var modeVisibleCols = mode["visibleCols"] as ArrayList;
            var cols = new Dictionary<string, string>();
            foreach (var col in modeVisibleCols)
            {
                var item = col as Dictionary<string, object>;
                if (item != null)
                {
                    cols.Add(item["code"].ToString(), item["title"].ToString());
                }
            }


            if (modeType == "excel")
            {
                var g = Guid.NewGuid();
                var url = "/uploads/crud/excel/";
                var path = HttpContext.Server.MapPath(String.Format(url));
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                СleaningDirectory(path);

                var workbook = new XLWorkbook();
                if (workbook.Worksheets.Count == 0)
                {
                    workbook.Worksheets.Add("Заказы " + DateTime.Now.ToString("dd.MM.yyyy"));
                }
                var list = workbook.Worksheets.FirstOrDefault();

                IXLCell cell = null;

                var header = list.Range(1, 1, 1, cols.Count);
                header.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 102, 0);

                var allTable = list.Range(1, 1, Math.Min(items.Count + 1, 1001), cols.Count);
                allTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                allTable.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                allTable.Style.Alignment.SetWrapText();

                var currentCol = 1;
                foreach (var col in cols)
                {
                    cell = list.Cell(1, currentCol); cell.Value = col.Value;
                    currentCol++;
                }

                list.Column(1).Width = 16;
                list.Column(2).Width = 16;
                list.Column(3).Width = 12;
                list.Column(4).Width = 10;
                list.Column(5).Width = 20;
                list.Column(6).Width = 10;
                list.Column(7).Width = 10;
                list.Column(8).Width = 15;
                list.Column(9).Width = 12;
                list.Column(10).Width = 12;
                list.Column(11).Width = 25;
                list.Column(12).Width = 12;
                list.Column(13).Width = 12;
                list.Column(14).Width = 12;
                list.Column(15).Width = 12;
                list.Column(16).Width = 12;
                list.Column(17).Width = 12;
                list.Column(18).Width = 12;

                var i = 2;
                foreach (var x in items)
                {
                    var data = (IDictionary<string, object>)x;

                    if (i > 1001) break;
                    currentCol = 1;
                    if (cols.ContainsKey("id"))
                    {
                        cell = list.Cell(i, currentCol); cell.Value = data["id"].ToString();
                        currentCol++;
                    }
                    if (cols.ContainsKey("created"))
                    {
                        cell = list.Cell(i, currentCol); cell.Value = data["created"].ToString(); //x.created.GetValueOrDefault().ToString("dd.MM.yyyy");
                        currentCol++;
                    }
                    if (cols.ContainsKey("orderNum"))
                    {
                        cell = list.Cell(i, currentCol); cell.Value = data["orderNum"].ToString();//x.orderNum;
                        currentCol++;
                    }
                    if (cols.ContainsKey("statusName"))
                    {
                        cell = list.Cell(i, currentCol); cell.Value = data["statusName"].ToString();//x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "";
                        currentCol++;
                    }
                    if (cols.ContainsKey("clientName"))
                    {
                        cell = list.Cell(i, currentCol); cell.Value = data["clientName"].ToString(); ;// x.crm_clients != null ? x.crm_clients.fio : "";
                        currentCol++;
                    }

                    i++;
                }

                workbook.SaveAs(String.Format("{0}{1}.xlsx", path, g));
                res = String.Format("{0}{1}.xlsx", url, g);

            }


            if (modeType == "pdf")
            {
                var g = Guid.NewGuid();
                var url = "/uploads/crud/pdf/";
                var path = HttpContext.Server.MapPath(String.Format(url));
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                СleaningDirectory(path);

                var newFileUrl = String.Format("{0}{1}.pdf", path, g);

                //Create our document object
                Document Doc = new Document(PageSize.A4, 2, 1, 30, 1);

                //Create our file stream
                using (FileStream fs = new FileStream(newFileUrl, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    //Bind PDF writer to document and stream
                    PdfWriter writer = PdfWriter.GetInstance(Doc, fs);

                    //Open document for writing
                    Doc.Open();

                    //Add a page
                    Doc.NewPage();

                    //Full path to the Unicode Arial file
                    string ARIALUNI_TFF = Path.Combine(HttpContext.Server.MapPath(String.Format("/fonts/")), "ARIALUNI.TTF");

                    //Create a base font object making sure to specify IDENTITY-H
                    BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    //Create a specific font object
                    Font f = new Font(bf, 12, Font.NORMAL);

                    //int numCols = 0;
                    List<float> widthList = new List<float>();

                    foreach (var col in cols)
                    {
                        if (col.Key.Equals("id"))
                        {
                            widthList.Add(1f);
                        }
                        if (col.Key.Equals("created"))
                        {
                            widthList.Add(3f);
                        }
                        if (col.Key.Equals("orderNum"))
                        {
                            widthList.Add(3f);
                        }
                        if (col.Key.Equals("statusName"))
                        {
                            widthList.Add(3f);
                        }
                        if (col.Key.Equals("clientName"))
                        {
                            widthList.Add(7f);
                        }
                    }

                    PdfPTable table = new PdfPTable(widthList.ToArray());

                    foreach (var col in cols)
                    {
                        if (col.Key.Equals("id") || col.Key.Equals("created") || col.Key.Equals("orderNum") || col.Key.Equals("statusName") || col.Key.Equals("clientName"))
                        {
                            PdfPCell headCell = new PdfPCell(new Phrase(col.Value, f));
                            headCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            headCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            headCell.BackgroundColor = BaseColor.GRAY;
                            table.AddCell(headCell);
                        }
                    }

                    var i = 1;
                    PdfPCell rowCell = new PdfPCell(new Phrase("", f));
                    foreach (var x in items)
                    {
                        var data = (IDictionary<string, object>)x;
                        if (i > 1001) break;
                        bool GrayRow = false;

                        if (i % 2 == 0)
                        {
                            GrayRow = true;
                        }

                        if (cols.ContainsKey("id"))
                        {
                            rowCell = new PdfPCell(new Phrase(data["id"].ToString()/*x.id.ToString()*/, f));
                            if (GrayRow) { rowCell.BackgroundColor = BaseColor.LIGHT_GRAY; }
                            table.AddCell(rowCell);
                        }
                        if (cols.ContainsKey("created"))
                        {
                            rowCell = new PdfPCell(new Phrase(data["created"].ToString()/*x.created.GetValueOrDefault().ToString("dd.MM.yyyy")*/, f));
                            if (GrayRow) { rowCell.BackgroundColor = BaseColor.LIGHT_GRAY; }
                            table.AddCell(rowCell);
                        }
                        if (cols.ContainsKey("orderNum"))
                        {
                            rowCell = new PdfPCell(new Phrase(data["orderNum"].ToString()/*x.orderNum.ToString()*/, f));
                            if (GrayRow) { rowCell.BackgroundColor = BaseColor.LIGHT_GRAY; }
                            table.AddCell(rowCell);
                        }
                        if (cols.ContainsKey("statusName"))
                        {
                            rowCell = new PdfPCell(new Phrase(data["statusName"].ToString()/*x.crm_orderStatuses != null ? x.crm_orderStatuses.name : ""*/, f));
                            if (GrayRow) { rowCell.BackgroundColor = BaseColor.LIGHT_GRAY; }
                            table.AddCell(rowCell);
                        }
                        if (cols.ContainsKey("clientName"))
                        {
                            rowCell = new PdfPCell(new Phrase(data["clientName"].ToString()/*x.crm_clients != null ? x.crm_clients.fio : ""*/, f));
                            if (GrayRow) { rowCell.BackgroundColor = BaseColor.LIGHT_GRAY; }
                            table.AddCell(rowCell);
                        }

                        i++;
                    }

                    Doc.Add(table);

                    //Close the PDF
                    Doc.Close();
                }

                res = String.Format("{0}{1}.pdf", url, g);
            }
            return res;
        }

        // ----------------------- Simple Table --------------------------
        public ActionResult ShowStatTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult ShowStatTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }


            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetShowStatTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult ShowStatTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ShowStatTable_getDateStat(int itemID, string period, string from)
        {
            var mng = new CRMManager();
            var orders = mng.GetOrders(itemID);

            var minOrderDate = orders.Min(x => x.created).GetValueOrDefault();
            var dateFrom = RDL.Convert.StrToDateTime(from, minOrderDate);
            if (dateFrom == DateTime.MinValue)
            {
                dateFrom = new DateTime(2012, 12, 1);
            }
            var dateTo = new DateTime();
            switch (period)
            {
                case "days":
                    dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, 1);
                    dateTo = dateFrom.AddMonths(1).AddDays(-1);
                    break;
                case "months":
                    dateFrom = new DateTime(dateFrom.Year, 1, 1);
                    dateTo = dateFrom.AddYears(1).AddMonths(-1);
                    break;
                default:
                    dateTo = DateTime.Now;
                    break;
            }

            var headers = new List<string> { "Заказов от клиента" };
            var statItems = new List<DateStatItemModel>();
            var dt = dateFrom;
            var dt2 = new DateTime();
            var periodName = "";
            while (dt <= dateTo)
            {
                switch (period)
                {
                    case "days": dt2 = dt.AddDays(1); periodName = dt.ToString("dd.MM.yyyy"); break;
                    case "months": dt2 = dt.AddMonths(1); periodName = RDL.Dicts.Months[dt.Month - 1] + " " + dt.Year.ToString(); break;
                    default: // years
                        dt2 = dt.AddYears(1); periodName = dt.Year.ToString(); break;
                }
                var el = new DateStatItemModel
                {
                    from = dt.ToString("dd.MM.yyyy"),
                    to = dt2.ToString("dd.MM.yyyy"),
                    periodName = periodName,
                    period = period,
                    values = new List<string>()
                };
                el.values.Add(orders.Count(x => dt <= x.created && x.created < dt2).ToString());
                statItems.Add(el);
                dt = dt2;
            }

            var res = true;

            return Json(new
            {
                result = res,
                items = statItems,
                headers = headers
            }, JsonRequestBehavior.AllowGet);
        }

        public class OrderItemModel
        {
            public int id { set; get; }
            public string color { set; get; }
            public int lastMonthRequestCount { set; get; }
            public int lastMonthInvoiceCount { set; get; }
            public string lastInvoiceDate { set; get; }
        }


        // ----------------------- Simple Table --------------------------
        public ActionResult ReplaceToolTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult ReplaceToolTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetReplaceToolTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult ReplaceToolTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReplaceToolTable_replace(string code, string from, string to)
        {
            var res = false;
            var count = 0;
            try
            {
                var mng = new CRMManager();
                count = mng.ReplaceOrderField(code, from, to);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res,
                count = count
            }, JsonRequestBehavior.AllowGet);
        }


        // -----------------------CopyRow Table --------------------------
        public ActionResult CopyRowTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }

        public ActionResult CopyRowTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }


            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";


            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetCopyRowTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");


            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CopyRowTable_getItemsOldLinq()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CopyRowTable_copy(int id)
        {
            var res = false;
            try
            {
                var mng = new CRMManager();
                var item = mng.GetOrder(id);
                var newItem = new crm_orders { addedBy = User.Identity.Name, clientID = item.clientID, created = DateTime.Now, id = 0, orderNum = item.orderNum, statusID = item.statusID };
                mng.SaveOrder(newItem);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res,
            }, JsonRequestBehavior.AllowGet);
        }
        // ----------------------- FullEditTable --------------------------
        public ActionResult FullEditTable()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();

            return View();
        }


        public ActionResult FullEditTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("orderNum", parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "");
                p.Add("clientID", parameters.filter.ContainsKey("clientID") ? RDL.Convert.StrToInt(parameters.filter["clientID"].ToString(), 0) : 0);

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

                p.Add("statusIDs", String.Join(",", statusIDs));
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            p.Add("SortField", sort1);
            p.Add("Directions", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetFullEditTable]", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");


            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");

        }


        public ActionResult FullEditTable_getItemsOldLinq()
        {



            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.orderNum);
                    else items = items.OrderByDescending(x => x.orderNum);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy")
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }
        // ----------------------- Simple Table --------------------------
        public ActionResult SimplePopovers()
        {
            return View();
        }
        public ActionResult FullEditTable_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var created = id > 0 ? RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("created", fields), DateTime.Now) : DateTime.Now;
                var orderNum = AjaxModel.GetValueFromSaveField("orderNum", fields);
                var addedBy = id > 0 ? AjaxModel.GetValueFromSaveField("addedBy", fields) : User.Identity.Name;

                int? clientID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("clientName", fields), 0);
                if (clientID == 0) clientID = null;

                int? statusID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("statusName", fields), 0);
                if (statusID == 0) statusID = null;

                var item = new crm_orders { id = id, orderNum = orderNum, created = created, addedBy = addedBy, clientID = clientID, statusID = statusID };
                mng.SaveOrder(item);
                savedID = item.id;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- AS Form --------------------------
        public ActionResult ASForm()
        {
            var mng = new CRMManager();
            ViewBag.OrderStatuses = mng.GetOrderStatuses();
            ViewBag.Clients = mng.GetClients();
            return View();
        }



        public ActionResult ASForm_get(string id)
        {
            return Json(new
            {
                orderNum = "",
                clientName = 0
            }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult ASForm_save()
        {
            var res = false;
            var msg = "";
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                var mng = new CRMManager();
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var clientID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("clientName", fields), 0);
                var orderNum = AjaxModel.GetValueFromSaveField("orderNum", fields);
                var item = new crm_orders { id = 0, clientID = clientID, addedBy = User.Identity.Name, created = DateTime.Now, orderNum = orderNum, statusID = mng.GetOrderStatuses().FirstOrDefault(x => x.code == "processing").id };
                mng.SaveOrder(item);
                msg = "Заказ  успешно поступил в обработку";
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                msg = ex.Message;
            }
            return Json(new
            {
                result = res,
                msg = msg
            }, JsonRequestBehavior.AllowGet);
        }

        // ----------------------- Simple Form --------------------------        
        public ActionResult SimpleForm()
        {
            return View();
        }
        // ----------------------- Mailing --------------------------        

        public ActionResult TableExport()
        {
            return View();
        }

        public ActionResult Metrics()
        {
            return View();
        }

        public ActionResult ImageManager()
        {
            return View();
        }
        public ActionResult ImageControls()
        {
            return View();
        }

        public ActionResult FileManager()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/Account/Login/?demoType=FileManager");
                //(String.Format("/Account/Login/?demoType={0}&isGuest={1}", FileManager, 1));

            }

        }

        public ActionResult UserOpinion()
        {
            return View();
        }

        public ActionResult PopupInfo()
        {
            return View();
        }

        public ActionResult SignalR()
        {
            return View();
        }

        public ActionResult CountDown()
        {
            return View();
        }

        public ActionResult VideoBackground()
        {
            return View();
        }

        public ActionResult PlayAudio()
        {
            return View();
        }

        public ActionResult Jubilee()
        {
            return View();
        }

        public ActionResult AsImageAsText()
        {
            return View();
        }
        public ActionResult ImageCrop()
        {
            return View();
        }
        public ActionResult UploadImage()
        {

            var file = HttpContext.Request.Files["Filedata"];
            string savePath = Server.MapPath(@"\uploads\Images\" + file.FileName);
            file.SaveAs(savePath);

            return Content(Url.Content(@"\uploads\Images\" + file.FileName));
        }
        public ActionResult TestImageCrop()
        {
            return View();
        }
        // ----------------------- Simple Form -------------------------- 
        //public ActionResult Calc()
        //{
        //    return View();
        //}

        //public ActionResult Calc_getCalc(string code)
        //{
        //    var calc = new CalcManager().GetCalc(code);
        //    if (calc != null)
        //    {
        //        return Json(new
        //        {
        //            calc.makeup,
        //            calc.calcFunction,
        //            calc.resultFunction,
        //            parameters = calc.calc_parameters.Select(p => new
        //            {
        //                p.code,
        //                dataTypeCode = p.as_dataTypes.code,
        //                p.defaultValue,
        //            }),
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}
        // ----------------------- User Profile --------------------------
        public static string ProfileGetUrl(string pathToUrl, int level)
        {
            string delimiter = @"\";
            string delimiterLevel = null;
            for (int i = level; i > 0; i--)
            {
                delimiterLevel = "";
                for (int j = i; j > 0; j--)
                {
                    delimiterLevel += delimiter;
                }
                pathToUrl = pathToUrl.Replace(delimiterLevel, "/");
            }
            return pathToUrl;
        }
        public static string ProfileGetImageSRC(string userGuid)
        {
            string pathImage = null;
            try
            {

                string PathFind = AppDomain.CurrentDomain.BaseDirectory + PathChapter;
                string[] extensions = { ".jpg", ".png", ".jpeg", ".gif" };
                List<string> files = new List<string>();

                foreach (string ext in extensions)
                {
                    files.AddRange(Directory.GetFiles(PathFind, userGuid + ext, SearchOption.AllDirectories));
                    if (files.Count >= 1)
                    {
                        break;
                    }
                }
                if (files.Count == 1)
                {
                    pathImage = PathChapter + Path.GetFileName(files[0]);
                }
                else
                {
                    pathImage = PathChapter + FileStandart;      //GuiDefault+".jpg";
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return pathImage;
        }
        public ActionResult ProfileFormUser()
        {
            string UserName = "";
            string GuiUser = GuiDefault;
            string UserAvatar = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
                GuiUser = ((Guid)Membership.GetUser().ProviderUserKey).ToString();
                UserAvatar = ProfileGetImageSRC(GuiUser);
            }
            else
            {
                return Redirect("/Account/Login/?demoType=ProfileFormUser");
                //  UserName = "NoRegisterUser";
                // UserAvatar = PathChapter + FileStandart;
            }


            ViewBag.UserName = UserName;
            ViewBag.Avatar = ProfileGetUrl(UserAvatar, 1);
            ViewBag.UserGuid = GuiUser;
            return View();
        }
        public ActionResult ProfileGetUserData()
        {
            string UserSurname = "";
            string UserName = "";
            string UserPatronymic = "";
            string UserEmail = "";
            string UserSkype = "";
            string UserPhoneTwo = "";
            string GuiUser = GuiDefault;
            if (User.Identity.IsAuthenticated)
            {
                GuiUser = ((Guid)Membership.GetUser().ProviderUserKey).ToString();
            }
            ProfileManager pm = new ProfileManager();
            UserSurname = pm.GetProperty("surname", "pfu", GuiUser);
            UserName = pm.GetProperty("name", "pfu", GuiUser);
            UserPatronymic = pm.GetProperty("otch", "pfu", GuiUser);
            UserEmail = pm.GetProperty("email", "pfu", GuiUser);
            UserSkype = pm.GetProperty("skype", "pfu", GuiUser);
            UserPhoneTwo = pm.GetProperty("phoneTwo", "pfu", GuiUser);

            return Json(new
            {
                UserSurname = UserSurname,
                UserName = UserName,
                UserPatronymic = UserPatronymic,
                UserEmail = UserEmail,
                UserSkype = UserSkype,
                UserPhoneTwo = UserPhoneTwo
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProfileSaveImage(string UserGuid, HttpPostedFileBase FileImg)
        {
            string error = "Error";
            string imgFile = PathChapter + FileStandart;
            string PathCurrent = AppDomain.CurrentDomain.BaseDirectory + PathChapter;

            if (FileImg.ContentLength > 0 && FileImg.ContentLength < 2100000)
            {

                try
                {
                    string extension = "";
                    if (Request.Browser.Browser.ToUpper() == "IE")
                    {
                        string[] files = FileImg.FileName.Split(new char[] { '\\' });
                        extension = files[files.Length - 1];
                    }
                    else
                        extension = Path.GetExtension(FileImg.FileName);
                    imgFile = PathChapter + UserGuid + extension;
                    FileImg.SaveAs(PathCurrent + UserGuid + extension);
                    error = "Success";
                }
                catch (Exception ex)
                {
                    RDL.Debug.LogError(ex);
                }
            }
            return Json(new
            {
                error = error,
                imgFile = ProfileGetUrl(imgFile, 1)
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProfileContactDataSave(string UserGuid, string UserSurname, string UserName, string UserPatronymic, string UserEmail, string UserSkype, string UserPhoneTwo)
        {
            string error = "Error";
            try
            {
                ProfileManager pm = new ProfileManager();
                pm.SetProperty("surname", UserSurname, UserGuid);
                pm.SetProperty("name", UserName, UserGuid);
                pm.SetProperty("otch", UserPatronymic, UserGuid);
                pm.SetProperty("email", UserEmail, UserGuid);
                pm.SetProperty("skype", UserSkype, UserGuid);
                pm.SetProperty("phoneTwo", UserPhoneTwo, UserGuid);
                error = "Success";
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                error = error
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsCtrl()
        {
            return View();
        }
        public ActionResult AsCtrlSave(string datacode, string datatype)
        {

            AsCtrlManager mng = new AsCtrlManager();
            as_ctrl obj = new as_ctrl();
            obj.code = datacode;
            obj.type = datatype;
            obj.username = this.User.Identity.Name;
            obj.url = this.Request.Url.ToString();
            obj.referrer = this.Request.UrlReferrer.ToString();
            obj.guid = Guid.NewGuid();
            obj.created = DateTime.Now;
            mng.GetCompMonitorings();
            string status = "BAD";
            if (mng.SaveCompMonitoring(obj) == 0)
            {
                status = "OK";
            }

            return Content(
            Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                STATUS = status
            }), "application/json");
        }

        public ActionResult TestV()
        {
            return View();
        }
        //------------------------Hot Keys---------------------------------
        public ActionResult HotKeysDemo()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/Account/Login/?demoType=HotKeysDemo");

            }
        }
        public ActionResult HotKeysDemo_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new HotKeyManager();
            IQueryable<as_hotkeys> items = mng.GetHotKeys().AsQueryable();
            var res = items;

            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.Id,
                    key = (x.isAlt == true ? "Alt+" : "") + (x.isCtrl == true ? "Ctrl+" : "") + (x.isShift == true ? "Shift+" : "") +
                             (
                              x.keyCode == 8 ? "backspace" :
                              x.keyCode == 9 ? "Tab" :
                              x.keyCode == 13 ? "enter" :
                              x.keyCode == 19 ? "pause/break" :
                              x.keyCode == 20 ? "caps lock" :
                              x.keyCode == 27 ? "escape" :
                              x.keyCode == 33 ? "page up" :
                              x.keyCode == 34 ? "page down" :
                              x.keyCode == 35 ? "end" :
                              x.keyCode == 36 ? "home" :
                              x.keyCode == 37 ? "left arrow" :
                              x.keyCode == 38 ? "up arrow" :
                              x.keyCode == 39 ? "right arrow" :
                              x.keyCode == 40 ? "down arrow" :
                              x.keyCode == 45 ? "insert" :
                              x.keyCode == 46 ? "delete" :
                              x.keyCode == 48 ? "0" :
                              x.keyCode == 49 ? "1" :
                              x.keyCode == 50 ? "2" :
                              x.keyCode == 51 ? "3" :
                              x.keyCode == 52 ? "4" :
                              x.keyCode == 53 ? "5" :
                              x.keyCode == 54 ? "6" :
                              x.keyCode == 55 ? "7" :
                              x.keyCode == 56 ? "8" :
                              x.keyCode == 57 ? "9" :
                              x.keyCode == 65 ? "A" :
                              x.keyCode == 66 ? "B" :
                              x.keyCode == 67 ? "C" :
                              x.keyCode == 68 ? "D" :
                              x.keyCode == 69 ? "E" :
                              x.keyCode == 70 ? "F" :
                              x.keyCode == 71 ? "G" :
                              x.keyCode == 72 ? "H" :
                              x.keyCode == 73 ? "I" :
                              x.keyCode == 74 ? "J" :
                              x.keyCode == 75 ? "K" :
                              x.keyCode == 76 ? "L" :
                              x.keyCode == 77 ? "M" :
                              x.keyCode == 78 ? "N" :
                              x.keyCode == 79 ? "O" :
                              x.keyCode == 80 ? "P" :
                              x.keyCode == 81 ? "Q" :
                              x.keyCode == 82 ? "R" :
                              x.keyCode == 83 ? "S" :
                              x.keyCode == 84 ? "T" :
                              x.keyCode == 85 ? "U" :
                              x.keyCode == 86 ? "V" :
                              x.keyCode == 87 ? "W" :
                              x.keyCode == 88 ? "X" :
                              x.keyCode == 89 ? "Y" :
                              x.keyCode == 90 ? "Z" :
                              x.keyCode == 91 ? "left window key" :
                              x.keyCode == 92 ? "right window key" :
                              x.keyCode == 93 ? "select key" :
                              x.keyCode == 96 ? "numpad 0" :
                              x.keyCode == 97 ? "numpad 1" :
                              x.keyCode == 98 ? "numpad 2" :
                              x.keyCode == 99 ? "numpad 3" :
                              x.keyCode == 100 ? "numpad 4" :
                              x.keyCode == 101 ? "numpad 5" :
                              x.keyCode == 102 ? "numpad 6" :
                              x.keyCode == 103 ? "numpad 7" :
                              x.keyCode == 104 ? "numpad 8" :
                              x.keyCode == 105 ? "numpad 9" :
                              x.keyCode == 106 ? "multiply" :
                              x.keyCode == 107 ? "add" :
                              x.keyCode == 109 ? "subtract" :
                              x.keyCode == 110 ? "decimal point" :
                              x.keyCode == 111 ? "divide" :
                              x.keyCode == 112 ? "f1" :
                              x.keyCode == 113 ? "f2" :
                              x.keyCode == 114 ? "f3" :
                              x.keyCode == 115 ? "f4" :
                              x.keyCode == 116 ? "f5" :
                              x.keyCode == 117 ? "f6" :
                              x.keyCode == 118 ? "f7" :
                              x.keyCode == 119 ? "f8" :
                              x.keyCode == 120 ? "f9" :
                              x.keyCode == 121 ? "f10" :
                              x.keyCode == 122 ? "f11" :
                              x.keyCode == 123 ? "f12" :
                              x.keyCode == 144 ? "num lock" :
                              x.keyCode == 145 ? "scroll lock" :
                              x.keyCode == 186 ? "semi-colon" :
                              x.keyCode == 187 ? "equal sign" :
                              x.keyCode == 188 ? "comma" :
                              x.keyCode == 189 ? "dash" :
                              x.keyCode == 190 ? "period" :
                              x.keyCode == 191 ? "forward slash" :
                              x.keyCode == 192 ? "grave accent" :
                              x.keyCode == 219 ? "open bracket" :
                              x.keyCode == 220 ? "back slash" :
                              x.keyCode == 221 ? "close braket" :
                              x.keyCode == 222 ? "single quote" : ""
                              ),
                    action = x.js != null ? x.js : x.url != null ? "Redirect to:" + x.url : "",
                    roles = "<div class='usHotKeyRoles'>" + x.roles + "</div>"

                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Access(int id)
        {
            bool res = false;
            try
            {
                var mng = new HotKeyManager();
                res = mng.HaveCurrentUserAccess(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(res);
        }
        public ActionResult HotKeysForUser()
        {
            var items = new List<as_hotkeys>();
            try
            {
                var mng = new HotKeyManager();
                items = mng.GetHotKeys();
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            var total = items.Count();
            return Json(new
            {
                items = items.Select(x => new
                {
                    id = x.Id,
                    key = x.keyCode,
                    isAlt = x.isAlt != null && x.isAlt.Value ? true : false,
                    isCtrl = x.isCtrl != null && x.isCtrl.Value ? true : false,
                    isShift = x.isShift != null && x.isShift.Value ? true : false,
                    js = x.js,
                    url = x.url,
                    roles = x.roles
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Car()
        {

            return View();

        }

        //public ActionResult Skype()
        //{
        //    SkypeApplication.Instance.Start();

        //    var status = SkypeApplication.Instance.MyStatus;
        //    var contacts = SkypeApplication.Instance.ContactList;

        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach (var user in contacts)
        //    {
        //        list.Add(new SelectListItem() { Text = user.UserInfo.Name, Value = user.UserInfo.login });
        //    }
        //    ViewBag.contacts = list;
        //    ViewBag.contacts1 = contacts;

        //    ViewBag.status = status;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Skype(string name, string msg)
        //{
        //    SkypeApplication.Instance._skype.SendMessage(name, msg);
        //    var contacts = SkypeApplication.Instance.ContactList;
        //    var status = SkypeApplication.Instance.MyStatus;
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach (var user in contacts)
        //    {
        //        list.Add(new SelectListItem() { Text = user.UserInfo.Name, Value = user.UserInfo.login });
        //    }
        //    ViewBag.contacts = list;
        //    ViewBag.status = status;
        //    return new HttpStatusCodeResult(HttpStatusCode.OK);
        //}

        //-----------------------Javascript template engine--------------------------
        public ActionResult DotJsDemo()
        {
            return View();
        }
        //--------------------------Domain checker------------------------

        public ActionResult DomainChecker()
        {
            return View();
        }


        public ActionResult CheckDomain(string domain)
        {
            string result = new StreamReader(WebRequest.Create("http://whoisapi.netfox.ru/api_v1/?domain=" + domain).GetResponse().GetResponseStream()).ReadToEnd();


            return Json(new
            {
                result = (result == "0") ? "домен свободен" : "домен занят",
                domain = domain
            }, JsonRequestBehavior.AllowGet);
        }

        //--------------------------Domain checker------------------------

        public ActionResult Indicators()
        {
            return View();
        }

        public ActionResult Location()
        {
            return View();
        }

        public ActionResult Weather()
        {
            return View();
        }

        public ActionResult Currency()
        {
            return View();
        }

        public ActionResult TegReplace()
        {
            return View();
        }
        
        public ActionResult Mosaik() {
            return View();
        }

        public ActionResult GetMosaik()
        {
            var model = new MosaikViewModel()
            {
                Images = Directory.EnumerateFiles(Server.MapPath("/Content/images/mosaik/"))
                .Select(fn => "/Content/images/mosaik/" + Path.GetFileName(fn))
            };
            return Json(model.Images ,JsonRequestBehavior.AllowGet);
        }

        #region Vacancy
        public ActionResult Vacancy()
        {
            return View();
        }
    #endregion

        #region TimeLine
        public ActionResult TimeLine()
        {
            return View();
        }

        #endregion

        #region Mailchimp
        //-------------------------Mailchimp-----------------------------
        public ActionResult Mailchimp()
        {
            return View();
        }

        public ActionResult Mailchimp_getItems()
        {
            var res = BLL.Mailchimp.Mailchimp.GetAllLists();
            var json = res.Replace("lists", "items");

            return Content(json, "application/json");
        }

        #endregion

        #region ApiForSendSms
        //-------------------------ApiForSendSms-----------------------------

        public ActionResult ApiForSendSms()
        {
            return View();
        }

        [HttpGet]
        public JsonResult SendSmsUseApi(string phone, string message)
        {
            var res = BLL.SmsUseApi.SmsUseApi.SendSmsUseApi(phone, message);
            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion
    
        #region MapPlaces
        public ActionResult MapPlaces()
        {
            return View();
        } 
        #endregion

        public JsonResult getUnregQueries()
        {
            LocalSqlServer db = new LocalSqlServer();
            var q = new SqlQueries();
            
            List<rosh_sqlQueriesTable> inpParameters = db.rosh_sqlQueriesTable.ToList();
            List<string> unrQueries = q.GetUnregQueries(inpParameters);

            return Json(unrQueries, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult Calendar_getEvents()
        {
            var mng = new CalendarManager();
            var items = mng.GetEvents();

            var json = JsonConvert.SerializeObject(items);
            return Content(json, "application/json");
        }
    }

}