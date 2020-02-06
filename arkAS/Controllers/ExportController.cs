using arkAS.BLL.Core;
using arkAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class ExportController : Controller
    {
        public ActionResult ExportTable()
        {
            var res = false;
            var url = "";
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var type = parameters["type"].ToString();
            var title = parameters["title"].ToString();
            var subtitle = parameters["subtitle"].ToString();
            var table = parameters["table"];

            var mng = new ExportManager();
            switch (type)
            {
                case "excel":
                    url = mng.ExportTableToExcel(title, subtitle, table);
                    break;
                case "pdf":
                    url = mng.ExportTableToPDF(title, subtitle, table);
                    break;

            }
            res = url != "";

            return Json(new
            {
                result = res,
                url = url
            });
        }
    }
}