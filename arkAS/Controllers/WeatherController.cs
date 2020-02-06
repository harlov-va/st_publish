using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Weather;

namespace arkAS.Controllers
{
    public class WeatherController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetWeatherByCity(string city)
        {
            var res = Weather.GetWeatherByCity(city);
            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeatherByLocation(string lon, string lat)
        {
            var res = Weather.GetWeatherByLocation(lon.ToString(), lat.ToString());
            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }
	}
}