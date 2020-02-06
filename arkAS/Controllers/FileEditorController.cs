using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.Models;
using System.IO;
using System.Web.Security;
 

namespace arkAS.Controllers
{


    public class FileEditorController : Controller
    {
        public static string PatternFolder = "/uploads/patternFolder/";
        public static string GetRootURL(string id)
        {

            return PatternFolder + id + "/";
        }
        public ActionResult Example()
        {
            return View();
        }
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
        private static dynamic GetDirsList(string path)
        {
            return Directory.GetDirectories(path).Select(d => new { name = Path.GetFileName(d), dirs = GetDirsList(d) }).ToList();
        }
        public ActionResult loadFileEditor()
        {
            string id = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);

            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";

            var url = GetRootURL(id);
            var path = HttpContext.Server.MapPath(url);//HttpContext.Current.Server.MapPath(url);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var dirs = Directory.GetDirectories(path).ToList();
            //if (dirs.Count == 0)
            //{ // загрузка дефолтового содержания для папки картинок
            //    try
            //    {

            //        CopyFilesRecursively(new DirectoryInfo(HttpContext.Server.MapPath(PatternFolder)), new DirectoryInfo(path));
            //        dirs = Directory.GetDirectories(path).ToList();
            //    }
            //    catch (Exception ex) { }
            //}
            //dirs.Insert(0, "");


            return Json(new
            {
                dirs = dirs.Select(x => new { name = Path.GetFileName(x), dirs = GetDirsList(x) })
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadDir()
        {

            string dir = "";
            string id = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            var url = GetRootURL(id);
            var path = HttpContext.Server.MapPath(url) + dir;
            var files = Directory.GetFiles(path);
            var res = new List<dynamic>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                int width = 0, height = 0;
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                {
                    var img = System.Drawing.Image.FromFile(file);
                    width = img.Width;
                    height = img.Height;
                }

                res.Add(new
                {
                    name = Path.GetFileName(file),
                    url = Path.Combine(url, dir, Path.GetFileName(file) ?? string.Empty),
                    width,
                    height,
                    ext
                });
            }

            return Json(new
            {
                files = res
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult createDir()
        {
            string dir = "";
            string id = "";
            string currentDir = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            currentDir = parameters.Where(x => x.Key == "currDir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "currDir").FirstOrDefault().Value.ToString() : "";
           
            var url = GetRootURL(id);
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
            string id = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            var res = false;
            try
            {
                var url = GetRootURL(id);
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
        public ActionResult deleteFile()
        {
            string dir = "";
            string id = "";
            string file = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            file = parameters.Where(x => x.Key == "file").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "file").FirstOrDefault().Value.ToString() : "";

            var res = false;
            string path = string.Empty;
            try
            {
                var url = GetRootURL(id);
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
        public ActionResult renameFile()
        {
            string dir = "";
            string id = "";
            string file = "";
            string newName = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            file = parameters.Where(x => x.Key == "file").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "file").FirstOrDefault().Value.ToString() : "";
            newName = parameters.Where(x => x.Key == "newName").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "newName").FirstOrDefault().Value.ToString() : "";

            try
            {
                var url = GetRootURL(id);
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
        public ActionResult renameDir()
        {
            string dir = "";
            string id = "";
            string newName = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            dir = parameters.Where(x => x.Key == "dir").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "dir").FirstOrDefault().Value.ToString() : "";
            id = parameters.Where(x => x.Key == "id").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "id").FirstOrDefault().Value.ToString() : "";
            newName = parameters.Where(x => x.Key == "newName").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "newName").FirstOrDefault().Value.ToString() : "";

            try
            {
                var url = GetRootURL(id);
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
        public ActionResult getTextFile()
        {
            string url = "";
            var text = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            url = parameters.Where(x => x.Key == "url").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "url").FirstOrDefault().Value.ToString() : "";

            if (!url.StartsWith("/uploads/"))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            //var us = Membership.GetUser(HttpContext.User.Identity.Name);
            //if (url.IndexOf(us.ProviderUserKey.ToString()) < 0)
            //{
            //    return Json("", JsonRequestBehavior.AllowGet);
            //}
            var path = HttpContext.Server.MapPath(url);
            text = System.IO.File.ReadAllText(path);

            return Json(new
            {
                text = text
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult saveTextFile()
        {
            string url = "";
            var text = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            url = parameters.Where(x => x.Key == "url").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "url").FirstOrDefault().Value.ToString() : "";
            text = parameters.Where(x => x.Key == "text").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "text").FirstOrDefault().Value.ToString() : "";
            var us = Membership.GetUser(HttpContext.User.Identity.Name);
            //if (!url.StartsWith("/uploads/") || HttpContext.User.Identity.Name == "" || url.IndexOf(us.ProviderUserKey.ToString()) < 0)
            if (!url.StartsWith("/uploads/"))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            var path = HttpContext.Server.MapPath(url);
            using (StreamWriter outfile = new StreamWriter(path))
            {
                outfile.Write(text);
            }
            return Json(new
            {
                text = text
            }, JsonRequestBehavior.AllowGet);

        }
    }
}