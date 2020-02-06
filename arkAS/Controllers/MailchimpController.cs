using arkAS.BLL.Mailchimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class MailchimpController : Controller
    {
        [HttpGet]
        public JsonResult GetAllLists()
        {
            var res = Mailchimp.GetAllLists();
            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }
       
    }
}