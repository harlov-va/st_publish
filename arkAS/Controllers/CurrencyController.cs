using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Currency;

namespace arkAS.Controllers
{
    public class CurrencyController : Controller
    {
        [HttpGet]
        public ActionResult GetCurrency()
        {
            var res = Currency.GetCurrency();
            return this.Content(res, "text/xml");
        }

        [HttpGet]
        public ActionResult GetDynamic(string code)
        {
            var res = Currency.GetDynamic(code);
            return this.Content(res, "text/xml");
        }
	}
}