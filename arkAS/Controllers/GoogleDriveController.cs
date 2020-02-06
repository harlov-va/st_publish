using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Google;
using arkAS.Helpers;

namespace arkAS.Controllers
{
    public class GoogleDriveController : Controller
    {
        // GET: GoogleDrive
        public ActionResult Index()
        {

            GoogleManager mng = new GoogleManager();
            mng.Authentications();
            List<string> title = mng.GetFiles();
            ViewBag.name = title;
            return View();
        }
        public JsonResult GetContent(string Name)
        {
            GoogleManager mng = new GoogleManager();
            mng.Authentications();
            string[] res = mng.DownloadContentFile(Name);
            return Json(res, JsonRequestBehavior.AllowGet);
       }

        public JsonResult CreateDoc(MItemtModel jsonObject)
        {
            string text = jsonObject.theHtml;
            string title = jsonObject.title; 
            GoogleManager mng = new GoogleManager();
            mng.Authentications();
            bool res =  mng.CreateDocument(title, text);
            return Json(res, JsonRequestBehavior.AllowGet);    
        }

        public JsonResult EditDoc(MitemEditModel json)
        {
            string content = json.theHtml;
            string idFile = json.idFile;
            GoogleManager mng = new GoogleManager();
            mng.Authentications();
            bool res = mng.UpdataDocument(idFile, content);
            return Json(res, JsonRequestBehavior.AllowGet); 
        }
        public JsonResult DeletFiles(string name)
        {
            GoogleManager mng = new GoogleManager();
            mng.Authentications();
            bool res = mng.DeleteFiles(name);
            return Json(res, JsonRequestBehavior.AllowGet); 
        }

    }
}