using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.Areas.harlov.BLL;
using Newtonsoft.Json;
using arkAS.BLL;
using arkAS.Models;
using System.Collections;
using System.Web.Security;

namespace arkAS.Areas.harlov.Controllers
{
    [Authorize(Roles = "Admin, Guest")]
    public class MailController : BaseController
    {
        public MailController(IManager mng) : base(mng)
        {           
        }
        #region Mails
        public ActionResult MailsList()
        {
            var user = mng.GetUser();
            string msg = "";
            var model = new Models.MailViewModel
            {
                DeliverySystems = mng.Mails.GetDeliverySystems(out msg, user),
                MailStatuses = mng.Mails.GetMailStatuses(out msg, user)
            };
            return View(model);
        }
        public ActionResult MailsList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Mails.GetMails(out msg, user);
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                if (text != "")
                {
                    items = items.Where(x => x.fromSender != null && x.fromSender.Contains(text)).ToList();
                }
                var delSysID = parameters.filter.ContainsKey("delSysID") ? RDL.Convert.StrToInt(parameters.filter["delSysID"].ToString(), 0) : 0;
                if (delSysID != 0)
                {
                    items = items.Where(x => x.deliverySystemID == delSysID).ToList();
                }
                
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                if (statusIDs.Count != 0)
                {
                    items = items.Where(x => statusIDs.Contains(x.mailStatusID)).ToList();
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "number":
                    if (direction1 == "up") items = items.OrderBy(x => x.fromSender).ToList();
                    else items = items.OrderByDescending(x => x.fromSender).ToList();
                    break;
                case "email":
                    if (direction1 == "up") items = items.OrderBy(x => x.date).ToList();
                    else items = items.OrderByDescending(x => x.date).ToList();
                    break;
                case "sum":
                    if (direction1 == "up") items = items.OrderBy(x => x.toRecipient).ToList();
                    else items = items.OrderByDescending(x => x.toRecipient).ToList();
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.fromSender).ToList();
                    else items = items.OrderByDescending(x => x.fromSender).ToList();
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            var json = JsonConvert.SerializeObject(new
            {
                items = res.Select(x => new
                {
                    id = x.id,
                    uniqueCode = x.uniqueCode,
                    date = x.date.ToShortDateString(),
                    fromSender = x.fromSender,
                    toRecipient = x.toRecipient,
                    description = x.description,
                    trackNumber = x.trackNumber,
                    backTrackNumber = x.backTrackNumber,
                    backDateRecipient = x.backDateRecipient,
                    delSystemsName = x.h_deliverySystems != null ? x.h_deliverySystems.name : "undefined",
                    mailStatuses = x.h_mailStatuses.name
                }),
                msg = msg,
                total = total
            });
            return Json(json);
        }
    
        public ActionResult MailsList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_mails item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Mails.CreateMail(parameters, out msg, user);
            }
            else
            {
                item = mng.Mails.EditMail(parameters, currentID, out msg, user);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            //Content(json, "application/json")
            return Json(json);
        }

        public ActionResult MailsList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Mails.RemoveMail(id, out msg, user);
            if (!result)
            {
                return Json(new { result = result, msg = msg });
            }
            else
            {
                return Json(new { result = result });
            }
        }
        public ActionResult Mails_changeInline(int pk, string value, string name)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Mails.ChangeMailsStatus(pk, name, value, out msg, user);
            if (!result)
            {
                return Json(new { result = result, msg = msg });
            }
            else
            {
                return Json(new { result = result });
            }
        }
        public ActionResult GetMailLogs(int id)
        {
            //var res = mng.Mails.GetMailLogsByID(id);
            //var json = JsonConvert.SerializeObject(new
            //{
            //    items = res.Select(x => new
            //    {
            //        date = x.date.ToShortDateString(),
            //        notice = x.notice,
            //        userName = x.userName
            //    })
            //});
            //return Content(json, "application/json");
            var items = mng.Mails.GetMailLogsByID(id, true);
            var json = JsonConvert.SerializeObject(new
            {
                items
            });
            return Content(json, "application/json");
        }
        #endregion
        #region MailStatus
        public ActionResult MailStatusesList()
        {
            return View();
        }
        public ActionResult MailStatusesList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Mails.GetMailStatuses(out msg, user);
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                if (text != "")
                {
                    items = items.Where(x => x.name != null && x.name.Contains(text)).ToList();
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name).ToList();
                    else items = items.OrderByDescending(x => x.name).ToList();
                    break;
                case "email":
                    if (direction1 == "up") items = items.OrderBy(x => x.code).ToList();
                    else items = items.OrderByDescending(x => x.code).ToList();
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.name).ToList();
                    else items = items.OrderByDescending(x => x.name).ToList();
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            var json = JsonConvert.SerializeObject(new
            {
                items = res.Select(x => new {
                    id = x.id,
                    name = x.name,
                    code = x.code
                }),
                msg = msg,
                total = total
            });
            return Content(json, "application/json");
        }
        public ActionResult MailStatusesList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_mailStatuses item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Mails.CreateMailStatus(parameters, out msg, user);
            }
            else
            {
                item = mng.Mails.EditMailStatus(parameters, currentID, out msg, user);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            return Content(json, "application/json");

        }

        public ActionResult MailStatusesList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Mails.RemoveMailStatus(id, out msg, user);
            if (!result)
            {
                return Json(new { result = result, msg = msg });
            }
            else
            {
                return Json(new { result = result });
            }
        }
        #endregion
        #region DeliverySystems
        public ActionResult DeliverySystemsList()
        {
            return View();
        }
        public ActionResult DeliverySystemsList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Mails.GetDeliverySystems(out msg, user);
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                if (text != "")
                {
                    items = items.Where(x => x.name != null && x.name.Contains(text)).ToList();
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name).ToList();
                    else items = items.OrderByDescending(x => x.name).ToList();
                    break;
                case "email":
                    if (direction1 == "up") items = items.OrderBy(x => x.code).ToList();
                    else items = items.OrderByDescending(x => x.code).ToList();
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.name).ToList();
                    else items = items.OrderByDescending(x => x.name).ToList();
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            var json = JsonConvert.SerializeObject(new
            {
                items = res.Select(x => new {
                    id = x.id,
                    name = x.name,
                    code = x.code
                }),
                msg = msg,
                total = total
            });
            return Content(json, "application/json");
        }
        public ActionResult DeliverySystemsList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_deliverySystems item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Mails.CreateDeliverySystem(parameters, out msg, user);
            }
            else
            {
                item = mng.Mails.EditDeliverySystem(parameters, currentID, out msg, user);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            return Content(json, "application/json");
        }
        public ActionResult DeliverySystemsList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = mng.Mails.RemoveDeliverySystem(id, out msg, user);
            if (!result)
            {
                return Json(new { result = result, msg = msg });
            }
            else
            {
                return Json(new { result = result });
            }
        }
        #endregion
    }
}