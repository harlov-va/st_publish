using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.Docs;
using arkAS.Models;
using Dapper;
using Newtonsoft.Json;

namespace arkAS.Controllers
{
    public class SqlCrudController : Controller
    {
        // GET: SqlCrud
        
        public ActionResult SqlCrud()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/Account/Login/?demoType=SqlCrud/SqlCrud");

            }
            
        }
        
        public ActionResult SqlCrudInline(int pk, string value, string name)
        {
            var mng = new SqlCrudManager();
            mng.EditRightField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }
        [Authorize]
        public ActionResult SqlCrud_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new SqlCrudManager();
            IQueryable<as_sqlGet> items = mng.GetSqlCruds().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var code = parameters.filter.ContainsKey("code") ? parameters.filter["code"].ToString() : "";
                List<string> roles = new List<string>();
                if (parameters.filter.ContainsKey("roles"))
                {
                    roles = (parameters.filter["roles"] as ArrayList).ToArray().Select(x => x.ToString()).ToList();
                }
                items = items.Where(x =>
                    (roles.Count == 0 || x.as_sqlRole.Select(z => z.role).ToList().Intersect(roles).Count() > 0)
                );
                if (code != "")
                {
                    items = items.ToList().Where(x => x.code != null && x.code.IndexOf(code, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.code != null && x.code.IndexOf(code, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
                }
            }

            var res = items;


            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "code":
                    if (direction1 == "up") res = res.OrderBy(x => x.code);
                    else res = res.OrderByDescending(x => x.code);
                    break;

                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.id);
                    else res = res.OrderByDescending(x => x.id);
                    break;
            }
            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    sql = x.sql?? "",
                    code = x.code ?? "",
                    users=x.users??"",
                    roles = "<div class='usUserRoles'>" + String.Join(",", x.as_sqlRole.Select(z => z.role)) + "</div>"
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeRoleForSql(int sqlID, string role, bool turnOn)
        {
            var res = false;
            var mng = new SqlCrudManager();
            try
            {
                var item = mng.GetSqlCrud(sqlID);

                var rolesSql = mng.GetSqlByRole(role);
                if (turnOn)
                {
                    if (rolesSql.FirstOrDefault(x => x.id == sqlID) == null)
                    {
                        mng.SaveSqlRole(new as_sqlRole { id = 0, sqlID = sqlID, role = role });
                    }
                }
                else
                {
                    mng.DeleteSqlRole(sqlID, role);

                }
                res = true;

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult GetSql()
        {
            return View();
        }
        [Authorize]
        public ActionResult CreateSql(string sql,string code)
        {
            var res = false;
            var msg = "";
            var mng = new SqlCrudManager();
            var item = mng.GetSqlGet(code);
            try
            {

                if (item != null)
                {
                    msg = "Sql с таким кодом уже существует в системе";
                }
                else
                {
                    var users = Membership.GetUser().UserName;
                    item = new as_sqlGet{ id = 0, code = code, sql= sql,users = users};
                    mng.SaveSql(item);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {
                result = res,
                msg = msg,
                rightID = item.id
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult SqlCrud_remove(int id)
        {
            var res = false;
            var mng = new SqlCrudManager();

            var item = mng.GetSql(id);
            if (item != null)
            {
                mng.DeleteRolesBySqlID(id);
                mng.DeleteSqlCrud(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Sql удален!"
            });
        }
    }
}