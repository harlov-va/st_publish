using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("DocumentsList", "Documents", new { area = "harlov" });
            //return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Estimate()
        {
            var estT = new List<EstimateTypes>();
            estT.Add(new EstimateTypes { id = 1, type = "бесплатная" });
            estT.Add(new EstimateTypes { id = 2, type = "платная" });
            ViewBag.estimateType = estT;
            return View();
        }

        public ActionResult Estimate_get(string id)
        {
            return Json(new
            {
                name = "",
                email = "",
                skype = "",
                estimate = 0
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Estimate_save()
        {
            var res = false;
            var msg = "";
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                var mngCRM = new CRMManager();
                var mngCore = new CoreManager();
                var email = "ru@rudensoft.ru"; //ru@rudensoft.ru

                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var clName = AjaxModel.GetValueFromSaveField("name", fields);
                var clEmail = AjaxModel.GetValueFromSaveField("email", fields);
                var clStype = AjaxModel.GetValueFromSaveField("skype", fields);
                var clEstimate = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("estimate", fields), 1);
   
                var subject = "Хочу сделать оценку своего проекта, " + clName;
                var body = "<p>Клиент: </p>" +
                    "<p>" + clName + ", e-mail: " + clEmail + ", skype: " + clStype + "</p>" +
                    "<p> Хочет сделать " + (clEstimate == 2 ? "<strong>платную</strong>" : "<strong>бесплатную</strong>") + 
                    " оценку своего проекта</p>";
                
                try
                {
                    mngCore.SendEmail(email, subject, body);
                    msg = "Заказ  успешно поступил в обработку";
                    res = true;
                }
                catch (Exception e)
                {
                    res = false;
                    msg = "Произошла ошибка во время отправки письма";
                    RDL.Debug.LogError(e, "SendMailToFriend");
                }
            }
            catch (Exception ex)
            {
                res = false;
                msg = ex.Message;
            }
            return Json(new
            {
                result = res,
                msg = msg
            }, JsonRequestBehavior.AllowGet);
        }
    }
}