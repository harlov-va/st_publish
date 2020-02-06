using arkAS.BLL.Core;
using arkAS.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL_Mikhailova;
using arkAS.BLL;
using System.IO;

namespace arkAS.Controllers
{
    public class MikhailovaDocsController : Controller
    {
        #region Contract

        public ActionResult Contracts()
        {
            var mng=new ContractsManager();
            ViewBag.ListType = mng.GetDocTypes();
            ViewBag.ListTemplate = mng.GetDocTypeTemplates();
            ViewBag.ListContagent = mng.GetContagents();
            ViewBag.ListStatuses = mng.GetStatusesContract();
            return View();
        }

        public ActionResult Contracts_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var number = "";
            var date = "";
            var contagentID = "";
            var summa = "";
            var desc = "";
            var statusID = "";
            var link = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            
           if (parameters.filter != null && parameters.filter.Count > 0)
            {
                number = parameters.filter.ContainsKey("number") ? parameters.filter["number"].ToString() : "";
                date = parameters.filter.ContainsKey("date") ? parameters.filter["date"].ToString() : "";
                statusID = parameters.filter.ContainsKey("statusID") ? parameters.filter["statusID"].ToString() : "0";
                contagentID = parameters.filter.ContainsKey("contagentID") ? parameters.filter["contagentID"].ToString() : "0";

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
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
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
            p.Add("contagentID", contagentID);
            p.Add("statusID", statusID);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetContractsMikh", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateContract(string number, string path, int typeID, int contagentID, string desc, decimal summa)
        {
            var mng = new ContractsManager();
            int statusID = 1;
            var item = new Mikhailova_contracts
            {
                id = 0,
                number = number,
                date = DateTime.Now.Date,
                path = path,
                typeID = typeID,
                statusID = statusID,
                contagentID = contagentID,
                summa = summa,
                desc = desc
            };
            mng.SaveContract(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContract(int id)
        {
            var mng = new ContractsManager();
            var item = mng.GetContract(id);

            item.path = CopyFileOnFolderID(item.id, item.path);

            return Json(new
            {
                id = item.id,
                number = item.number,
                path = item.path,
                typeID = item.typeID,
                statusID = item.statusID,
                contagentID = item.contagentID,
                date = item.date.ToString("dd.MM.yyyy"),
                summa = item.summa,
                desc = item.desc
            });
        }

        public ActionResult EditContract(Mikhailova_contracts item)
        {
            var mng = new ContractsManager();
            mng.SaveContract(item);

            return Json(new { result = true });
        }

        public ActionResult ContractsInline(int pk, string value, string name)
        {
            var mng = new ContractsManager();
            mng.EditDocField(pk, name, value);

            return Json(new
            {
                result = true
            });
        }

        public ActionResult DownloadDoc(int docId, string path)
        {
            string fullPath = path;

            return Json(new
            {
                result = fullPath

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadDoc(int docId, string note)
        {
            string directoryPath = Server.MapPath("~/uploads/Contracts/") + "ID" + docId + "/";
            var res = false;
            var fileName = "";
            var exMessage = "";

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    var file = files[0];

                    fileName = UploadContract(file, directoryPath);
                    string namePath = directoryPath + fileName;
                    file.SaveAs(namePath);

                    fileName = "/uploads/Contracts/ID" + docId + "/" + fileName;

                    res = true;
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res,
                fileName = fileName,
                exMessage = exMessage

            }, JsonRequestBehavior.AllowGet);
        }

        public string UploadContract(HttpPostedFileBase file, string directoryPath)
        {
            string finalFileName = "";
            int counter = 0;

            if (file != null && file.ContentLength > 0)
            {
                for (; ; )
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(fileName);
                    fileName = Path.GetFileNameWithoutExtension(fileName);

                    finalFileName = fileName + "_" + ((counter).ToString()) + extension;
                    string namePath = directoryPath + finalFileName;

                    if (System.IO.File.Exists(namePath))
                    {
                        ++counter;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return finalFileName;
        }

        public string CopyFileOnFolderID(int id, string path)
        {
            string dirPathTempate = Server.MapPath(path);

            string fileName = Path.GetFileName(path);
            string folderName = "ID" + id + "/";
            string pathFolder = "/uploads/Contracts/";
            string pathDoc = pathFolder + folderName;
            string dirPathDoc = Server.MapPath(pathDoc) + fileName;

            var res = pathDoc + fileName;

            try
            {
                if (!System.IO.File.Exists(dirPathDoc))
                {
                    if (!Directory.Exists(Server.MapPath(pathDoc)))
                    {
                        Directory.CreateDirectory(Server.MapPath(pathDoc));
                    }

                    System.IO.File.Copy(dirPathTempate, dirPathDoc, true);
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }

        public ActionResult GetListTemplatesByTypeId(int typeId)
        {
            var mng = new ContractsManager();
            var items = mng.GetListTemplatesByType(typeId);

            return Json(new
            {
                result = items.Select(x => new
                {
                    id = x.id,
                    path = x.path,
                    name = x.name,
                    typeId = x.typeID

                }
                    )
            });
        }

        #endregion

        #region Templates

        public ActionResult Templates()
        {
            var mng=new ContractsManager();
            ViewBag.DocTypes = mng.GetDocTypes();
            return View();
        }

        public ActionResult Templates_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var name = "";
            var typeID = "";

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                typeID = parameters.filter.ContainsKey("typeID") ? parameters.filter["typeID"].ToString() : "0";
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("name", name);
            p.Add("typeID", typeID);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetDocTypeTemplatesMikh", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateTemplate(string name, int typeID, string path, int ord)
        {
            var mng = new ContractsManager();
            var item = new Mikhailova_docTypeTemplates { id = 0, name = name, typeID = typeID, path = path, ord = ord };
            mng.SaveDocTypeTemplate(item);

            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTemplate(int id)
        {
            var mng = new ContractsManager();
            var item = mng.GetDocTypeTemplate(id);
            return Json(new
            {
                id = item.id,
                name = item.name,
                typeID = item.typeID,
                path = item.path,
                ord = item.ord,
            });
        }

        public ActionResult TemplateEdit(Mikhailova_docTypeTemplates item)
        {
            var mng = new ContractsManager();
            mng.SaveDocTypeTemplate(item);
            return Json(new { result = true });
        }

        public ActionResult TemplatesInline(int pk, string value, string name)
        {
            var mng = new ContractsManager();
            mng.EditTemplateField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Templates_remove(int id)
        {
            var res = false;
            var mng = new ContractsManager();

            var item = mng.GetDocTypeTemplate(id);
            if (item != null)
            {
                mng.DeleteDocTypeTemplate(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Шаблон удален!"
            });
        }

        [HttpPost]
        public ActionResult UploadTemplate()
        {
            string directoryPath = "/uploads/templatesDoc/";
            var res = false;
            string resPath = "";
            var exMessage = "";

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    var file = files[0];

                    var fileName = Upload(file, directoryPath);

                    string path = Server.MapPath(directoryPath) + fileName;

                    file.SaveAs(path);
                    resPath = directoryPath + fileName;
                    res = true;
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
                RDL.Debug.LogError(ex);
            }

            return Json(new
            {
                result = res,
                path = resPath,
                exMessage = exMessage

            }, JsonRequestBehavior.AllowGet);
        }

        public string Upload(HttpPostedFileBase file, string directoryPath)
        {
            string finalFileName = "";
            int counter = 0;

            if (file != null && file.ContentLength > 0)
            {
                for (; ; )
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(fileName);
                    fileName = Path.GetFileNameWithoutExtension(fileName);

                    finalFileName = fileName + "_" + ((counter).ToString()) + extension;
                    string namePath = directoryPath + finalFileName;

                    if (System.IO.File.Exists(namePath))
                    {
                        ++counter;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return finalFileName;
        }

        #endregion

        #region Invoices

        public ActionResult Invoices()
        {
            var mng = new ContractsManager();
            ViewBag.ListContagent = mng.GetContagents();
            ViewBag.ListStatuses = mng.GetStatusesInvoice();
            return View();
        }

        public ActionResult Invoices_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var number = "";
            var date = "";
            var contagentID = "";
            var desc = "";
            var statusID = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                number = parameters.filter.ContainsKey("number") ? parameters.filter["number"].ToString() : "";
                date = parameters.filter.ContainsKey("date") ? parameters.filter["date"].ToString() : "";
                statusID = parameters.filter.ContainsKey("statusID") ? parameters.filter["statusID"].ToString() : "0";
                contagentID = parameters.filter.ContainsKey("contagentID") ? parameters.filter["contagentID"].ToString() : "0";

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
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
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
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("statusID", statusID);
            p.Add("contagentID", contagentID);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetInvoicesMikh", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateInvoice(string number, int contagentID, string desc)
        {
            var mng = new ContractsManager();
            int statusID = 1;
            var item = new Mikhailova_invoices
            {
                id = 0,
                number = number,
                date = DateTime.Now.Date,
                statusID = statusID,
                contagentID = contagentID,
                desc = desc
            };
            mng.SaveInvoice(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InvoicesInline(int pk, string value, string name)
        {
            var mng = new ContractsManager();
            mng.EditInvoiceField(pk, name, value);

            return Json(new
            {
                result = true
            });
        }
        
        public ActionResult GetInvoice(int id)
        {
            var mng = new ContractsManager();
            var item = mng.GetInvoice(id);
            
            return Json(new
            {
                id = item.id,
                number = item.number,
                statusID = item.statusID,
                contagentID = item.contagentID,
                date = item.date.ToString("dd.MM.yyyy"),
                desc = item.desc
            });
        }

        public ActionResult EditInvoice(Mikhailova_invoices item)
        {
            var mng = new ContractsManager();
            mng.SaveInvoice(item);

            return Json(new { result = true });
        }

         #endregion

        #region Mails

        public ActionResult Mails()
        {
            var mng = new ContractsManager();
            ViewBag.ListStatuses = mng.GetStatusesMail();
            return View();
        }

        public ActionResult Mails_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var date = "";
            var from = "";
            var to = "";
            var desc = "";
            var systemMail = "";
            var treckNumber = "";
            var statusID = "";
            var treckNumberReplay = "";
            var dateReplay = "";
           
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                treckNumber = parameters.filter.ContainsKey("treckNumber") ? parameters.filter["treckNumber"].ToString() : "";
                date = parameters.filter.ContainsKey("date") ? parameters.filter["date"].ToString() : "";
                statusID = parameters.filter.ContainsKey("statusID") ? parameters.filter["statusID"].ToString() : "0";

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
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();

            p.Add("treckNumber", treckNumber);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("statusID", statusID);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetMailsMikh", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateMails(string treckNumber, string from, string to, string systemMail, string desc)
        {
            var mng = new ContractsManager();
            int statusID = 1;
            var item = new Mikhailova_mails
            {
                id = 0,
                treckNumber = treckNumber,
                date = DateTime.Now.Date,
                from = from,
                to = to,
                statusID = statusID,
                systemMail = systemMail,
                treckNumberReplay = "",
                dateReplay = DateTime.Now.Date,
                desc = desc
            };
            mng.SaveMail(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MailsInline(int pk, string value, string name)
        {
            var mng = new ContractsManager();
            mng.EditMailField(pk, name, value);

            return Json(new
            {
                result = true
            });
        }
        
        public ActionResult GetMail(int id)
        {
            var mng = new ContractsManager();
            var item = mng.GetMail(id);

            return Json(new
            {
                id = item.id,
                treckNumber = item.treckNumber,
                from = item.from,
                to = item.to,
                statusID = item.statusID,
                systemMail = item.systemMail,
                date = item.date.ToString("dd.MM.yyyy"),
                treckNumberReplay = item.treckNumberReplay,
                dateReplay = item.dateReplay,  //.ToString("dd.MM.yyyy"),
                desc = item.desc
            });
        }

        public ActionResult EditMails(Mikhailova_mails item)
        {
            var mng = new ContractsManager();
            mng.SaveMail(item);

            return Json(new { result = true });
        }

        #endregion
      
    }
}