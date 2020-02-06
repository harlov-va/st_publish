using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using arkAS.Handlers;

namespace arkAS.Controllers
{
    public class ArkStuffController : Controller
    {
        // GET: ArkStuff

      
       public ArkStuffController()
        {

           
        }

    
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Info(int itemID, string code)
        {
            var res = false;
            var content = "";
            var title = "";
            try
            {
                content = "Описание чего либо (popover-itemID:" + itemID + ", popover-code:" + code + ")";
                title = "Заголовок" + itemID;

                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {
                result = res,
                title,
                content,
            }, JsonRequestBehavior.AllowGet);
        }
        #region as-image-canEdit

        public static string PatternFolder = "/uploads/Images/texts/";
        private static dynamic GetDirsList(string path)
        {
            return Directory.GetDirectories(path).Select(d => new { name = Path.GetFileName(d), dirs = GetDirsList(d) }).ToList();
        }
        public static string GetRootURL(string id)
        {

            return PatternFolder + id + "/";
        }
        public ActionResult loadFileEditor()
        {
            feUploadFilesHandler.PatternFolder = PatternFolder;
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);

            var url = PatternFolder;
            var path = HttpContext.Server.MapPath(url);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var dirs = Directory.GetDirectories(path).ToList();

            string json = "";
            json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                dirs = dirs.Select(x => new { name = Path.GetFileName(x), dirs = GetDirsList(x) })
            });
            return Content(json, "application/json");

        }

        public ActionResult loadDir()
        {

            feUploadFilesHandler.PatternFolder = PatternFolder;
            string[] extensions = { ".jpg", ".png", ".jpeg", ".gif" };
            string dir = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            var res = new List<dynamic>();
            if (dir != "undefined") 
            {
                var url = PatternFolder;
                var path = HttpContext.Server.MapPath(url) + dir;
                var files = Directory.GetFiles(path).Where(f => extensions.Contains(System.IO.Path.GetExtension(f).ToLower()));


                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file).ToLower();
                    int width = 0, height = 0;
                    var img = System.Drawing.Image.FromFile(file);
                    width = img.Width;
                    height = img.Height;

                    res.Add(new
                    {
                        name = Path.GetFileName(file),
                        url = Path.Combine(PatternFolder, dir, Path.GetFileName(file) ?? string.Empty),
                        width,
                        height,
                        ext
                    });



                }
            }

            string json = "";
            json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                files = res
            });
            return Content(json, "application/json");
        }
        public ActionResult ArkImgExist(string codeDiv)
        {
            string STATUS = "BAD";
            TextManager obj = new TextManager();
            as_texts GetCodeIs = obj.GetCodeIs(codeDiv);
            if (GetCodeIs != null)
            {
                STATUS = "ItExists";
            }
            else
            {
                STATUS = "NotExist";
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                STATUS = STATUS
            });
            return Content(json, "application/json");
        }
        public ActionResult ArkImgSave(string ImgText, string codeDiv, string NameDiv)
        {
            string STATUS = "BAD";
            TextManager obj = new TextManager();

            as_texts at = new as_texts { id = 0, name = NameDiv, code = codeDiv, text = ImgText, categoryID = null };

            if (obj.CodeIs(at) >= 0)
            {
                STATUS = "OK";
            }
            string json = "";
            json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                STATUS = STATUS
            });
            return Content(json, "application/json");
        }
        public ActionResult createDir()
        {
            string dir = "";
            string id = "";
            string currentDir = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            //  id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            currentDir = parameters.Where(x => x.Key == "currDir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "currDir").FirstOrDefault().Value.ToString() : "";

            var url = PatternFolder;// GetRootURL(id);
            var path = HttpContext.Server.MapPath(url) + dir;
            Directory.CreateDirectory(path);

            return Json(new
            {
                dir = dir
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult deleteDir()
        {
            string dir = "";
            //string id = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            // id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            var res = false;
            try
            {
                var url = PatternFolder;// GetRootURL(id);
                var path = HttpContext.Server.MapPath(url) + dir;
                RDL.Files.DeleteDirectory(path);
                res = true;
            }
            catch (Exception)
            {
            }

            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult renameDir()
        {
            string dir = "";
            // string id = "";
            string newName = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            //id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            newName = parameters.Where(x => x.Key == "newName").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "newName").FirstOrDefault().Value.ToString() : "";

            try
            {
                var url = PatternFolder;// GetRootURL(id);
                var path = HttpContext.Server.MapPath(url) + dir;
                var dirName = Path.GetFileName(path);
                int ind = path.LastIndexOf(dirName, StringComparison.OrdinalIgnoreCase);
                var newPath = path.Remove(ind, dirName.Length).Insert(ind, newName);
                Directory.Move(path, newPath);
                return Json(new
                {
                    result = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult renameFile()
        {
            string dir = "";
            //string id = "";
            string file = "";
            string newName = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            file = parameters.Where(x => x.Key == "file").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "file").FirstOrDefault().Value.ToString() : "";
            newName = parameters.Where(x => x.Key == "newName").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "newName").FirstOrDefault().Value.ToString() : "";

            try
            {
                var url = PatternFolder;// GetRootURL(id);
                var path = HttpContext.Server.MapPath(url) + dir + "\\" + file;
                var newPath = HttpContext.Server.MapPath(url) + dir + "\\" + newName;
                System.IO.File.Move(path, newPath);
                return Json(new
                {
                    result = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult deleteFile()
        {
            string dir = "";
            string file = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            file = parameters.Where(x => x.Key == "file").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "file").FirstOrDefault().Value.ToString() : "";

            var res = false;
            string path = string.Empty;
            try
            {
                var url = PatternFolder;
                path = HttpContext.Server.MapPath(url) + dir + "\\" + file;

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                res = true;
            }
            catch (Exception)
            {
                /*if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        Libs.DetectOpenFiles.CloseHandle(path);
                        File.Delete(path);
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        Tracer.Error("File delete error", ex.ToString(), string.Empty); 
                    }
                }*/
            }

            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public ActionResult GetTexts(string[] codes)
        {   
            var mng = new TextManager();
            var items = new List<as_texts>();
            if (codes != null) items = mng.GetTexts(codes);

            var mng2 = new RightsManager();
            var canEdit = mng2.CheckRightForUser(User.Identity.Name, "canEditInlineText");
            return Json(new
            {
                result = true,
                canEdit = canEdit,
                items = items.Select(x => new { x.code, x.text })
            });
        }

        public ActionResult SaveText(string code, string text)
        {   
            var mng = new TextManager();
            var mng2 = new RightsManager();
            var canEdit = mng2.CheckRightForUser(User.Identity.Name, "canEditInlineText");

            var msg = "";
            var res = false;
            if (canEdit)
            {
                var item = mng.GetText(code);
                if (item == null)
                {
                    item = new as_texts { categoryID=null, code = code, id = 0, name = code, text = text };
                }
                else {
                    item.text = text;
                }                
                mng.SaveText(item);
                res = true;
            }
            else {
                msg = "У вас нет прав на редактирование этого текста. Обратитесь к администрации сайта";
            }
            
            return Json(new
            {
                result = res,
                msg = msg             
            });
        }


        public ActionResult simpleForm_save()
        {
            var res = false;
            var msg = "Спасибо! Ваша заявка принята в обработку. Мы свяжемся с Вами в ближайшее время.";
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);

                var code = AjaxModel.GetAjaxParameter("code", parameters);
                var name = AjaxModel.GetAjaxParameter("name", parameters);
                var phone = AjaxModel.GetAjaxParameter("phone", parameters);
                var email = AjaxModel.GetAjaxParameter("email", parameters);
                var text = AjaxModel.GetAjaxParameter("text", parameters);
                var mng = new CoreManager();
                    
                if (code != "example") {
                    var mng2 = new SettingsManager();

                    var body = String.Format("Форма: {0}<br /><br />Имя: {1}<br /><br />Телефон: {2}<br /><br />Email: {3}<br /><br />Комментарий: {4}<br /><br />", code, name, phone, email, text);
                    mng.SendEmail(mng2.GetSetting("mainEmail", ""), "Новая заявка-лид на Ark AS", body, "", "", "wa-check@yandex.ru");
                }

                if (code == "book") { 
                    // send ouк book to user
                    mng.SendEmail(email, 
                        "Ваша книга по созданию CRM",
                        string.Format("Добрый день, {0}!<br /><br />Меня зовут Раянов Руслан. Я автор книги, которую вы запросили с нашего сайта. Постарайтесь при прочтении по максимуму проработать практические упражнения для получения максимального эффекта от книги.  <br /><br />  Во вложении вы найдете нашу книгу (формат PDF) по созданию CRM в виде веб-приложения. <br /><br />Если будут какие-либо вопросы по материалу - пожалуйста пишите на мой ящик ru@rudensoft.ru. <br /><br /> C уважением, Раянов Руслан. <br /><br /> П.С. Не отвечайте на это письмо. Оно создано автоматически. Все ответы пожалуйста пишите на ru@rudensoft.ru", name),
                        HttpContext.Server.MapPath("~/Content/Books/rayanov-create-crm.pdf"), "", "wa-check@yandex.ru");
                    msg = "Книга отправлена Вам на почту. Приятного чтения!";
                }


                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {        
                result = res,
                msg =  res ? msg: "Во время выполнения операции произошла ошибка. Напишите пожалуйста свой запрос на почту ru@rudensoft.ru" 
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadStuffElements(string url)
        {
            var mng = new MenuManager();
            var mngMenuProfile = new MenuProfileManager();
            var mngCore = new CoreManager();

            var userGuid = mngCore.GetUserGuid();
            var items = mng.GetMenu();
            var itemsProfile = mngMenuProfile.GetProfileMenu(userGuid);

            
            return Json(new
            {                
                name = User.Identity.Name,
                menuItems = items.Where(x => x.parentID.GetValueOrDefault() == 0).Select(x => new
                {
                    x.name,
                    x.title,
                    x.url,
                    x.cssclass,
                    isActive = !String.IsNullOrEmpty(x.pattern) && Regex.IsMatch(url, x.pattern, RegexOptions.IgnoreCase),
                    items = items.Where(y => y.parentID.GetValueOrDefault() == x.id).Select(y => new {
                        y.name,
                        y.title,
                        y.url,
                        y.cssclass,
                        isActive = !String.IsNullOrEmpty(y.pattern) && Regex.IsMatch(url, y.pattern, RegexOptions.IgnoreCase),                  
                        items = items.Where(z => z.parentID.GetValueOrDefault() == y.id).Select(z => new {
                            z.name,
                            z.title,
                            z.url,
                            z.cssclass,
                            isActive = !String.IsNullOrEmpty(z.pattern) && Regex.IsMatch(url, z.pattern, RegexOptions.IgnoreCase),
                  
                        })
                    })
                }),
                menuProfileItems = itemsProfile.Select(x => new
                {
                    x.id,
                    x.name,
                    x.href,
                    x.cssClass
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditProfileMenu(List<as_profileMenu> editMenu)
        {
            var mngMenuProfile = new MenuProfileManager();
            var mngCore = new CoreManager();
            var userGuid = mngCore.GetUserGuid();
            var itemsProfile = mngMenuProfile.GetProfileMenu(userGuid);

            if (editMenu != null)
            {

                foreach (var item in itemsProfile)
                {
                    if (!editMenu.Any(x => x.id == item.id))
                    {
                        mngMenuProfile.DeleteProfileMenu(item.id);
                    }
                }

                for (int i = 0; i < editMenu.Count; i ++)
                {
                   if (editMenu[i].id > 0)
                   {
                       mngMenuProfile.EditProfileMenu(editMenu[i].id, editMenu[i].name, editMenu[i].href, editMenu[i].cssClass);
                   }
                   else
                   {
                       editMenu[i].id = 0;
                       editMenu[i].userGuid = userGuid;
                       mngMenuProfile.SaveProfileMenu(editMenu[i]);
                   }
                }
            }

            return Json(new
           {
               result = true
           }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult jsError(string s)
        {
            RDL.Debug.LogError(new Exception(s));            
            return Json(new
            {      
            }, JsonRequestBehavior.AllowGet);
        }
        

        
    }
}