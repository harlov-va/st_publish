using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.Models.SuperJob;

namespace arkAS.Controllers
{
    public class SuperJobController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetVacancies( string count,
                                        string page,
                                        string keyword,
                                        string town,
                                        string payment_from,
                                        string payment_to,
                                        string type_of_work,
                                        string gender,
                                        string experience
                                      )
        {
            SuperJob sj = new SuperJob();
            SuperJobInput data = new SuperJobInput();
            data.count = count;
            data.page = page;
            data.keyword = keyword;
            data.town = town;
            data.payment_from = payment_from;
            data.payment_to = payment_to;
            data.type_of_work = type_of_work;
            data.gender = gender;
            data.experience = experience;
            var res = SuperJob.GetFullInfo(data);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}