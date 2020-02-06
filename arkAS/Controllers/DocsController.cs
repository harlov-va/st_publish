using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.BLL.Docs;
using arkAS.BLL.HR;
using arkAS.BLL.Finance;
using arkAS.Models;
using Newtonsoft.Json;
using Dapper;
using System.Collections;
using System.Web.Security;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web.Http.ModelBinding;
using DocumentFormat.OpenXml.Spreadsheet;

namespace arkAS.Controllers
{
    [Authorize(Roles = "admin, manager")]
    public class DocsController : Controller
    {

        #region Docs

        public ActionResult Docs()
        {
            var mng = new DocsManager();
            var finMng = new FinanceManager();
            ViewBag.DocTypes = mng.GetDocTypes();
            ViewBag.DocStatuses = mng.GetDocStatuses();
            ViewBag.FinProjects = finMng.GetProjects().OrderBy(x => x.name).ToList();
            ViewBag.FinContragents = finMng.GetFinContragents().OrderBy(x => x.name).ToList();
            return View();
        }

        public ActionResult Docs_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var name = "";
            var number = "";
            var path = "";
            var typeID = "";
            var statusID = "";
            var projectID = "";
            var contragentID = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                number = parameters.filter.ContainsKey("number") ? parameters.filter["number"].ToString() : "";
                path = parameters.filter.ContainsKey("path") ? parameters.filter["path"].ToString() : "";
                typeID = parameters.filter.ContainsKey("typeID") ? parameters.filter["typeID"].ToString() : "0";
                statusID = parameters.filter.ContainsKey("statusID") ? parameters.filter["statusID"].ToString() : "0";
                projectID = parameters.filter.ContainsKey("projectID") ? parameters.filter["projectID"].ToString() : "0";
                contragentID = parameters.filter.ContainsKey("contragentID") ? parameters.filter["contragentID"].ToString() : "0";

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
            p.Add("name", name);
            p.Add("number", number);
            p.Add("path", path);
            p.Add("typeID", typeID);
            p.Add("statusID", statusID);
            p.Add("projectID", projectID);
            p.Add("contragentID", contragentID);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetDocs", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateDoc(string name, string number, string path, int typeID, int projectID, int contragentID)
        {
            var mng = new DocsManager();
            int statusID = 1;
            var item = new doc_docs { 
                id = 0, 
                name = name,
                number = number,
                path = path, 
                typeID = typeID, 
                statusID = statusID, 
                projectID = projectID, 
                contragentID = contragentID, 
                created = DateTime.Now.Date,
                createdBy = new BLL.Core.CoreManager().GetUserGuid()
             }; 
            mng.SaveDoc(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDoc(int id)
        {
            var mng = new DocsManager();
            var item = mng.GetDoc(id);

            item.path = CopyFileOnFolderID(item.id, item.path);
            
            return Json(new
            {
                id = item.id,
                name = item.name,
                number = item.number,
                path = item.path,
                typeID = item.typeID,
                statusID = item.statusID,
                projectID = item.projectID,
                contragentID = item.contragentID,
                created = item.created.ToString("dd.MM.yyyy")
            });
        }

        public ActionResult DocEdit(doc_docs item)
        {
            var mng = new DocsManager();
            mng.SaveDoc(item);
           
            return Json(new { result = true });
        }

        public ActionResult DownloadDoc(int docId, string path)
        {
            DocLog(docId, true);

            string fullPath =path;
            
            return Json(new
            {
                result = fullPath

            }, JsonRequestBehavior.AllowGet);
        }

        public bool DocLog(int docId, bool isDown)
        {
           var mng = new DocsManager();
            var res = false;
            try
            {
                var id = 0;
                var docID = docId;
                var createdBy = Membership.GetUser().UserName;
                var isDownload = isDown;
                var created = DateTime.Now;
                var item = new doc_docLogs { id = id, docID = docID, isDownload = isDownload, created = created, createdBy = createdBy };
                
                mng.SaveDocLogs(item);
                res = true;

            }
            catch (Exception ex){
                
                RDL.Debug.LogError(ex);
            }
            return res;
        } 
        
        [HttpPost]
        public ActionResult UploadDoc(int docId, string note)
        {
            string directoryPath = Server.MapPath("~/uploads/Docs/") + "ID" + docId +"/";
            var res = false;
            var fileName = "";
            var exMessage = "";

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    var file = files[0];

                    fileName = Upload(file, directoryPath);
                    string namePath = directoryPath + fileName;
                    file.SaveAs(namePath);

                    fileName = "/uploads/Docs/ID" + docId + "/" + fileName;
                    
                    DocLog(docId, false);
                    DocLogVersion(docId, note);
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


        public bool DocLogVersion(int docId, string note)
        {
            var mng = new DocsManager();
            var res = false;
            
            try
            {
                var id = 0;
                var createdBy = Membership.GetUser().UserName;
                var docID = docId;
                var decs = note;
                var created = DateTime.Now;
                var item = new doc_docVersions { id = id, createdBy = createdBy, docID = docID, decs = decs, created = created };

                mng.SaveDocVersions(item);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return res;
        }

        public ActionResult DocsInline(int pk, string value, string name)
        {
            var mng = new DocsManager();
            mng.EditDocField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Docs_remove(int id)
        {
            var res = false;
            var mng = new DocsManager();

            var item = mng.GetDoc(id);
            if (item != null)
            {
                mng.DeleteDoc(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Документ удален!"
            });
        }

        public ActionResult GetListTemplatesByTypeId(int typeId)
        {
            var mng = new DocsManager();
            var items = mng.GetListTemplatesByType(typeId);

            return Json (new { result = items.Select(x => new
            {
                id = x.id,
                path = x.path,
                name = x.name,
                typeId = x.typeID
             
            }
            )});
        }

        public string CopyFileOnFolderID(int id, string path)
        {
            string dirPathTempate = Server.MapPath(path);

            string fileName = Path.GetFileName(path);
            string folderName = "ID" + id + "/";
            string pathFolder = "/uploads/Docs/";
            string pathDoc = pathFolder + folderName;
            string dirPathDoc =Server.MapPath(pathDoc) + fileName;

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
                else{
                }
                
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }

        #endregion

        #region DocTypes

        public ActionResult DocTypes()
        {
            return View();
        }

        public ActionResult DocTypes_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetDocTypes", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult DocTypes_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new DocsManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var code = AjaxModel.GetValueFromSaveField("code", fields);

                var item = new doc_docTypes { id = id, name = name, code = code };
                mng.SaveDocType(item);
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

        public ActionResult DocTypes_remove(int id)
        {
            var res = false;
            var mng = new DocsManager();

            var item = mng.GetDocType(id);
            if (item != null)
            {
                mng.DeleteDocType(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Тип удален!"
            });
        }

        #endregion

        #region DocStatuses

        public ActionResult DocStatuses()
        {
            return View();
        }

        public ActionResult DocStatuses_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetDocStatuses", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult DocStatuses_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new DocsManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var code = AjaxModel.GetValueFromSaveField("code", fields);
                var color = AjaxModel.GetValueFromSaveField("color", fields);

                var item = new doc_docStatuses { id = id, name = name, code = code, color = color };
                mng.SaveDocStatus(item);
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

        public ActionResult DocStatuses_remove(int id)
        {
            var res = false;
            var mng = new DocsManager();

            var item = mng.GetDocStatus(id);
            if (item != null)
            {
                mng.DeleteDocStatus(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Статус удален!"
            });
        }

        #endregion

        #region DocTypeTemplates

        public ActionResult DocTypeTemplates()
        {
            ViewBag.DocTypes = new DocsManager().GetDocTypes();
            return View();
        }

        public ActionResult GetDocTypeTemplate(int id)
        {
            var mng = new DocsManager();
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

        public ActionResult DocTypeTemplateEdit(doc_docTypeTemplates item)
        {
            var mng = new DocsManager();
            mng.SaveDocTypeTemplate(item);
            return Json(new { result = true });
        }

        public ActionResult DocTypeTemplates_getItems()
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
            var items = rep.GetSQLData<dynamic>("GetDocTypeTemplates", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateDocTypeTemplate(string name, int typeID, string path, int? ord)
        {
            var mng = new DocsManager();
            var item = new doc_docTypeTemplates { id = 0, name = name, typeID = typeID, path = path, ord = ord };
            mng.SaveDocTypeTemplate(item);

            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocTypeTemplates_remove(int id)
        {
            var res = false;
            var mng = new DocsManager();

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
            string  resPath = "";
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
                   for (;;)
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


        #region DocLogs

        public ActionResult DocLogs()
        {
            var mng = new DocsManager();
            ViewBag.Docs = mng.GetDocs();
            return View();
        }
        public ActionResult DocLogs_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var isDownload = "";
            var createdBy = "";
            var name = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                isDownload=parameters.filter.ContainsKey("isDownload") ? parameters.filter["isDownload"].ToString() : "0";
                createdBy = parameters.filter.ContainsKey("createdBy") ? parameters.filter["createdBy"].ToString() : "";
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString()
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
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }
            var rep = new CoreRepository();
                var p = new DynamicParameters();
                p.Add("isDownLoad", isDownload);
                p.Add("createdBy", createdBy);
                p.Add("name", name);
                p.Add("createdMin", createdMin);
                p.Add("createdMax", createdMax);

                var items = rep.GetSQLData<dynamic>("GetDocLogs", p, CommandType.StoredProcedure); 
                var json = JsonConvert.SerializeObject(new
                {
                    items
                });
                return Content(json, "application/json");
            }
       
        public ActionResult DocLogs_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new DocsManager();
            var res = false;
            var savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var createdBy = Membership.GetUser().UserName;
                var docID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("name", fields),0);
                var isDownload = RDL.Convert.StrToBoolean(AjaxModel.GetValueFromSaveField("isDownload", fields));
                var created = DateTime.Now;
                var item = new doc_docLogs { id = id, createdBy = createdBy, docID = docID, isDownload = isDownload, created = created };


                mng.SaveDocLogs(item);
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

        public ActionResult DocLogs_remove(int id)
        {
            var res = false;
            var mng = new DocsManager();

            var item = mng.GetDocLog(id);
            if (item != null)
            {
                mng.DeleteDocLogs(item.id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Лог удалён!"
            });
        }
        public ActionResult DocLogsInline(int pk, string value, string name)
        {
            var mng = new DocsManager();
            mng.EditTextField(pk,name,value);
            return Json(new
            {
                result = true
            });
        }
        #endregion

        #region DocVersion

        public ActionResult DocVersions()
        {
            var mng = new DocsManager();
            ViewBag.Docs = mng.GetDocs();
            return View();
        }
        public ActionResult DocVersions_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var createdBy = "";
            var name = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                createdBy = parameters.filter.ContainsKey("createdBy") ? parameters.filter["createdBy"].ToString() : "";
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString()
                        .Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(),
                            (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(),
                            (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("createdBy", createdBy);
            p.Add("name", name);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);

            var items = rep.GetSQLData<dynamic>("GetDocVersions", p, CommandType.StoredProcedure);
            var json = JsonConvert.SerializeObject(new
            {
                items
            });
            return Content(json, "application/json");
        }
        public ActionResult DocVersions_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new DocsManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var createdBy = Membership.GetUser().UserName;
                var docID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("name", fields), 0);
                var decs = AjaxModel.GetValueFromSaveField("decs", fields);
                var created = DateTime.Now;
                var item = new doc_docVersions { id = id, createdBy = createdBy, docID = docID, decs = decs, created = created };


                mng.SaveDocVersions(item);
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

        public ActionResult DocVersions_remove(int id)
        {
            var res = false;
            var mng = new DocsManager();

            var item = mng.GetDocVersion(id);
            if (item != null)
            {
                mng.DeleteDocVersions(item.id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Версия удалена!"
            });
        }
        public ActionResult DocVersionsInline(int pk, string value, string name)
        {
            var mng = new DocsManager();
            mng.EditTextField2(pk, name, value);
            return Json(new
            {
                result = true
            });
        }
        #endregion
    }
}
