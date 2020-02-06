using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Imp;
using arkAS.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace arkAS.Controllers
{
    [Authorize(Roles = "admin")]
    public class ExcelController : Controller
    {
        private TablesDataBase tablesDataBase = new TablesDataBase();

        // GET: Excel
        public ActionResult Export()
        {
            ViewBag.Tables = tablesDataBase.GetTablesDataBaseList();
            return View();
        }

        public ActionResult GetCountRows(int code)
        {
            try
            {
                var tableDataBase = tablesDataBase.GetTableDataBaseByCode(code);

                //делаем запрос к БД, возможно вынести в репозиторий
                string query = @"select count(*) from dbo." + tableDataBase.db_name;
                int count = 0;
                using (
                    var connect =
                        new SqlConnection(
                            ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    connect.Open();
                    var command = new SqlCommand(query, connect);
                    count = Convert.ToInt32(command.ExecuteScalar());
                    connect.Close();
                }

                return Json(new {result = true, count = count});
            }
            catch (Exception exc)
            {
                return Json(new { result = false, msg = exc.Message });
            }
            
        }


        public ActionResult ExportData(int codeTable, int fromRows, int toRows, int allRows, string where, string fileName)
        {
            string query = "";
            var columnNames = new List<string>();
            string whereCond = String.IsNullOrEmpty(where) ? "" : "where " + where;

            var stopwatch = new Stopwatch();
            int seconds = 0;

            var url = "/uploads/crud/excel/";
            var path = HttpContext.Server.MapPath(String.Format(url));

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var tableDataBase = tablesDataBase.GetTableDataBaseByCode(codeTable);

            try
            { 
                if (allRows == 1)
                {
                    query = @"select * from dbo." + tableDataBase.db_name + " " + whereCond;
                }
                else
                {
                    var skip = (fromRows - 1) == -1 ? 0 : fromRows - 1;
                    var countRaws = toRows - skip;
                    query = @"select * from dbo." + tableDataBase.db_name + " " + whereCond + " ORDER BY id OFFSET " + skip + 
                        " ROWS FETCH NEXT " + countRaws + " ROWS ONLY";
                }

                //запускаем счетчик
                stopwatch.Start();

                using (var connect =
                        new SqlConnection(
                            ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    connect.Open();
                    var command = new SqlCommand(query, connect);
                    var sqlDataReader = command.ExecuteReader();

                    //создаем лист excel
                    var sheetName = tableDataBase.view_name + DateTime.Now.ToString("dd.MM.yyyy");
                    var workBook = new XLWorkbook();
                    var workSheet = workBook.Worksheets.Add(sheetName);

                    //шапка с названием колонок таблицы
                    for (var i = 1; i < sqlDataReader.FieldCount+1; i++)
                    {
                        workSheet.Cell(1, i).Value = sqlDataReader.GetName(i-1);
                    }

                    //заполняем данные
                    if (sqlDataReader.HasRows)
                    {
                        var row = 2;
                        while (sqlDataReader.Read())
                        {
                            for (var i = 1; i < sqlDataReader.FieldCount+1; i++)
                            {
                                workSheet.Cell(row, i).Value = sqlDataReader.GetValue(i-1);
                                string a = "field = " + sqlDataReader.GetName(i-1) + " value= " + sqlDataReader.GetValue(i-1);
                                columnNames.Add(a);
                            }
                            row++;
                        }
                    }

                    workBook.SaveAs(String.Format("{0}{1}.xlsx", path, fileName));
                    connect.Close();
                    stopwatch.Stop();
                    seconds = stopwatch.Elapsed.Seconds;    
                }
                return Json(RecordItemLog(tableDataBase.db_name, where, "", seconds, path, fileName));
            }
            catch (Exception exc)
            {
                return Json(RecordItemLog(tableDataBase.db_name, where, exc.Message, seconds,"",""));
            }
        }


        private object RecordItemLog(
            string table_name, 
            string where, 
            string error, 
            int duration,
            string path,
            string fileName)
        {
            var user = User.Identity.Name;
            var date = DateTime.Today;
            var info = where;
            var durationSec = duration;
            var errors = error;

            var imp = new ImpManager();

            try
            {
                if (!imp.GetItems().Any(i => i.name == "ExportIn" + table_name))
                {
                    var item = new imp_items
                    {
                        code = "exportIn" + table_name,
                        datatable = table_name,
                        name = "ExportIn" + table_name
                    };
                    imp.SaveItem(item);
                }

                var itemID = imp.GetItems().FirstOrDefault(i => i.name == "ExportIn" + table_name).id;
                var itemLog = new imp_itemLog
                {
                    itemID = itemID,
                    created = date,
                    createdBy = user,
                    durationSec = durationSec,
                    errors = errors,
                    info = info,
                    isImport = false,
                    withBackup = false
                };
                imp.SaveItemLog(itemLog);


                return new { result = true, msg = error, path = path, fileName = fileName + ".xlsx" };
            }
            catch (Exception exc)
            {
                return new { result = false, msg = exc.Message};
            }
        }

        public FileResult DownloadFile(string path, string fileName)
        {
            string contentType = "application/ms-excel";
            return File(path + fileName, contentType, fileName);
        }
    }
}