using arkAS.Areas.harlov.BLL;
using arkAS.Areas.harlov.Models;
using arkAS.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;

namespace arkAS.Areas.harlov.Controllers
{
    [Authorize(Roles = "Admin, Guest")]
    //[AllowAnonymous]
    public class DocumentsController : BaseController
    {
        public DocumentsController(IManager mng) : base(mng)
        { }
        #region Documents
        // GET: harlov/Document
        public ActionResult DocumentsList()
        {
            var user = mng.GetUser();
            string msg = "";
            var model = new DocumentViewModel
            {
                Contragents = mng.Contragents.GetContragents(user, out msg),
                DocStatuses = mng.Documents.GetDocStatuses(user, out msg),
                DocTypes = mng.Documents.GetDocTypes(user, out msg),
                Documents = mng.Documents.GetDocuments(user, out msg)
            };
            return View(model);
        }
        public ActionResult DocumentsList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Documents.GetDocuments(user, out msg);
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                if (text != "")
                {
                    items = items.Where(x => x.number != null && x.number.Contains(text)).ToList();
                }
                var ctrgID = parameters.filter.ContainsKey("ctrgID") ? RDL.Convert.StrToInt(parameters.filter["ctrgID"].ToString(), 0) : 0;
                if (ctrgID != 0)
                {
                    items = items.Where(x => x.contragentID == ctrgID).ToList();
                }
                var docTypeID = parameters.filter.ContainsKey("docTypeID") ? RDL.Convert.StrToInt(parameters.filter["docTypeID"].ToString(), 0) : 0;
                if (docTypeID != 0)
                {
                    items = items.Where(x => x.docTypeID == docTypeID).ToList();
                }
                var parentDocID = parameters.filter.ContainsKey("parentDocID") ? RDL.Convert.StrToInt(parameters.filter["parentDocID"].ToString(), 0) : 0;
                if (parentDocID != 0)
                {
                    items = items.Where(x => x.docParentID == parentDocID).ToList();
                }
                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusIDs"))
                {
                    statusIDs = (parameters.filter["statusIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                if (statusIDs.Count != 0)
                {
                    items = items.Where(x => statusIDs.Contains(x.docStatusID)).ToList();
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "number":
                    if (direction1 == "up") items = items.OrderBy(x => x.number).ToList();
                    else items = items.OrderByDescending(x => x.number).ToList();
                    break;
                case "email":
                    if (direction1 == "up") items = items.OrderBy(x => x.date).ToList();
                    else items = items.OrderByDescending(x => x.date).ToList();
                    break;
                case "sum":
                    if (direction1 == "up") items = items.OrderBy(x => x.sum).ToList();
                    else items = items.OrderByDescending(x => x.sum).ToList();
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.number).ToList();
                    else items = items.OrderByDescending(x => x.number).ToList();
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
                    number = x.number,
                    sum = x.sum,
                    description = x.description,
                    link = x.link,
                    isDeleted = x.isDeleted,
                    contragentName = x.h_contragents !=null ? x.h_contragents.name : "undefined",
                    docStatus = x.h_docStatuses != null ? x.h_docStatuses.name : "undefined",
                    docTypes = x.h_docTypes != null ? x.h_docTypes.name : "undefined",
                    ParentDocs = x.h_documents2 != null ? x.h_documents2.number : "undefined"
                }),
                msg = msg,
                total = total
            }
            );
            return Content(json, "application/json");
        }
        public ActionResult DocumentsList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_documents item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Documents.CreateDocument(parameters, user, out msg);
            }
            else
            {
                item = mng.Documents.EditDocument(parameters, currentID, user, out msg);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            return Content(json, "application/json");
        }
        public ActionResult DocumentsList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Documents.RemoveDocument(id, user, out msg);
            if (!result)
            {
                return Json(new { result = result, msg = msg });
            }
            else
            {
                return Json(new { result = result });
            }
        }
        public ActionResult Documents_changeInLine(int pk, string value, string name)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Documents.ChangeDocumentInLine(pk, name, value, user, out msg);
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
        #region DocStatuses
        public ActionResult DocStatusesList()
        {
            return View();
        }
        public ActionResult DocStatusesList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Documents.GetDocStatuses(user, out msg);
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
                items = res.Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    code = x.code
                }),
                msg = msg,
                total = total
            });
            return Content(json, "application/json");
        }
        public ActionResult DocStatusesList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_docStatuses item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Documents.CreateDocStatus(parameters, user, out msg);
            }
            else
            {
                item = mng.Documents.EditDocStatus(parameters, currentID, user, out msg);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            return Content(json, "application/json");
        }
        public ActionResult DocStatusesList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Documents.RemoveDocStatus(id, user, out msg);
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
        #region DocTypes
        public ActionResult DocTypesList()
        {
            return View();
        }
        public ActionResult DocTypesList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Documents.GetDocTypes(user,out msg);
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
                items = res.Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    code = x.code
                }),
                msg = msg,
                total = total
            });
            //Content(json, "application/json");
            return Json(json);
        }
        public ActionResult DocTypesList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_docTypes item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Documents.CreateDocType(parameters, user,out msg);
            }
            else
            {
                item = mng.Documents.EditDocType(parameters, currentID, user, out msg);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            return Content(json, "application/json");
        }

        public ActionResult DocTypesList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Documents.RemoveDocType(id, user, out msg);
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