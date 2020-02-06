using arkAS.BLL.Core;
using arkAS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class MetricsController : Controller
    {
        [Authorize]
       public ActionResult GetTypes(string code)
       {
           var mng = new MetricsManager();
           var res = mng.GetTypes(code).Select(x=> new {x.id, x.name});

           return Json(new
           {
               result = true,
               items = res
           });
       }

        [Authorize]
        public ActionResult GetMetrics(int typeID)
        {
            var mng = new MetricsManager();
            var res = mng.GetMetrics(typeID).Select(x => new { x.id, x.title, x.subtitle });

            return Json(new
            {
                result = true,
                items = res
            });
        }

        [Authorize]
        public ActionResult GetMetric()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var metricID = RDL.Convert.StrToInt(parameters["metricID"].ToString(), 0);
            var row = parameters["row"] as System.Collections.ArrayList;
            var mng = new MetricsManager();
            var dt = new DataTable();
            var res = mng.GetMetric(metricID, row, out dt);

            return Json(new
            {
                result = true,                
                Table= new {
                    columns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName),
                    rows = dt.Rows.Cast<DataRow>().Select(x => new { fields = x.ItemArray })
                },
                Parameters = res.as_mt_metrics1.Select(x => new { x.title, x.parName, x.id }).ToList()               
            });
        }
        
    }
}
