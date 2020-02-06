using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.Models.GoogleSheets;

namespace arkAS.Controllers
{
    public class GoogleDocsController : Controller
    {

        public ActionResult GoogleSheets()
        { 
            return View();
        }

        public JsonResult GetTableData(string tableID, string sheetName)
        {
            GoogleSheets table = new GoogleSheets();
            GoogleDrive drive = new GoogleDrive();
            string startCell = "";
            string finishCell = "";
            string name = "";
            if (sheetName =="nullTable")
                name = table.GetTableSheets(tableID)[0];
            else 
                name = sheetName;

            var res = new {
                values = table.GetTable(tableID, name, startCell, finishCell),
                sheets = table.GetTableSheets(tableID),
                editable = drive.Editable(tableID)
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InlineEditTable(int pk, string value, string name, string tableID)
        {
            GoogleSheets table = new GoogleSheets();
            string sheetName = name + Convert.ToString(pk);
            var res = table.InlineEditTable(tableID, sheetName, value);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateTable(string tableID, string sheetName)
        {
            GoogleSheets table = new GoogleSheets();
            var res = table.CreateTable(tableID, sheetName);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}