using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Pptx;
using arkAS.BLL.Core;

namespace arkAS.Controllers
{
    public class PptxController : Controller
    {
        // GET: Pptx
        public ActionResult Index()
        {
            var mng = new Pptx();
            var sm = new SettingsManager();
            mng.SetFolder(sm.GetSetting("folder_presentation", ""));
            ViewBag.CountSlide = mng.CountSlide();
            return View();
        }

         [HttpGet]
        public JsonResult SetParameters(string[] arrayParameters, int numberSlide)
        {
            var mng = new Pptx();
            var sm = new SettingsManager();
            mng.SetFolder(sm.GetSetting("folder_presentation", ""));
            mng.SetPPTShapeText(arrayParameters, numberSlide);

            return new JsonResult()
            {
                Data = "jbsdlj",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
          

        }
        public JsonResult AddSlide(int Count)
        {
            var mng = new Pptx();
            var sm = new SettingsManager();
            mng.SetFolder(sm.GetSetting("folder_presentation", ""));
            bool newSlide = mng.CloneSlidePart(Count);

            return new JsonResult
            {
                Data = newSlide,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DelitSlide(int numSlide , int count)
        {
            var mng = new Pptx();
            var sm = new SettingsManager();
            mng.SetFolder(sm.GetSetting("folder_presentation", ""));
            bool newSlide = mng.DeleteSlide(numSlide,count);

            return new JsonResult
            {
                Data = newSlide,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}