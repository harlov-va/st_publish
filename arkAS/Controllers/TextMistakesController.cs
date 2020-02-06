using arkAS.BLL;
using arkAS.BLL.TextMistakes;
using arkAS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using Dapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace arkAS.Controllers
	{
	public class TextMistakesController : Controller
		{
		public ActionResult TextMistakesSave()
			{
			var res = false;
			var mesage = "";
			try
				{
				var parameters = AjaxModel.GetAjaxParameters(HttpContext);
				var comment = parameters["comment"].ToString();
				var selectText = parameters["selectText"].ToString();
				var url = parameters["url"].ToString();
				if(selectText != null && url != null)
					{
					var txt = new TextMistakesManager();
					var user = HttpContext.User.Identity.IsAuthenticated ?
						Membership.GetUser(HttpContext.User.Identity.Name) : null;
					var item = new as_textMistakes
					{
						id = 0,
						userID = user != null ? (Nullable<Guid>)user.ProviderUserKey : null,
						url = url,
						selectText = selectText,
						comment = comment,
						correct = false
					};
					txt.SaveItem(item);
					res = true;
					}
				}
			catch(Exception ex)
				{
				res = false;
				}

			return Json(new
			{
				result = res,
				msg = mesage
			}, JsonRequestBehavior.AllowGet);
			}

		#region Admin
		[Authorize(Roles = "admin")]
		public ActionResult GetTextMistakesTable()
			{
			var txt = new List<string>();
			txt.Add("да");
			txt.Add("нет");
			ViewBag.correctType = txt;

			var rep = new CoreRepository();
			var p = new DynamicParameters();
			p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

			var items = rep.GetSQLData<dynamic>("[GetTextMistakesTable]", p, CommandType.StoredProcedure);

			var res = new List<object>();
			try
				{
				foreach(var item in (items as List<dynamic>))
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
			catch(NullReferenceException e)
				{
				Console.WriteLine(e);
				}

			var users = new List<string>();
			var urls = new List<string>();
			var textMistakes = new List<string>();
			var textComments = new List<string>();
			if(res.Count != 0)
				{
				foreach(dynamic obj in res)
					{
					users.Add(obj.user);
					urls.Add(obj.url);
					textMistakes.Add(obj.selectText);
					textComments.Add(obj.comment);
					}
				}

			var resultUsers = users.Where(x => x != "").Distinct().ToList();
			var resultUrls = urls.Where(x => x != "").Distinct().ToList();
			var resultTextMistakes = textMistakes.Where(x => x != "").Distinct().ToList();
			var resultTextComments = textComments.Where(x => x != "").Distinct().ToList();
			ViewBag.Users = resultUsers;
			ViewBag.Urls = resultUrls;
			ViewBag.TextMistakes = resultTextMistakes;
			ViewBag.TextComments = resultTextComments;
			return View();
			}

		[Authorize(Roles = "admin")]
		public ActionResult GetTextMistakesTable_getItems()
			{
			var parameters = AjaxModel.GetParameters(HttpContext);

			string userName = "";
			string searchText = "";
			object correct=null;

			if(parameters.filter != null && parameters.filter.Count > 0)
				{
				userName = parameters.filter.ContainsKey("user") ? (parameters.filter["user"].ToString()!="0" ? parameters.filter["user"].ToString():"") : "";
				searchText = parameters.filter.ContainsKey("searchText") ? (parameters.filter["searchText"].ToString() != "0" ? parameters.filter["searchText"].ToString() : "") : "";
				correct = parameters.filter.ContainsKey("correct") ? parameters.filter["correct"] : null;
				}

			string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
			string sort1 = sorts.Length > 0 ? sorts[0] : "";
			string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
			string direction1 = directions.Length > 0 ? directions[0] : "";

			var rep = new CoreRepository();
			var p = new DynamicParameters();
			p.Add("UserName", userName);
			p.Add("SelectText", searchText);
			p.Add("Correct", correct);
			p.Add("SortField", sort1);
			p.Add("Directions", direction1);
			p.Add("page", parameters.page);
			p.Add("pageSize", parameters.pageSize);
			p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

			var items = rep.GetSQLData<dynamic>("[GetTextMistakesTable]", p, CommandType.StoredProcedure);

			var res = new List<object>();
			var usersList = new List<object>();
			try
				{
				foreach(var item in (items as List<dynamic>))
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
			catch(NullReferenceException e)
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
		public ActionResult GetTextMistakesTableInline(int pk, string value, string name)
			{
			var txt = new TextMistakesManager();
			txt.EditItemField(pk, name, value);
			return Json(new
			{
				result = true
			});
			}

		[Authorize(Roles = "admin")]
		public ActionResult GetTextMistakesTable_remove(int id)
			{
			var res = false;
			var txt = new TextMistakesManager();
			var item = txt.GetItem(id);
			var msg = "";
			if(item != null)
				{
				txt.DeleteItem(id);
				msg = "Запись успешно удалена!";
				res = true;
				}

			return Json(new
			{
				result = res,
				msg = msg
			});
			}
		#endregion
		}
	}