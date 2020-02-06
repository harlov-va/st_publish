using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Vacancy;
using System.Web.Script.Serialization;

namespace arkAS.Controllers
{
    public class VacancyController : Controller
    {
        // GET: Vacancy
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetVacancyParams(string keywords, string region, string levelSolary, string experience, string currency)
        {
            string res = Vacancy.GetVacancyParams(keywords,region,levelSolary,experience,currency);

            return new JsonResult()
            {
                Data = res,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetVacancyParamsKeywords(string keywords)
        {
            string res = Vacancy.GetVacancyParamsKeywords(keywords);

            return new JsonResult()
            {
                Data = res,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetSity()
        {
            string res = Vacancy.GetSity();

            return new JsonResult()
            {
                Data = res,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}