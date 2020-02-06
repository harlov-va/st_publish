using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.SmallData;
using arkAS.BLL;
using arkAS.BLL.Core;

namespace arkAS.Controllers
{
    public class SmallDataController : Controller
    {
        // GET: SmallData
        public ActionResult Client()
        {
            SmallDataManager mng = new SmallDataManager();
            var res = mng.GetTable();
            return View(res);
        }

        public JsonResult EditTable(int productID)
        {
            SmallDataManager mng = new SmallDataManager();
            if (User.Identity.IsAuthenticated)
            {
                SmallDataTest res = new SmallDataTest();
                res.UserName = User.Identity.Name;
                res.productID = productID;
                res.visitedDate = DateTime.Now;
                mng.SaveTable(res);
            }
            var res2 = mng.GetTable().Select(x => new
            {
                productID = x.productID,
                UserName = x.UserName,
                d = x.visitedDate.Value.Day,
                m = x.visitedDate.Value.Month,
                y = x.visitedDate.Value.Year
            });

            return Json(res2, JsonRequestBehavior.AllowGet);
        }
    }
}