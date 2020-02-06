using arkAS.BLL;
using arkAS.BLL.Article;
using arkAS.Models;
using RDL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace arkAS.Controllers
{
    public class ArticleController : Controller
    {
        public ActionResult GetArticleItems()
        {
            var art = new ArticleManager();
            return View(art.GetItems());
        }

        public ActionResult GetArticleItem(int id)
        {
            var art = new ArticleManager();
            return View(art.GetItem(id));
        }

        #region Admin
        // Редактирование новостей
        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTable()
        {
            var art = new ArticleManager();
            ViewBag.NewsType = art.GetItemsType();
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var art = new ArticleManager();
            var acim = new ArticleCatalogImagesManager();

            var items = art.GetItems().AsQueryable();

            #region filter
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                List<int?> td = new List<int?>();
                if (parameters.filter.ContainsKey("typeID"))
                {
                    td = (parameters.filter["typeID"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                items = items.Where(x => (td.Count == 0 || td.Contains(x.typeID != null ? x.typeID : 0))); 
            }
            #endregion

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "created":
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
                case "title":
                    if (direction1 == "up") items = items.OrderBy(x => x.title);
                    else items = items.OrderByDescending(x => x.title);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }

            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();
            var imgUrl = new FileItemInfo1();

            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                    x.title,
                    imgPath = (imgUrl = Files.GetTargetFiles(x.id, "/uploads/Images/Articles/").Where(s => s.Name.Contains("_mini")).FirstOrDefault()) == null
                        ? "<div class='myUsUploadImage' data-imgPath=''></div>"
                        : String.Format("<div class='myUsUploadImage' data-imgPath='" + "/uploads/Images/Articles/" + x.id + "/" + imgUrl.Name + "'></div>"),
                    x.anouns,
                    x.text,
                    typeID = x.typeID != null ? art.GetItemType((int)x.typeID).name : "",
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTableInline(int pk, string value, string name)
        {
            var art = new ArticleManager();
            art.EditItemField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTable_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var art = new ArticleManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var created = id > 0 ? RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("created", fields), DateTime.Now) : DateTime.Now;
                var title = AjaxModel.GetValueFromSaveField("title", fields);
                var anouns = AjaxModel.GetValueFromSaveField("anouns", fields);
                int? typeID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("typeID", fields), 0);
                if (typeID == 0) typeID = null;

                var item = new art_news { id = id, created = created, title = title, anouns = anouns, typeID = typeID };
                art.SaveItem(item);
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

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTable_remove(int id)
        {
            var res = false;
            var art = new ArticleManager();
            var item = art.GetItem(id);
            var msg = "";
            if (item != null)
            {
                art.DeleteItem(id);
				ArticleImageDelete(id);
                msg = "Запись успешно удалена!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleText(int id)
        {
            var res = false;
            var art = new ArticleManager();
            var t = new art_news();
            try
            {
                t = art.GetItem(id);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                text = t.text
            });
        }

        //[Authorize(Roles = "admin")] - Данный атрибут отключён из-за некорректной работы uploadify в opere
        public ActionResult ArticleImageSave()
        {
            var context = HttpContext;
            var mesage1 = HttpContext.User.Identity.IsAuthenticated;
            var mesage2 = HttpContext.Request.IsAuthenticated;
            var mesage3 = Request.IsAjaxRequest();
            var art = new ArticleManager();
            var acim = new ArticleCatalogImagesManager();
            var res = false;
            int suggestionID = 0;
            try
            {
                string toSaveDirectory = context.Request.Params["toSaveDirectory"];
                suggestionID = RDL.Convert.StrToInt(context.Request.Params["suggestionID"], 0);
                var isPropImages = RDL.Convert.StrToInt(context.Request.Params["isPropImages"], 0);
                string propertyName = context.Request.Params["propertyName"];
                string propertyValue = context.Request.Params["propertyValue"] == null ? "" : context.Request.Params["propertyValue"];

                HttpPostedFileBase file = context.Request.Files["Filedata"];
                if (file != null && !string.IsNullOrEmpty(toSaveDirectory) && suggestionID > 0)
                {
                    string serverUrl = context.Server.MapPath("~" + toSaveDirectory + "/" + suggestionID + "/");//сохраняем рисунки с измен размерами

                    if (isPropImages > 0)
                    {
                        if (propertyValue == "")
                            serverUrl = context.Server.MapPath("~" + toSaveDirectory + "/" + suggestionID + "/property/" + propertyName + "/");
                        else
                            serverUrl = context.Server.MapPath("~" + toSaveDirectory + "/" + suggestionID + "/property/" + propertyName + "/" + propertyValue + "/");
                    }

                    if (Directory.Exists(serverUrl))
                    {
                        Files.DeleteDirectoryFiles(serverUrl);
                    }
                    else
                    {
                        Directory.CreateDirectory(serverUrl);
                    }

                    var fileGuid = Guid.NewGuid();          //
                    var fileGuidName = fileGuid.ToString() + Path.GetExtension(file.FileName);

                    //------------преобразование размера---------------------------------------------------------
                    Image bmp = Bitmap.FromStream(file.InputStream);
                    Image bmp_resized = Img.ResizeImage(bmp, new SizeF(1024, 1024));   //преобразовали к размеру 1024*...
                    bmp_resized.Save(serverUrl + fileGuidName);                       //сохранили в папке

                    Image bmp_resized_mini = Img.ResizeImage(bmp, new SizeF(300, 300));    //получаем миниатюру 300*...               
                    string fileNameMini = Path.GetFileNameWithoutExtension(fileGuidName) + "_mini";
                    fileNameMini = fileNameMini + Path.GetExtension(fileGuidName);
                    bmp_resized_mini.Save(serverUrl + fileNameMini);

                    // Сохраняем в базу путь к картинке
                    art.EditItemField(suggestionID, "imgPath", toSaveDirectory + "/" + suggestionID + "/" + fileNameMini);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public ActionResult ArticleImageDelete(int id)
        {
            var art = new ArticleManager();
            var mesage1 = HttpContext.User.Identity.IsAuthenticated;
            var mesage2 = HttpContext.Request.IsAuthenticated;
            var res = false;
            try
            {
                string serverUrl = HttpContext.Server.MapPath("~/uploads/Images/Articles/" + id.ToString() + "/");
                Files.DeleteDirectory(serverUrl);
                art.EditItemField(id, "imgPath", null);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }


        // Редактирование типов новостей
        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTypeTable()
        {
            //var art = new ArticleManager();
            //ViewBag.NewsType = art.GetItemsType();
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTypeTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var art = new ArticleManager();
            var acim = new ArticleCatalogImagesManager();

            var items = art.GetItemsType().AsQueryable();

            //#region filter
            //if (parameters.filter != null && parameters.filter.Count > 0)
            //{
            //    List<int?> td = new List<int?>();
            //    if (parameters.filter.ContainsKey("typeID"))
            //    {
            //        td = (parameters.filter["typeID"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
            //    }
            //    items = items.Where(x => (td.Count == 0 || td.Contains(x.typeID != null ? x.typeID : 0)));
            //}
            //#endregion

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
                case "code":
                    if (direction1 == "up") items = items.OrderBy(x => x.code);
                    else items = items.OrderByDescending(x => x.code);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.code);
                    else items = items.OrderByDescending(x => x.code);
                    break;
            }

            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();
            var imgUrl = new FileItemInfo1();

            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.name,
                    x.code,
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTypeTableInline(int pk, string value, string name)
        {
            var art = new ArticleManager();
            art.EditItemTypeField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTypeTable_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var art = new ArticleManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var code = AjaxModel.GetValueFromSaveField("code", fields);

                var item = new art_newsType { id = id, name = name, code = code };
                art.SaveItemType(item);
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

        [Authorize(Roles = "admin")]
        public ActionResult GetArticleTypeTable_remove(int id)
        {
            var res = false;
            var art = new ArticleManager();
            var item = art.GetItemType(id);
            var msg = "";
            if (item != null)
            {
                art.DeleteItemType(id);
                msg = "Запись успешно удалена!";
                res = true;
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