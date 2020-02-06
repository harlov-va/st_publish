using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.Gurevskiy;
using arkAS.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class GurevskiyController : Controller
    {
        #region Main
        // GET: Gurevskiy
        public ActionResult Index()
        {
            var mng = new GurevskiyRepository();

            ViewBag.Partners = mng.GetPartners();
            ViewBag.ContractTypes = mng.GetContractTypes();
            ViewBag.ContractStatus = mng.GetContractStatuses();
            ViewBag.InvoiceStatus = mng.GetInvoiceStatuses();
            ViewBag.MailStatus = mng.GetMailStatuses();

            return View();
        }
        #endregion

        #region Contracts
        public ActionResult Contracts_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var number = "";
            var typeID = 0;
            var statusID = 0;
            var partnerID = 0;
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                number = parameters.filter.ContainsKey("number") ? parameters.filter["number"].ToString() : "";
                typeID = parameters.filter.ContainsKey("typeName") ? RDL.Convert.StrToInt(parameters.filter["typeName"].ToString(), 0) : 0;
                statusID = parameters.filter.ContainsKey("statusName") ? RDL.Convert.StrToInt(parameters.filter["statusName"].ToString(), 0) : 0;
                partnerID = parameters.filter.ContainsKey("partnerName") ? RDL.Convert.StrToInt(parameters.filter["partnerName"].ToString(), 0) : 0;

                if (parameters.filter.ContainsKey("date") && parameters.filter["date"] != null)
                {
                    var dates = parameters.filter["date"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
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


            var rep = new CoreRepository();
            var p = new DynamicParameters();

            p.Add("number", number);
            p.Add("typeID", typeID);
            p.Add("statusID", statusID);
            p.Add("partnerID", partnerID);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("Gurevskiy_GetContracts", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });

            return Content(json, "application/json");
        }

        public ActionResult Contracts_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new GurevskiyRepository();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var created = RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("date", fields), DateTime.Now);
                var number = AjaxModel.GetValueFromSaveField("number", fields);
                var typeID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("typeName", fields), 0);
                var statusID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("statusName", fields), 0);
                var partnerID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("partnerName", fields), 0);
                var sum = RDL.Convert.StrToDecimal(AjaxModel.GetValueFromSaveField("sum", fields), 0);
                var comment = AjaxModel.GetValueFromSaveField("comment", fields);
                var link = AjaxModel.GetValueFromSaveField("link", fields);

                var item = new gurevskiy_contracts { id = id, typeID = typeID, statusID = statusID, partnerID = partnerID, date = created, number = number, sum = sum, comment = comment, link = link };
                res = mng.SaveContract(item);
                savedID = item.id;
            }
            catch
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

        public ActionResult Contracts_remove(int id)
        {
            var res = false;
            var mng = new GurevskiyRepository();
            var msg = "Ошибка удаления контракта!";

            var item = mng.GetContract(id);
            if (item != null)
            {
                res = mng.DeleteContract(id);
                if (res)
                    msg = "Контракт удален!";
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }
        #endregion


        #region Invoices
        public ActionResult Invoices_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var number = "";
            var statusID = 0;
            var partnerID = 0;
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                number = parameters.filter.ContainsKey("number") ? parameters.filter["number"].ToString() : "";
                statusID = parameters.filter.ContainsKey("statusName") ? RDL.Convert.StrToInt(parameters.filter["statusName"].ToString(), 0) : 0;
                partnerID = parameters.filter.ContainsKey("partnerName") ? RDL.Convert.StrToInt(parameters.filter["partnerName"].ToString(), 0) : 0;

                if (parameters.filter.ContainsKey("date") && parameters.filter["date"] != null)
                {
                    var dates = parameters.filter["date"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
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


            var rep = new CoreRepository();
            var p = new DynamicParameters();

            p.Add("number", number);
            p.Add("statusID", statusID);
            p.Add("partnerID", partnerID);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("Gurevskiy_GetInvoices", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });

            return Content(json, "application/json");
        }

        public ActionResult Invoices_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new GurevskiyRepository();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var created = RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("date", fields), DateTime.Now);
                var number = AjaxModel.GetValueFromSaveField("number", fields);
                var statusID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("statusName", fields), 0);
                var partnerID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("partnerName", fields), 0);
                var comment = AjaxModel.GetValueFromSaveField("comment", fields);

                var item = new gurevskiy_invoices { id = id, statusID = statusID, partnerID = partnerID, date = created, number = number, comment = comment };
                res = mng.SaveInvoice(item);
                savedID = item.id;
            }
            catch
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

        public ActionResult Invoices_remove(int id)
        {
            var res = false;
            var mng = new GurevskiyRepository();
            var msg = "Ошибка удаления счета!";

            var item = mng.GetInvoice(id);
            if (item != null)
            {
                res = mng.DeleteInvoice(id);
                if (res)
                    msg = "Счет удален!";
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }
        #endregion

        #region Mails
        public ActionResult Mails_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var from = "";
            var to = "";
            var statusID = 0;
            var numberTrack = "";
            var numberTrackBack = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                from = parameters.filter.ContainsKey("from") ? parameters.filter["from"].ToString() : "";
                to = parameters.filter.ContainsKey("to") ? parameters.filter["to"].ToString() : "";
                statusID = parameters.filter.ContainsKey("statusName") ? RDL.Convert.StrToInt(parameters.filter["statusName"].ToString(), 0) : 0;
                numberTrack = parameters.filter.ContainsKey("numberTrack") ? parameters.filter["numberTrack"].ToString() : "";
                numberTrackBack = parameters.filter.ContainsKey("numberTrackBack") ? parameters.filter["numberTrackBack"].ToString() : "";

                if (parameters.filter.ContainsKey("date") && parameters.filter["date"] != null)
                {
                    var dates = parameters.filter["date"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
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


            var rep = new CoreRepository();
            var p = new DynamicParameters();

            p.Add("from", from);
            p.Add("to", to);
            p.Add("statusID", statusID);
            p.Add("numberTrack", numberTrack);
            p.Add("numberTrackBack", numberTrackBack);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("Gurevskiy_GetMails", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });

            return Content(json, "application/json");
        }

        public ActionResult Mails_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new GurevskiyRepository();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var statusID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("statusName", fields), 0);
                var created = RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("date", fields), DateTime.Now);
                var from = AjaxModel.GetValueFromSaveField("from", fields);
                var to = AjaxModel.GetValueFromSaveField("to", fields);
                var desc = AjaxModel.GetValueFromSaveField("desc", fields);
                var systemSending = AjaxModel.GetValueFromSaveField("systemSending", fields);
                var numberTrack = AjaxModel.GetValueFromSaveField("numberTrack", fields);
                var numberTrackBack = AjaxModel.GetValueFromSaveField("numberTrackBack", fields);
                var dateBack = RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("dateBack", fields), (DateTime)System.Data.SqlTypes.SqlDateTime.Null);

                var item = new gurevskiy_mails { id = id, statusID = statusID, date = created, from = from, to = to, desc = desc, systemSending = systemSending, numberTrack = numberTrack, numberTrackBack = numberTrackBack, dateBack = dateBack };
                res = mng.SaveMail(item);
                savedID = item.id;
            }
            catch
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

        public ActionResult Mails_remove(int id)
        {
            var res = false;
            var mng = new GurevskiyRepository();
            var msg = "Ошибка удаления почты!";

            var item = mng.GetMail(id);
            if (item != null)
            {
                res = mng.DeleteMail(id);
                if (res)
                    msg = "Почта удалена!";
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }
        #endregion


    }
}