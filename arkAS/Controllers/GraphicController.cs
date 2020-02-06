using arkAS.BLL.Core;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using System.Data.SqlClient;
using System.Configuration;

namespace arkAS.Controllers
{
    public class GraphicController : Controller
    {
        public ActionResult Graphic()
        {
            return View();
        }


        public ActionResult Statistics()
        {
            return View();
        }

        public ActionResult StatisticsByWeeks()
        {
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            int year = DateTime.Now.Year;
            DateTime date = DateTime.Now;
            int week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var curtMonth = new DateTime(DateTime.Now.Year, 01, 01);
            p.Add("weeks", week);
            p.Add("curMonth", year);
            var items = rep.GetSQLData<dynamic>("[GetMailByWeeks]", p, CommandType.StoredProcedure);

            return Json(new
            {
                item = items,
                day = DateTime.Now.ToString("yyyy/MM/dd"),
                prevDay = curtMonth.ToString("yyyy/MM/dd"),
                nextDay = DateTime.Now.ToString("yyyy/MM/dd")
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GraphFunnel()
        {
            
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            List<dynamic> items = null;
            using (var conection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
            {
                conection.Open();
                items = conection.Query<dynamic>("[GetSite]", p, commandType: CommandType.StoredProcedure) as List<dynamic>;
            }
            return Json(new
            {
                item = items.Select(x => new { 
                    x.visits,
                    x.downloads,
                    x.querys,
                    x.cheques,
                    x.pays
                }).FirstOrDefault(),
            }, JsonRequestBehavior.AllowGet);
        }

        

    }

}


