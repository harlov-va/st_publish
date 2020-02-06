using arkAS.BLL.Core;
using Dapper;
using System;
using arkAS.BLL;
using arkAS.BLL.TextMistakes;
using arkAS.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using arkAS.BLL.CRM;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace arkAS.Controllers
{
    public class LulchenkoTextMistakesController : Controller
    {
        private TextMistakesManager mistakesManager;

        public LulchenkoTextMistakesController()
        {
            mistakesManager = new TextMistakesManager();
        }


        public ActionResult AddMistake(string url, string selectText, string comment)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(selectText))
            {
                return Json(new { result = false, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var currentUser = HttpContext.User.Identity.IsAuthenticated ? Membership.GetUser(HttpContext.User.Identity.Name) : null;
                var newMistake = new as_textMistakes
                {
                    id = 0,
                    userID = currentUser != null ? (Nullable<Guid>)currentUser.ProviderUserKey : null,
                    url = url,
                    selectText = selectText,
                    comment = comment,
                    correct = false
                };
                mistakesManager.SaveItem(newMistake);
                return Json(new { result = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = "" }, JsonRequestBehavior.AllowGet);
            }
        }



        // GET: LulchenkoTextMistakes
        [Authorize(Roles ="admin")]
        public ActionResult GetTable()
        {
            var correctStatuses = new List<string>() { "да", "нет" };
            ViewBag.correctStatuses = correctStatuses;

            var rep = new CoreRepository();
            var parameters = new DynamicParameters();
            parameters.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("pageSize", 100);

            var items = rep.GetSQLData<dynamic>("[GetTextMistakesTable]", parameters, CommandType.StoredProcedure);

            var res = new List<object>();
            try
            {
                foreach (var item in (items as List<dynamic>))
                {
                    res.Add(new
                    {
                        item.id,
                        user = item.userName != null ? item.userName : "",
                        item.selectText,
                        item.url,
                        item.comment,
                        correct = item.correct ? "да" : "нет"
                    });
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }

            var users = new List<string>();
            var urls = new List<string>();
            var textMistakes = new List<string>();
            var textComments = new List<string>();
            if (res.Count != 0)
            {
                foreach (dynamic obj in res)
                {
                    users.Add(obj.user);
                    urls.Add(obj.url);
                    textMistakes.Add(obj.selectText);
                    textComments.Add(obj.comment);
                }
            }

            ViewBag.Users = users.Where(x => x != "").Distinct().ToList();
            ViewBag.Urls = urls.Where(x => x != "").Distinct().ToList();
            ViewBag.TextMistakes = textMistakes.Where(x => x != "").Distinct().ToList();
            ViewBag.TextComments = textComments.Where(x => x != "").Distinct().ToList();
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult EditInline(int pk, string value, string name)
        {
            mistakesManager.EditItemField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        [Authorize(Roles = "admin")]
        public ActionResult Table_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            string userName = "";
            string searchText = "";
            object correct = null;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                userName = parameters.filter.ContainsKey("user") ? (parameters.filter["user"].ToString() != "0" ? parameters.filter["user"].ToString() : "") : "";
                searchText = parameters.filter.ContainsKey("searchText") ? (parameters.filter["searchText"].ToString() != "0" ? parameters.filter["searchText"].ToString() : "") : "";
                correct = parameters.filter.ContainsKey("correct") ? parameters.filter["correct"] : null;
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var param = new DynamicParameters();
            param.Add("UserName", userName);
            param.Add("SelectText", searchText);
            param.Add("Correct", correct);
            param.Add("SortField", sort1);
            param.Add("Directions", direction1);
            param.Add("page", parameters.page);
            param.Add("pageSize", parameters.pageSize);
            param.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetTextMistakesTable]", param, CommandType.StoredProcedure);

            var res = new List<object>();
            var usersList = new List<object>();
            try
            {
                foreach (var item in (items as List<dynamic>))
                {
                    res.Add(new
                    {
                        item.id,
                        user = item.userName != null ? item.userName : "",
                        item.selectText,
                        item.url,
                        item.comment,
                        correct = item.correct ? "да" : "нет"
                    });
                    usersList.Add(new { user = item.userName != null ? item.userName : "" });
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
            var json = JsonConvert.SerializeObject(new
            {
                items = res,
                total = res.Count
            });

            return Content(json, "application/json");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Table_remove(int id)
        {
            var result = false;
            var item = mistakesManager.GetItem(id);
            var msg = "";
            if (item != null)
            {
                mistakesManager.DeleteItem(id);
                msg = "Запись успешно удалена!";
                result = true;
            }

            return Json(new
            {
                result = result,
                msg = msg
            });
        }
    }
}