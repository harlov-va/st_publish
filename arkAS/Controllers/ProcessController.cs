using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.Process;
using arkAS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace arkAS.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        public ActionResult Index()
        {
            var mng = new ProcessManager();

            ViewBag.processesNameSource = new JavaScriptSerializer().Serialize(mng.GetProcesses().Select(x => new { Value = x.id, Text = x.name, Selected = false }));
            ViewBag.processesItemStatusSource = new JavaScriptSerializer().Serialize(new List<object>() { new { Value = 1, Text = "Завершен", Selected = false }, new { Value = 0, Text = "Активен", Selected = false }, new { Value = -1, Text = "Не выбран", Selected = false } });

            ViewBag.processesNameSourceInLine = new JavaScriptSerializer().Serialize(mng.GetProcesses().Select(x => new { value = x.id, text = x.name }));
            ViewBag.processesItemStatusSourceInLine = new JavaScriptSerializer().Serialize(new List<object>() { new { value = 1, text = "Завершен" }, new { value = 0, text = "Активен" } });

            ViewBag.usersNameSource = new JavaScriptSerializer().Serialize(Membership.GetAllUsers().Cast<MembershipUser>().Select(x => new { Value = x.UserName, Text = x.UserName }));
            ViewBag.rolesNameSource = new JavaScriptSerializer().Serialize(System.Web.Security.Roles.GetAllRoles().AsQueryable().Select(x => new { Value = x, Text = x }));
            return View();
        }

        private bool SearchArrayPattern(string[] pattern, string[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (bytes[i] == pattern[j])
                        return true;
                }
            }
            return false;
        }
        
        public ActionResult ProcessItem_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new ProcessManager();
            var items = mng.GetProcessItems()
                .AsQueryable()
                .Skip(parameters.pageSize * (parameters.page - 1))
                .Take(parameters.pageSize);
            //
            //var items = mng.GetSQLClients("GetCRMClients").AsQueryable();

            var sortName = parameters.sort.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var sortDirection = parameters.direction.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (sortName.Length > 0 & sortDirection.Length > 0)
            {
                for (int i = 0; i < sortName.Length; i++)
                {
                    if (i > 0){
                        items = items.ThenByField(sortName[i], (sortDirection[0] == "up"));
                    }
                    else {
                        items = items.OrderByField(sortName[i], (sortDirection[0] == "up"));
                    }
                }
            }

            if (parameters.filter != null && parameters.filter.Count > 0)
            {

                if (parameters.filter.ContainsKey("name"))
                {
                    if(!string.IsNullOrWhiteSpace(parameters.filter["name"].ToString()))
                        items = items.Where(x => x.name.Contains(parameters.filter["name"].ToString()));
                }
                if (parameters.filter.ContainsKey("isFinish"))
                {
                    if(parameters.filter["isFinish"].ToString() != "-1")
                        items = items.Where(x => x.isFinish == Convert.ToBoolean(RDL.Convert.StrToInt(parameters.filter["isFinish"].ToString(), 0)));
                }
                if (parameters.filter.ContainsKey("roles"))
                {
                    string[] statusIDs;
                    statusIDs = (parameters.filter["roles"] as ArrayList).ToArray().Select(x => x.ToString()).ToArray();
                    if(statusIDs.Length != 0)
                        items = items.Where(x => SearchArrayPattern(statusIDs, x.roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
                }
                if (parameters.filter.ContainsKey("users"))
                {
                    string[] statusIDs;
                    statusIDs = (parameters.filter["users"] as ArrayList).ToArray().Select(x => x.ToString()).ToArray();
                    if (statusIDs.Length != 0)
                        items = items.Where(x => SearchArrayPattern(statusIDs, x.users.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
                }
                if (parameters.filter.ContainsKey("processID"))
                {
                    if(parameters.filter["processID"].ToString() != "0")
                        items = items.Where(x => x.proc_processes.id == RDL.Convert.StrToInt(parameters.filter["processID"].ToString(), 0));
                }

            }

            /* ==================================================
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";

                var sourceID = parameters.filter.ContainsKey("sourceName") ? RDL.Convert.StrToInt(parameters.filter["sourceName"].ToString(), 0) : 0;
                items = items.Where(x => (sourceID == 0 || x.sourceID == sourceID));

                List<int?> statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusIDs = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                items = items.Where(x =>
                    (statusIDs.Count == 0 || statusIDs.Contains(x.statusID))
                );


                var needActive = parameters.filter.ContainsKey("needActive") ? RDL.Convert.StrToInt(parameters.filter["needActive"].ToString(), -1) : -1;
                items = items.Where(x =>
                    //фильтр в этом месте не срабатывает для needActive=NULL
                    (needActive == -1 || x.needActive == (needActive == 1 ? true : false))
                );

                DateTime nextContactMin = DateTime.MinValue, nextContactMax = DateTime.MaxValue;
                if (parameters.filter.ContainsKey("nextContact") && parameters.filter["nextContact"] != null)
                {
                    var dates = parameters.filter["nextContact"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        nextContactMin = RDL.Convert.StrToDateTime(dates[0].Trim(), DateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        nextContactMax = RDL.Convert.StrToDateTime(dates[1].Trim(), DateTime.MaxValue).AddDays(1);
                    }
                    items = items.Where(x =>
                        (nextContactMin <= x.nextContact && x.nextContact <= nextContactMax));
                }

                if (text != "")
                {
                    items = items.ToList().Where(x =>
                        x.fio != null && x.fio.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.city != null && x.city.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.note != null && x.note.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.username != null && x.username.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.addedBy != null && x.addedBy.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                        ).AsQueryable();
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            IOrderedQueryable<crm_clients> orderedItems = items.OrderByDescending(p => p.created);

            switch (sort1)
            {
                case "fio":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.fio);
                    else orderedItems = items.OrderByDescending(x => x.fio);
                    break;
                case "city":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.city);
                    else orderedItems = items.OrderByDescending(x => x.city);
                    break;
                case "note":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.note);
                    else orderedItems = items.OrderByDescending(x => x.note);
                    break;
                case "addedBy":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.addedBy);
                    else orderedItems = items.OrderByDescending(x => x.addedBy);
                    break;
                case "statusName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.crm_clientStatuses.name);
                    else orderedItems = items.OrderByDescending(x => x.crm_clientStatuses.name);
                    break;
                case "sourceName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.crm_sources.name);
                    else orderedItems = items.OrderByDescending(x => x.crm_sources.name);
                    break;
                case "subchannel":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.subchannel);
                    else orderedItems = items.OrderByDescending(x => x.subchannel);
                    break;
                case "username":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.username);
                    else orderedItems = items.OrderByDescending(x => x.username);
                    break;
                case "needActive":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.needActive);
                    else orderedItems = items.OrderByDescending(x => x.needActive);
                    break;
                case "nextContact":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.nextContact);
                    else orderedItems = items.OrderByDescending(x => x.nextContact);
                    break;

                default:
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.created);
                    else orderedItems = items.OrderByDescending(x => x.created);
                    break;
            }

            if (sort2 != "")
            {
                switch (sort2)
                {
                    case "fio":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.fio);
                        else orderedItems = orderedItems.ThenByDescending(x => x.fio);
                        break;
                    case "city":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.city);
                        else orderedItems = orderedItems.ThenByDescending(x => x.city);
                        break;
                    case "note":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.note);
                        else orderedItems = orderedItems.ThenByDescending(x => x.note);
                        break;
                    case "addedBy":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.addedBy);
                        else orderedItems = orderedItems.ThenByDescending(x => x.addedBy);
                        break;
                    case "statusName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.crm_clientStatuses.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.crm_clientStatuses.name);
                        break;
                    case "sourceName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.crm_sources.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.crm_sources.name);
                        break;
                    case "subchannel":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.subchannel);
                        else orderedItems = orderedItems.ThenByDescending(x => x.subchannel);
                        break;
                    case "username":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.username);
                        else orderedItems = orderedItems.ThenByDescending(x => x.username);
                        break;
                    case "needActive":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.needActive);
                        else orderedItems = orderedItems.ThenByDescending(x => x.needActive);
                        break;
                    case "nextContact":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.nextContact);
                        else orderedItems = orderedItems.ThenByDescending(x => x.nextContact);
                        break;

                    default:
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.created);
                        else orderedItems = orderedItems.ThenByDescending(x => x.created);
                        break;
                }
            }

            var total = orderedItems.Count();
            var res2 = orderedItems.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();
            */

            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    isFinish = x.isFinish ? "Завершен" : "Активен",
                    roles = x.roles ?? "",
                    users = x.users ?? "",
                    processID = x.proc_processes != null ? x.proc_processes.name : ""
                }),
                total = items.Count<proc_processItems>()
                
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult CreateProcessItem(proc_processItems item)
        {
            var mng = new ProcessManager();

            item.isFinish = false;

            mng.SaveProcessItem(item);

            return Json(new
            {
                result = item.id > 0,
                itemID = item.id
            });
        }

        public ActionResult ProcessItemInline(int pk, string value, string name)
        {
            var mng = new ProcessManager();
            //mng.SaveProcessItem(item);
            mng.EditProcessItemField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult ProcessItem_remove(int id)
        {
            var res = false;
            var mng = new ProcessManager();
            var item = mng.GetProcessItem(id);
            var msg = "";
            if (item != null)
            {
                mng.DeleteProcessItem(id);
                msg = "Задача удалена!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }
        


        public ActionResult Processes()
        {
            var mng = new ProcessManager();
            
            return View();
        }

        public ActionResult Processes_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new ProcessManager();
            var items = mng.GetProcesses()
                .AsQueryable()
                .Skip(parameters.pageSize * (parameters.page - 1))
                .Take(parameters.pageSize);

            var sortName = parameters.sort.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var sortDirection = parameters.direction.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (sortName.Length > 0 & sortDirection.Length > 0)
            {
                for (int i = 0; i < sortName.Length; i++)
                {
                    if (i > 0)
                    {
                        items = items.ThenByField(sortName[i], (sortDirection[0] == "up"));
                    }
                    else {
                        items = items.OrderByField(sortName[i], (sortDirection[0] == "up"));
                    }
                }
            }
            
            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    desc = x.desc ?? "",
                    code = x.code ?? ""
                }),
                total = items.Count<proc_processes>()

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateProcesses(proc_processes item)
        {
            var mng = new ProcessManager();

            //item.isFinish = false;

            mng.SaveProcess(item);

            return Json(new
            {
                result = item.id > 0,
                itemID = item.id
            });
        }

        public ActionResult ProcessesInline(int pk, string value, string name)
        {
            var mng = new ProcessManager();
            //mng.SaveProcessItem(item);
            mng.EditProcessField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Processes_remove(int id)
        {
            var res = false;
            var mng = new ProcessManager();
            var item = mng.GetProcess(id);
            var msg = "";
            if (item != null)
            {
                mng.DeleteProcess(id);
                msg = "Процесс удален!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

    }

    public static class extensionmethods
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
        public static IQueryable<T> ThenByField<T>(this IQueryable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "ThenBy" : "ThenByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
    }
}