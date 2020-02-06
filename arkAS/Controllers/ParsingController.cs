using arkAS.Models;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Threading;

namespace arkAS.Controllers
{
    public class ParsingController : Controller
    {
        // GET: parsing
        public ActionResult Index()
        {
            return View();
        }

        public static string CustomParsePage(int id, string urlFormat, List<ParseItem> items, string word)
        {
            var res = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                var url = String.Format(urlFormat, id);
                HtmlDocument doc = hw.Load(url);
                if (word == " ")
                {
                    res = "Нет элементов";
                    return res;
                }
                var element = doc.DocumentNode.SelectSingleNode("//a[contains(., '" + word + "')]");
                if (element == null)
                {
                    res = "Нет элементов";
                    return res;
                }

                var tag = element.InnerText;
                var count = element.ParentNode.ParentNode.SelectSingleNode("//p").InnerText;
                ParseItem pi = new ParseItem(tag, count);
                items.Add(pi);
                return res;
            }
            catch (Exception ex)
            {
                res = ex.Message;
                return res;
            }
        }

        public ActionResult ParsingGo()
        {
            var res = false;
            var msg = "";
            var url = "";
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var words = AjaxModel.GetValueFromSaveField("words", fields).ToLower().Trim().Replace(" ", string.Empty);
                var word = "";
                var i = 0;
                List<string> keys = new List<string>();
                while (i < words.Length)
                {
                    if (words[i] == '\n')
                    {
                        if (word != "")
                        {
                            keys.Add(word); 
                        }
                        word = "";
                    }
                    else
                    {
                        word += words[i];     
                    }
                    i++;
                };
                if (word != "") { keys.Add(word); }
                List<ParseItem> items = new List<ParseItem>();
                var b = keys;
                for (i = 0; i < keys.Count; i++)
                {
                    var a = CustomParsePage(i, "https://websta.me/search/" + keys[i], items, keys[i]);
                    Thread.Sleep(400);
                    if (a != "")
                    {
                        msg = a;
                        res = false;
                        return Json(new
                        {
                            result = res,
                            msg = msg
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (items.Count == 0)
                {
                    msg = "Нет элементов";
                    res = false;
                    return Json(new
                    {
                        result = res,
                        msg = msg
                    }, JsonRequestBehavior.AllowGet);
                }
                url = SaveFile(items);
                msg = "<a href='" + url + "'>Ссылка на excel-файл</a>";
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                msg = ex.Message;
            }
            var json = JsonConvert.SerializeObject(new
            {
                result = res,
                msg = msg,
                url = url
            });

            return Content(json, "application/json");
        }

        private void СleaningDirectory(string path)
        {
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string pathFile in files)
            {
                System.IO.File.Delete(pathFile);
            }
        }

        public string SaveFile(List<ParseItem> items)
        {
            //Для сохранения в txt.
            /*var res = "";
            var s = "";
            try
            {
                foreach (var item in items)
                {
                    s += item.Tag + "\t" + item.Count + "\n";
                }
                System.IO.File.WriteAllText("D:/Parsing.txt", s);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                return res;
            }
            return res;*/
            
            //Для сохранения в excel.
            var res = "";
            var cols = new Dictionary<string, string>();
            cols.Add("tag", "Тег");
            cols.Add("count", "Количество");

            var g = Guid.NewGuid();
            var url = "/uploads/excel/";
            var path = HttpContext.Server.MapPath(String.Format(url));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            СleaningDirectory(path);

            var workbook = new XLWorkbook();
            if (workbook.Worksheets.Count == 0)
            {
                workbook.Worksheets.Add("Парсинг");
            }
            var list = workbook.Worksheets.FirstOrDefault();

            IXLCell cell = null;

            var header = list.Range(1, 1, 1, cols.Count);
            header.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 102, 0);

            var allTable = list.Range(1, 1, Math.Min(items.Count + 1, 1001), cols.Count);
            allTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            allTable.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            allTable.Style.Alignment.SetWrapText();

            var currentCol = 1;
            foreach (var col in cols)
            {
                cell = list.Cell(1, currentCol); cell.Value = col.Value;
                currentCol++;
            }

            list.Column(1).Width = 16;
            list.Column(2).Width = 16;

            var i = 2;
            foreach (var x in items)
            {
                if (i > 1001) break;
                currentCol = 1;

                cell = list.Cell(i, currentCol); cell.Value = x.Tag;
                currentCol++;

                cell = list.Cell(i, currentCol); cell.Value = x.Count;
                currentCol++;

                i++;
            }

            workbook.SaveAs(String.Format("{0}{1}.xlsx", path, g));
            res = String.Format("{0}{1}.xlsx", url, g);

            return res;
        }

        public class ParseItem
        {
            public string Tag = "";
            public string Count = "";

            public ParseItem(string tag, string count)
            {
                Tag = tag;
                Count = count;
            }
        }
    }
}