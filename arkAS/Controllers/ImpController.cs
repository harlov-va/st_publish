using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.Imp;
using arkAS.Models;
using Dapper;
using Newtonsoft.Json;

namespace arkAS.Controllers
{
    [Authorize(Roles = "admin")]
    public class ImpController : Controller
    {
        // GET: Imp
        public ActionResult Index()
        {
            return View();
        }

        // --------------------- ItemsDict --------------------------
        public ActionResult Items()
        {
            return View();
        }

        public ActionResult Items_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new ImpManager();
            var name = "";

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new ImpRepository();
            var p = new DynamicParameters();
            p.Add("name", name);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetItems", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");
            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
 
        }
        public ActionResult Items_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new ImpManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var code = AjaxModel.GetValueFromSaveField("code", fields);
                var datatable = AjaxModel.GetValueFromSaveField("datatable", fields);
                var sqlInsert = AjaxModel.GetValueFromSaveField("sqlInsert", fields);
                var sqlUpdate = AjaxModel.GetValueFromSaveField("sqlUpdate", fields);
                var sqlDelete = AjaxModel.GetValueFromSaveField("sqlDelete", fields);


                var item = new imp_items
                {
                    id = id,
                    name = name,
                    code = code,
                    datatable = datatable,
                    sqlInsert = sqlInsert,
                    sqlUpdate = sqlUpdate,
                    sqlDelete = sqlDelete

                };
                mng.SaveItem(item);
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

        public ActionResult Items_remove(int id)
        {
            var res = false;
            var mng = new ImpManager();

            //проверка на дочерние
            var itemchild = mng.GetItemLogs(id);
            if (itemchild.Count != 0)
            {
                return Json(new
                {
                    result = res,
                    msg = "Сперва удалите/измените логи по действиям!"
                });
            }


            var item = mng.GetItem(id);
            if (item != null)
            {
                mng.DeleteItem(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Действие удалено !"
            });

        }

        public ActionResult GoItemLogsByItemID(int itemID)
        {
            var mng = new ImpManager();

            return View();
        }


        //// --------------------- ItemLogsDict --------------------------
        public ActionResult ItemLogs()
        {
            var mng = new ImpManager();
            ViewBag.Items = mng.GetItems();//items с первичным ключом

            return View();
        }

        public ActionResult ItemLogsEdit(imp_itemLog item)
        {
            var mng = new ImpManager();
            mng.SaveItemLog(item);
            return Json(new { result = true });
        }

        public ActionResult GetItemLog(int id)
        {
            var mng = new ImpManager();
            var item = mng.GetItemLog(id);
            return Json(new
            {
                id = item.id,
                created = item.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                createdBy = item.createdBy,
                durationSec = item.durationSec,
                errors = item.errors,
                itemID = item.itemID,
                withBackup = item.withBackup,
                isImport = item.isImport,
                info = item.info
            });
        }

        public ActionResult GetItemLogDefault()
        {
            return Json(new 
            {
                createdBy = User.Identity.Name,
                created = DateTime.Today.ToString("dd.MM.yyyy")
            });
        }

        public ActionResult ItemLogsCreate(imp_itemLog item)
        {
            var mng = new ImpManager();
            mng.SaveItemLog(item);
            return Json(new { result = true });
        }

        //public ActionResult ItemLogs_getItems()
        //{
        //    var parameters = AjaxModel.GetParameters(HttpContext);
        //    var mng = new ImpManager();

        //    DateTime createdMin = (DateTime) System.Data.SqlTypes.SqlDateTime.MinValue;
        //    DateTime createdMax = (DateTime) System.Data.SqlTypes.SqlDateTime.MaxValue;

        //    var isImport = -1;
        //    var withBackup = -1;
        //    var itemID = 0;

        //    //---------------обработка фильтров
        //    if (parameters.filter != null && parameters.filter.Count > 0)
        //    {

        //        if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
        //        {
        //            var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (dates.Length > 0)
        //            {
        //                createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
        //            }
        //            if (dates.Length > 1)
        //            {
        //                createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
        //            }
        //        }

        //        if (parameters.filter.ContainsKey("isImport") && parameters.filter["isImport"] != null)
        //        {
        //            isImport = RDL.Convert.StrToInt(parameters.filter["isImport"].ToString(), 0);
        //        }

        //        if (parameters.filter.ContainsKey("withBackup") && parameters.filter["withBackup"] != null)
        //        {
        //            withBackup = RDL.Convert.StrToInt(parameters.filter["withBackup"].ToString(), 0);
        //        }

        //        if (parameters.filter.ContainsKey("itemId") && parameters.filter["itemId"] != null)
        //        {
        //            itemID = RDL.Convert.StrToInt(parameters.filter["itemId"].ToString(), 0);
        //            if (itemID == -1) itemID = 0;
        //        }
        //    }
        //    var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        //    var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        //    var sort1 = sorts.Length > 0 ? sorts[0] : "";
        //    var direction1 = directions.Length > 0 ? directions[0] : "";

        //    var rep = new ImpRepository();
        //    var p = new DynamicParameters();
        //    p.Add("isImport",isImport);
        //    p.Add("withBackup",withBackup);
        //    p.Add("itemID",itemID);
        //    p.Add("createdMin",createdMin);
        //    p.Add("createdMax",createdMax);
        //    p.Add("sort1", sort1);
        //    p.Add("direction1", direction1);
        //    p.Add("page", parameters.page);
        //    p.Add("pageSize", parameters.pageSize);
        //    p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //    var items = rep.GetSQLData<dynamic>("GetItemLogs", p, CommandType.StoredProcedure);

        //    var total = p.Get<int>("total");

        //    var json = JsonConvert.SerializeObject(new
        //    {
        //        items,
        //        total = total
        //    });
        //    return Content(json, "application/json");
        //}

        public ActionResult ItemLogs_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new ImpManager();

            var items = mng.GetItemLogs().AsQueryable();

            //---------------обработка фильтров
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
                DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
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
                    items = items.Where(x => x.created >= createdMin && x.created <= createdMax).AsQueryable();
                }



                if (parameters.filter.ContainsKey("isImport") && parameters.filter["isImport"] != null)
                {
                    var isImport = RDL.Convert.StrToInt(parameters.filter["isImport"].ToString(), 0);
                    if (isImport == 1) items = items.Where(x => x.isImport == true).AsQueryable();
                    if (isImport == 0) items = items.Where(x => x.isImport == false).AsQueryable();
                }

                if (parameters.filter.ContainsKey("withBackup") && parameters.filter["withBackup"] != null)
                {
                    var withBackup = RDL.Convert.StrToInt(parameters.filter["withBackup"].ToString(), 0);
                    if (withBackup == 0) items = items.Where(x => x.withBackup == false).AsQueryable();
                    if (withBackup == 1) items = items.Where(x => x.withBackup == true).AsQueryable();
                }

                if (parameters.filter.ContainsKey("itemId") && parameters.filter["itemId"] != null)
                {
                    var itemID = RDL.Convert.StrToInt(parameters.filter["itemId"].ToString(), 0);
                    if (itemID != -1) items = items.Where(x => x.itemID == itemID).AsQueryable();
                }

            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "itemID":
                    if (direction1 == "up") items = items.OrderBy(x => x.itemID);
                    else items = items.OrderByDescending(x => x.itemID);
                    break;
                case "created":
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
                case "createdBy":
                    if (direction1 == "up") items = items.OrderBy(x => x.createdBy);
                    else items = items.OrderByDescending(x => x.createdBy);
                    break;
                case "errors":
                    if (direction1 == "up") items = items.OrderBy(x => x.errors);
                    else items = items.OrderByDescending(x => x.errors);
                    break;
                case "info":
                    if (direction1 == "up") items = items.OrderBy(x => x.info);
                    else items = items.OrderByDescending(x => x.info);
                    break;
                case "durationSec":
                    if (direction1 == "up") items = items.OrderBy(x => x.durationSec);
                    else items = items.OrderByDescending(x => x.durationSec);
                    break;
                case "withBackup":
                    if (direction1 == "up") items = items.OrderBy(x => x.withBackup);
                    else items = items.OrderByDescending(x => x.withBackup);
                    break;
                case "isImport":
                    if (direction1 == "up") items = items.OrderBy(x => x.isImport);
                    else items = items.OrderByDescending(x => x.isImport);
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
                    itemName = x.imp_items.name,
                    x.itemID,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                    x.createdBy,
                    x.errors,
                    x.info,
                    x.durationSec,
                    withBackup = (x.withBackup == true) ? "Да" : "Нет",
                    isImport = (x.isImport == true) ? "Да" : "Нет"
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemLogs_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new ImpManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var created = id > 0 ? RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("created", fields), DateTime.Now) : DateTime.Now;
                var createdBy = AjaxModel.GetValueFromSaveField("createdBy", fields);
                var errors = AjaxModel.GetValueFromSaveField("errors", fields);
                var info = AjaxModel.GetValueFromSaveField("info", fields);
                var durationSec = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("info", fields), 0);
                var withBackup = (AjaxModel.GetValueFromSaveField("withBackup", fields) == "") ? false : true;
                var isImport = (AjaxModel.GetValueFromSaveField("isImport", fields) == "") ? false : true;

                int? itemID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("itemName", fields), 0);
                if (itemID == 0) itemID = null;

                var item = new imp_itemLog
                {
                    id = id,
                    created = created,
                    createdBy = createdBy,
                    errors = errors,
                    durationSec = durationSec,
                    info = info,
                    isImport = isImport,
                    withBackup = withBackup,
                    itemID = itemID
                };
                mng.SaveItemLog(item);
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

        public ActionResult ItemLogs_remove(int id)
        {
            var res = false;
            var mng = new ImpManager();


            var item = mng.GetItemLog(id);
            if (item != null)
            {
                mng.DeleteItemLog(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Лог удален!"
            });

        }

        //-----------------------StatisticImportExport------------------------
        public ActionResult StatisticImportExport()
        {
            var oper = new ImpManager();
            ViewBag.Operations = oper.GetItems();

            var users = new CoreManager();
            ViewBag.Users = users.GetUsers().ToList();

            var tables = new TablesDataBase();
            ViewBag.Objects = tables.GetTablesDataBaseList();

            return View();
        }
        public ActionResult StatisticImportExport_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new ImpManager();

            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            var user = "";
            var operationId = 0;
            var datatable = "";
            var checkError = false;

            //--------------обработка фильтров--------------
            if (parameters.filter != null && parameters.filter.Count > 0)
            {

                if (parameters.filter.ContainsKey("creat") && parameters.filter["creat"] != null)
                {
                    var dates = parameters.filter["creat"].ToString()
                        .Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(),
                            (DateTime) System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(),
                            (DateTime) System.Data.SqlTypes.SqlDateTime.MaxValue);
                    }
                }

                if (parameters.filter.ContainsKey("user") && parameters.filter["user"] != null)
                {
                    var userId = RDL.Convert.StrToGuid(parameters.filter["user"].ToString(),Guid.Empty);
                    var users = new CoreManager();
                    user = userId == Guid.Empty ? "" : users.GetUsers().First(i => i.UserId == userId).UserName;
                }

                if (parameters.filter.ContainsKey("oper") && parameters.filter["oper"] != null)
                {
                    operationId = RDL.Convert.StrToInt(parameters.filter["oper"].ToString(), 0);
                }

                if (parameters.filter.ContainsKey("object") && parameters.filter["object"] != null)
                {
                    datatable = parameters.filter["object"].ToString();
                }

                if (parameters.filter.ContainsKey("checked") && parameters.filter["checked"] != null)
                {
                    checkError = Convert.ToBoolean(parameters.filter["checked"].ToString());
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new ImpRepository();
            var p = new DynamicParameters();
            p.Add("user",user);
            p.Add("operationId", operationId);
            p.Add("datatable", datatable);
            p.Add("checkError",checkError);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetStatisticImportExport", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }
    }
}