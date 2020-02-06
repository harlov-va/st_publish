using System.IO;
using System.Web.Script.Serialization;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.BLL.Imp;
using arkAS.Models;
using arkAS.BLL.Orders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ClosedXML.Excel;
using Dapper;
using Newtonsoft.Json;
using arkAS.BLL.Calc;



namespace arkAS.Controllers.admin
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        // ------------------------------------- USERS -------------------------------------------------------
        public ActionResult Users()
        {
            return View();
        }

        public ActionResult users_getItems()
        {

            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            string roles = "";
            if (parameters.filter.ContainsKey("roles"))
            {
                roles = String.Join(",", (parameters.filter["roles"] as ArrayList).ToArray());
            }

            var user = "";
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                user = parameters.filter.ContainsKey("username") ? parameters.filter["username"].ToString() : "";
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort = sorts.Length > 0 ? sorts[0] : "";

            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var direction = directions.Length > 0 ? directions[0] : "";

            var p = new DynamicParameters();
            p.Add("applicationName", Membership.ApplicationName);
            p.Add("userName", user);
            p.Add("roles", roles);
            p.Add("sort", sort);
            p.Add("direction", direction);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rep = new CoreRepository();
            var result = rep.GetSQLData<dynamic>("GetUsers", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            return Json(new
            {
                items = result.Select(item => new
                {
                    id = item.userID,
                    name = item.userName,
                    creationDate = item.creationDate.ToString("dd.MM.yyyy"),
                    lastActivityDate = item.lastActivityDate.ToString("dd.MM.yyyy"),
                    notifyEmails = "", // item.email,
                    roles = "<div class='usUserRoles'>" + item.roles + "</div>"
                }),
                total = result.Count > 0 ? p.Get<int>("total") : 0
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeRole(string userGuid, string role, string username, bool turnOn)
        {
            var res = false;
            try
            {
                var us = Membership.GetUser(new Guid(userGuid));
                if (turnOn) System.Web.Security.Roles.AddUserToRole(us.UserName, role);
                else System.Web.Security.Roles.RemoveUserFromRole(us.UserName, role);
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

        public ActionResult CreateUser(string login, string password)
        {
            var res = false;
            var msg = "";
            var us = Membership.GetUser(login);
            try
            {

                if (us != null)
                {
                    msg = "Такой пользователь уже существует в системе";
                }
                else
                {
                    us = Membership.CreateUser(login, password, login);
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
                guid = us.ProviderUserKey.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Users_remove(string id)
        {
            var res = false;
            try
            {
                //get guid by string
                Guid userIdToDelete = new Guid(id);
                //Delete User with its id
                Membership.DeleteUser(Membership.GetUser(userIdToDelete).UserName);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {
                result = res,
                msg = "Пользователь удален!"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Users_getUserActions(string username)
        {

            var mng = new CoreRepository();
            IQueryable<as_userActions> items = null;

            DateTime dbeg = DateTime.Now;
            dbeg = dbeg.AddDays(-1);
            DateTime dend = DateTime.Now;

            if (username != "")
            {
                items = mng.GetUserActionsList(username, dbeg, dend).AsQueryable();
            }

            //сортируем по дате asc
            items = items.OrderByDescending(x => x.created);

            var total = items.Count();
            //var res2 = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    url = x.url ?? "",
                    param = x.@params ?? ""
                }),
                total = total,
                dbeg = dbeg.ToString("dd.MM.yyyy"),
                dend = dend.ToString("dd.MM.yyyy")
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Users_getUserActionsDt(string username, DateTime dbeg, DateTime dend, string text)
        {

            var mng = new CoreRepository();
            IQueryable<as_userActions> items = null;
            dend = dend.AddDays(1);//добавляем к конечной дате еще 1 день, чтобы видеть все действия

            items = mng.GetUserActionsList(username, dbeg, dend).AsQueryable();

            //сортируем по дате
            items = items.OrderByDescending(x => x.created);
            //фильтруем по тексту
            if (text != "")
            {
                items = items.ToList().Where(x =>
                       x.url != null && x.url.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                       x.@params != null && x.@params.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                       ).AsQueryable();

            }

            var total = items.Count();
            //var res2 = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    url = x.url ?? "",
                    param = x.@params ?? ""
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public ActionResult GetUserPassword(string username)
        {
            var res = Membership.GetUser(username).GetPassword();
            return Json(new
            {
                pass = res
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin")]
        public ActionResult ChangeUserPassword(string login, string oldPassword, string newPassword)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            var res = false;
            var user = Membership.GetUser(login);
            try
            {
                user.ChangePassword(oldPassword, newPassword);
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



        // ------------------------------------- ROLES -------------------------------------------------------
        public ActionResult Roles()
        {
            return View();
        }

        public ActionResult Roles_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var items = System.Web.Security.Roles.GetAllRoles().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x);
                    else items = items.OrderByDescending(x => x);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    rolePK = x,
                    role = x
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Roles_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CRMManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var role = AjaxModel.GetValueFromSaveField("role", fields);
                if (!System.Web.Security.Roles.RoleExists(role)) System.Web.Security.Roles.CreateRole(role);
                savedID = 0;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Roles_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var res = false;
            var msg = "";
            var mng = new RightsManager();
            try
            {
                if (System.Web.Security.Roles.GetUsersInRole(id).Count() > 0)
                {
                    msg = "Роль содержит пользователей. Сначала снимите эту роль у всех пользователей на странице Пользователи";
                }
                else if (mng.GetRightsByRole(id).Count > 0)
                {
                    msg = "Роль задействована в механизме прав. Сначала снимите у этой роли все права на странице Права";
                }
                else
                {
                    System.Web.Security.Roles.DeleteRole(id);
                    msg = "Роль удалена!";
                    res = true;
                }
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                msg = msg
            }, JsonRequestBehavior.AllowGet);
        }

        // ------------------------------------- RIGHTS -------------------------------------------------------
        public ActionResult Rights()
        {
            return View();
        }

        public ActionResult Rights_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new RightsManager();
            IQueryable<as_rights> items = mng.GetRights().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                List<string> roles = new List<string>();
                if (parameters.filter.ContainsKey("roles"))
                {
                    roles = (parameters.filter["roles"] as ArrayList).ToArray().Select(x => x.ToString()).ToList();
                }
                items = items.Where(x =>
                    (roles.Count == 0 || x.as_rightsRoles.Select(z => z.role).ToList().Intersect(roles).Count() > 0)
                );
                if (name != "")
                {
                    items = items.ToList().Where(x => x.name != null && x.name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.code != null && x.code.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
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
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }
            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    code = x.code ?? "",
                    roles = "<div class='usUserRoles'>" + String.Join(",", x.as_rightsRoles.Select(z => z.role)) + "</div>"
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RightsInline(int pk, string value, string name)
        {
            var mng = new RightsManager();
            mng.EditRightField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult ChangeRoleForRight(int rightID, string role, bool turnOn)
        {
            var res = false;
            var mng = new RightsManager();
            try
            {
                var item = mng.GetRight(rightID);

                var rolesRights = mng.GetRightsByRole(role);
                if (turnOn)
                {
                    if (rolesRights.FirstOrDefault(x => x.id == rightID) == null)
                    {
                        mng.SaveRightRole(new as_rightsRoles { id = 0, rightID = rightID, role = role });
                    }
                }
                else
                {
                    mng.DeleteRightRole(rightID, role);

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

        public ActionResult CreateRight(string name, string code)
        {
            var res = false;
            var msg = "";
            var mng = new RightsManager();
            var item = mng.GetRight(code);
            try
            {

                if (item != null)
                {
                    msg = "Право с таким кодом уже существует в системе";
                }
                else
                {
                    item = new as_rights { id = 0, code = code, name = name };
                    mng.SaveRight(item);
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
        public ActionResult Rights_remove(int id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var res = false;
            var msg = "";
            var mng = new RightsManager();
            var item = mng.GetRight(id);

            try
            {
                if (item != null)
                {
                    mng.DeleteRolesByRightsID(id);
                    mng.DeleteRight(item);
                    res = true;
                    msg = "Право удалено!";
                }

            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                msg = msg
            }, JsonRequestBehavior.AllowGet);
        }


        // ------------------------------------- SETTINGS -------------------------------------------------------
        public ActionResult Settings()
        {
            var mng = new SettingsManager();
            var mng2 = new CoreManager();
            ViewBag.Categories = mng.GetSettingCategories();
            ViewBag.DataTypes = mng2.GetDataTypes();

            return View();
        }

        public ActionResult Settings_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new SettingsManager();
            IQueryable<as_settings> items = mng.GetSettings().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                var categoryID = parameters.filter.ContainsKey("categoryID") ? RDL.Convert.StrToInt(parameters.filter["categoryID"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (categoryID == 0 || x.categoryID == categoryID)
                );
                if (text != "")
                {
                    items = items.ToList().Where(x =>
                        x.name != null && x.name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.value != null && x.value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.code != null && x.code.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                        ).AsQueryable();
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
                case "categoryID":
                    if (direction1 == "up") res = res.OrderBy(x => x.categoryID);
                    else res = res.OrderByDescending(x => x.categoryID);
                    break;

                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }
            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    code = x.code ?? "",
                    typeCode = x.as_dataTypes != null ? x.as_dataTypes.code : "string",
                    x.categoryID,
                    categoryName = x.as_settingCategories != null ? x.as_settingCategories.name : "",
                    value = !String.IsNullOrEmpty(x.value) ? x.value : "Нет значения",
                    x.value2
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateSetting(string name, string code, int categoryID, int typeID)
        {
            var mng = new SettingsManager();

            int? categoryID_ = null;
            if (categoryID != 0) categoryID_ = categoryID;
            int? typeID_ = null;
            if (typeID != 0) typeID_ = typeID;
            var item = new as_settings { id = 0, categoryID = categoryID_, code = code, name = name, typeID = typeID_, value = "", value2 = "" };
            mng.SaveSetting(item);

            return Json(new
            {
                result = item.id > 0,
                settingID = item.id
            });
        }

        public ActionResult ClearCache()
        {
            var mng = new CoreManager();
            var res = mng.ClearCache();
            return Json(new
            {
                result = res
            });
        }
        public ActionResult BackupDatabase()
        {
            string DataBaseFile = "";
            CoreManager mng = new CoreManager();
            string errors = mng.BackupDatabase(ref DataBaseFile);
            return Json(new
            {
                result = errors,
                DbFile = DataBaseFile
            });
        }
        public ActionResult SettingsInline(int pk, string value, string name)
        {
            var mng = new SettingsManager();
            mng.EditSettingField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Settings_remove(int id)
        {
            var res = false;
            var mng = new SettingsManager();

            if (mng.DeleteSettingsAvalValueById(id) != 1)
            {
                return Json(new
                {
                    result = res,
                    msg = "Ошибка во время удаления значений Настройки!"
                });
            }

            var item = mng.GetSetting(id);
            if (item != null)
            {
                mng.DeleteSetting(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Настройка удалена!"
            });
        }

        //        // ------------------------------------------------ COMMENTS -------------------------------------------------------------------------------
        // ------------------------------------------------ COMMENTS -------------------------------------------------------------------------------
        public ActionResult Comments()
        {
            var mngComments = new CommentManager();
            ViewBag.CommentsTypes = mngComments.GetCommentTypes();

            return View();
        }

        public ActionResult Comments_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var commentType = "0";
            var text = "";
            var user = "";

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                commentType = parameters.filter.ContainsKey("type") ? parameters.filter["type"].ToString() : "0";
                text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                user = parameters.filter.ContainsKey("username") ? parameters.filter["username"].ToString() : "";
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("commentType", commentType);
            p.Add("text", text);
            p.Add("user", user);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetComments", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");

            foreach (var item in items)
            {
                item.audio = PathMap(item.audio);
            }

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");

            //    var parameters = AjaxModel.GetParameters(HttpContext);
            //    var mng = new CommentManager();
            //    IQueryable<as_comments> items = mng.GetComments("", "").AsQueryable();

            //    if (parameters.filter != null && parameters.filter.Count > 0)
            //    {
            //        var commentType = parameters.filter.ContainsKey("type") ? parameters.filter["type"].ToString() : "0";

            //        if (commentType != "0")
            //        {
            //            items = items.ToList().Where(x => x.type != null && x.type == commentType).AsQueryable();
            //        }

            //        var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";

            //        if (text != "")
            //        {
            //            items = items.ToList().Where(x =>
            //                x.text != null && x.text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
            //        }
            //        var user = parameters.filter.ContainsKey("username") ? parameters.filter["username"].ToString() : "";

            //        if (user != "0")
            //        {
            //            items = items.ToList().Where(x =>
            //                x.username != null && x.username.IndexOf(user, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
            //        }
            //    }
            //    var res = items;


            //    var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            //    var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            //    var sort1 = sorts.Length > 0 ? sorts[0] : "";
            //    var direction1 = directions.Length > 0 ? directions[0] : "";

            //    switch (sort1)
            //    {
            //        case "username":
            //            if (direction1 == "up") res = res.OrderBy(x => x.username);
            //            else res = res.OrderByDescending(x => x.username);
            //            break;
            //        case "created":
            //            if (direction1 == "up") res = res.OrderBy(x => x.created);
            //            else res = res.OrderByDescending(x => x.created);
            //            break;
            //        case "type":
            //            if (direction1 == "up") res = res.OrderBy(x => x.type);
            //            else res = res.OrderByDescending(x => x.type);
            //            break;
            //        case "itemID":
            //            if (direction1 == "up") res = res.OrderBy(x => x.itemID);
            //            else res = res.OrderByDescending(x => x.itemID);
            //            break;

            //        default:
            //            if (direction1 == "up") res = res.OrderBy(x => x.username);
            //            else res = res.OrderByDescending(x => x.username);
            //            break;
            //    }
            //    var total = res.Count();
            //    var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            //    //var mng2 = new CategoryManager();
            //    //var allCats = mng2.GetCategories("text");
            //    return Json(new
            //    {
            //        items = res2.Select(x => new
            //        {
            //            x.id,
            //            username = x.username ?? "",
            //            text = x.text ?? "",
            //            created = x.created.Value.ToString("dd.MM.yyyy") ?? "",
            //            x.itemID,
            //            type = x.type ?? "",
            //            x.audio
            //        })
            //            //items = res2
            //        ,
            //        total = total
            //    }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CommentsInline(int pk, string value, string name)
        {
            var mng = new CommentManager();
            mng.EditTextField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult GetComment(int id)
        {
            var mng = new CommentManager();
            var t = mng.GetComment(id);

            return Json(new
            {
                result = true,
                text = t.text
            });
        }

        public ActionResult GetAudioComment(int id)
        {
            var mng = new CommentManager();
            string path = mng.GetComment(id).audio;
            return File(path, "audio/vnd.wave", "voice_" + id + Path.GetExtension(path));
        }


        public ActionResult Comments_remove(int id)
        {
            var res = false;
            var mng = new CommentManager();

            var item = mng.GetComment(id);
            if (item != null)
            {
                mng.DeleteComment(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Комментарий удален!"
            });
        }

        private static string PathMap(string path)
        {
            if (path != null)
            {
                string approot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath.TrimEnd('\\');
                return path.Replace(approot, string.Empty).Replace('\\', '/');
            }
            else
            {
                return path;
            }
        }


        // ------------------------------------------------ OPINIONS -------------------------------------------------------------------------------
        public ActionResult Opinions()
        {
            var mngOpinion = new OpinionManager();
            return View();
        }

        public ActionResult Opinions_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var comment = "";
            var user = "";
            var like = "";

            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                comment = parameters.filter.ContainsKey("comment") ? parameters.filter["comment"].ToString() : "";
                user = parameters.filter.ContainsKey("username") ? parameters.filter["username"].ToString() : "";
                like = parameters.filter.ContainsKey("like") ? parameters.filter["like"].ToString() : "0";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("comment", comment);
            p.Add("user", user);
            p.Add("like", like);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetOpinions", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult OpinionsInline(int pk, string value, string name)
        {
            var mng = new OpinionManager();
            mng.EditTextField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult GetOpinion(int id)
        {
            var mng = new OpinionManager();
            var t = mng.GetOpinion(id);

            return Json(new
            {
                result = true,
                comment = t.comment,
                like = t.like
            });
        }

        public ActionResult Opinions_remove(int id)
        {
            var res = false;
            var mng = new OpinionManager();

            var item = mng.GetOpinion(id);
            if (item != null)
            {
                mng.DeleteOpinion(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Мнение удалено!"
            });
        }

        // ------------------------------------------------ TEXTS -------------------------------------------------------------------------------
        // ------------------------------------- SETTINGS -------------------------------------------------------
        public ActionResult Texts()
        {
            var mng = new CategoryManager();
            ViewBag.Categories = mng.GetCategories("text");
            return View();
        }

        public ActionResult Texts_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new TextManager();
            IQueryable<as_texts> items = mng.GetTexts().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                List<int?> categoryIDs = new List<int?>();
                if (parameters.filter.ContainsKey("categoryIDs"))
                {
                    categoryIDs = (parameters.filter["categoryIDs"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }
                items = items.Where(x =>
                    (categoryIDs.Count == 0 || categoryIDs.Contains(x.categoryID))
                );
                if (text != "")
                {
                    items = items.ToList().Where(x =>
                        x.name != null && x.name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.code != null && x.code.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                        ).AsQueryable();
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
                case "categoryID":
                    if (direction1 == "up") res = res.OrderBy(x => x.categoryID);
                    else res = res.OrderByDescending(x => x.categoryID);
                    break;

                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }
            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            var mng2 = new CategoryManager();
            var allCats = mng2.GetCategories("text");
            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    code = x.code ?? "",
                    x.categoryID,
                    categoryName = GetCatName(x.categoryID, allCats)
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetCatName(int? id, List<as_categories> allCats)
        {
            var res = "";
            var item = allCats.FirstOrDefault(x => x.id == id);
            if (item != null) res = item.name;
            return res;
        }


        public ActionResult CreateText(string name, string code, int categoryID)
        {
            var mng = new TextManager();

            int? categoryID_ = null;
            if (categoryID != 0) categoryID_ = categoryID;
            var item = new as_texts { id = 0, categoryID = categoryID_, code = code, name = name, text = "" };
            mng.SaveText(item);
            return Json(new
            {
                result = item.id > 0,
                textID = item.id
            });
        }

        public ActionResult TextsInline(int pk, string value, string name)
        {
            var mng = new TextManager();
            mng.EditTextField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }
        public ActionResult GetText(string code)
        {
            var mng = new TextManager();
            var t = mng.GetText(code);

            return Json(new
            {
                result = true,
                text = t.text
            });
        }
        public ActionResult Texts_remove(int id)
        {
            var res = false;
            var mng = new TextManager();

            var item = mng.GetText(id);
            if (item != null)
            {
                mng.DeleteText(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Текст удален!"
            });
        }
        // --------------------- CategoriesDict --------------------------
        public ActionResult Categories()
        {
            var mng = new CategoryManager();
            ViewBag.ParentCategories = mng.db.GetCategories();//список категорий-родителей, используем при формировании <select>

            return View();
        }

        public ActionResult Categories_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);      //получили параметры входящего запроса
            var mng = new CategoryManager();

            var items = mng.db.GetCategories().AsQueryable();              //получили все строки из бд

            //----обработка фильтра-----------------------------------------------------------------------------------------------------------------
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var typeCode = parameters.filter.ContainsKey("typeCode") ? parameters.filter["typeCode"].ToString() : "";

                if (typeCode != "")
                {
                    items = items.ToList().Where(x => x.typeCode != null && x.typeCode.IndexOf(typeCode, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
                }
            }

            //----обработка сортировки-----------------------------------------------------------------------------------------------------------------
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);            //получаем массив(строк) названий полей для сортировки
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);  //направление сортировки
            var sort1 = sorts.Length > 0 ? sorts[0] : "";               //если сортируем - берем первый элемент из массива
            var direction1 = directions.Length > 0 ? directions[0] : "";//и направление для него

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
                case "parentName":
                    //if (direction1 == "up") items = items.OrderBy(x => x.as_categories2.name);
                    //else items = items.OrderByDescending(x => x.as_categories2.name);
                    //из-за некорректной обработки null-значений для x.as_categories2.name
                    //сортируем по parentID
                    if (direction1 == "up") items = items.OrderBy(x => x.parentID);
                    else items = items.OrderByDescending(x => x.parentID);
                    break;
                case "typeCode":
                    if (direction1 == "up") items = items.OrderBy(x => x.typeCode);
                    else items = items.OrderByDescending(x => x.typeCode);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
            }

            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.name,
                    x.parentID,
                    parentName = x.as_categories2 != null ? x.as_categories2.name : "",
                    x.typeCode
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Categories_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new CategoryManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var typeCode = AjaxModel.GetValueFromSaveField("typeCode", fields);

                int? parentID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("parentName", fields), 0);
                if (parentID == 0) parentID = null;

                var item = new as_categories { id = id, name = name, typeCode = typeCode, parentID = parentID };
                mng.SaveCategory(item);
                savedID = item.id;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Categories_remove(int id)
        {
            var res = false;
            var mng = new CategoryManager();
            //проверка на дочерние
            var itemchild = mng.GetCategoryChild(id);
            if (itemchild.Count != 0)
            {
                return Json(new
                {
                    result = res,
                    msg = "Сперва удалите/измените дочерние категории!"
                });
            }
            //у texts устанавливаем NULL
            if (mng.SetNullCategoryText(id) != 1)
            {
                return Json(new
                {
                    result = res,
                    msg = "Ошибка обновления Текстов, ссылающихся на эту категорию!"
                });
            }

            var item = mng.GetCategory(id);
            if (item != null)
            {
                mng.DeleteCategory(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Категория удалена!"
            });

        }



        // ----------------------- TracesDict ---------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Traces()
        {
            return View();
        }
        public ActionResult TracesInline(int pk, string value, string name)
        {
            var mng = new TraceManager();
            mng.EditTextField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Traces_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var text = "";

            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();

            p.Add("text", text);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetTraces", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult Traces_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new TraceManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var header = AjaxModel.GetValueFromSaveField("header", fields);
                var text = AjaxModel.GetValueFromSaveField("text", fields);
                var code = AjaxModel.GetValueFromSaveField("code", fields);
                var created = DateTime.Now;
                var itemID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("itemID", fields), 0);

                var item = new as_trace { id = id, header = header, text = text, code = code, created = created, itemID = itemID };
                mng.SaveTrace(item);
                savedID = item.id;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Traces_remove(int id)
        {
            var res = false;
            var mng = new TraceManager();

            var item = mng.GetTrace(id);
            if (item != null)
            {
                mng.DeleteTrace(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Трассировка удалена!"
            });
        }


        // ----------------------- FavoritesDict ---------------------------------------------------------------------------------------------------------------------------------

        public ActionResult Favorites()
        {
            return View();
        }

        public ActionResult Favorites_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new BLL.Favorites.FavoritesManager();

            var appName = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                appName = parameters.filter.ContainsKey("appName") ? parameters.filter["appName"].ToString() : "";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("appName", appName);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetFavorites", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult Favorites_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new BLL.Favorites.FavoritesManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var itemID = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("itemID", fields), 0);
                var appName = AjaxModel.GetValueFromSaveField("appName", fields);
                var created = RDL.Convert.StrToDateTime(AjaxModel.GetValueFromSaveField("created", fields), DateTime.MinValue);
                var userGuid = new BLL.Core.CoreManager().GetUserGuid();

                var item = new as_favorites { id = id, itemID = itemID, appName = appName, created = created, userGuid = userGuid };
                mng.SaveFavorite(item);
                savedID = item.id;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Favorites_remove(int id)
        {
            var res = false;
            var mng = new BLL.Favorites.FavoritesManager();

            var item = mng.GetItem(id);
            if (item != null)
            {
                mng.DeleteFavorites(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Избранное удалено!"
            });
        }



        public ActionResult FavoritesInline(int pk, string value, string name)
        {
            var mng = new BLL.Favorites.FavoritesManager();
            mng.EditFavoriteField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }






        // ----------------------- StatusesDict ---------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Statuses()
        {
            return View();
        }

        public ActionResult Statuses_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new StatusesManager();

            var items = mng.db.GetStatuses().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
                case "color":
                    if (direction1 == "up") items = items.OrderBy(x => x.color);
                    else items = items.OrderByDescending(x => x.color);
                    break;
                case "code":
                    if (direction1 == "up") items = items.OrderBy(x => x.code);
                    else items = items.OrderByDescending(x => x.code);
                    break;
                case "desc":
                    if (direction1 == "up") items = items.OrderBy(x => x.desc);
                    else items = items.OrderByDescending(x => x.desc);
                    break;
                case "typeCode":
                    if (direction1 == "up") items = items.OrderBy(x => x.typeCode);
                    else items = items.OrderByDescending(x => x.typeCode);
                    break;

                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.id);
                    else items = items.OrderByDescending(x => x.id);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.name,
                    x.color,
                    x.code,
                    x.desc,
                    x.typeCode
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statuses_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new StatusesManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var color = AjaxModel.GetValueFromSaveField("color", fields);
                var code = AjaxModel.GetValueFromSaveField("code", fields);
                var desc = AjaxModel.GetValueFromSaveField("desc", fields);
                var typeCode = AjaxModel.GetValueFromSaveField("typeCode", fields);

                var item = new as_statuses { id = id, name = name, color = color, code = code, desc = desc, typeCode = typeCode };
                mng.SaveStatus(item);
                savedID = item.id;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statuses_remove(int id)
        {
            var res = false;
            var mng = new StatusesManager();

            var item = mng.GetStatus(id);
            if (item != null)
            {
                mng.DeleteStatus(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Статус удален!"
            });
        }

        // ----------------------- SettingCategoriesDict -----------------------------------------------------------------------------------------------------------------------
        public ActionResult SettingCategories()
        {
            return View();
        }

        public ActionResult SettingCategories_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new SettingsManager();

            var items = mng.GetSettingCategories().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
                case "ord":
                    if (direction1 == "up") items = items.OrderBy(x => x.ord);
                    else items = items.OrderByDescending(x => x.ord);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.id);
                    else items = items.OrderByDescending(x => x.id);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.name,
                    x.ord
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SettingCategories_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new SettingsManager();
            var res = false;
            int savedID = 0;
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();

                var id = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("id", fields), 0);
                var name = AjaxModel.GetValueFromSaveField("name", fields);
                var ord = RDL.Convert.StrToInt(AjaxModel.GetValueFromSaveField("ord", fields), 0);

                var item = new as_settingCategories { id = id, name = name, ord = ord };
                mng.SaveSettingCategory(item);
                savedID = item.id;
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                savedID = savedID,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SettingCategories_remove(int id)
        {
            var res = false;
            var mng = new SettingsManager();

            var itemchild = mng.GetSettingsByCategoryID(id);
            if (itemchild.Count != 0)
            {
                return Json(new
                {
                    result = res,
                    msg = "Сперва удалите/измените Настройку, ссылающуюся на эту Категорию!"
                });
            }

            var item = mng.GetSettingCategories(id);
            if (item != null)
            {
                mng.DeleteSettingCategory(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Категория настроек удалена!"
            });
        }

        // ------------------------------------- MENU -------------------------------------------------------
        public ActionResult Menu()
        {
            var mng = new MenuManager();
            ViewBag.parentName = mng.GetMenu(true); //список родительских пунктов меню
            return View();
        }

        public ActionResult Menu_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new MenuManager();
            IQueryable<as_menu> items = mng.GetMenu(true).AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var parentID = parameters.filter.ContainsKey("parentID") ? RDL.Convert.StrToInt(parameters.filter["parentID"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (parentID == 0 || x.parentID == parentID)
                );

                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                if (text != "")
                {
                    items = items.ToList().Where(x =>
                        x.name != null && x.name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.url != null && x.url.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                        ).AsQueryable();
                }

                List<string> roles = new List<string>();
                if (parameters.filter.ContainsKey("roles"))
                {
                    roles = (parameters.filter["roles"] as ArrayList).ToArray().Select(x => x.ToString()).ToList();
                }
                items = items.Where(x =>
                    (roles.Count == 0 || mng.getRolesForMenu(x.id).ToList().Intersect(roles).Count() > 0)
                );
            }


            var sorts = parameters.sort != null ? parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries) : (new List<string>()).ToArray();
            var directions = parameters.direction != null ? parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries) : (new List<string>()).ToArray();
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
                case "url":
                    if (direction1 == "up") items = items.OrderBy(x => x.url);
                    else items = items.OrderByDescending(x => x.url);
                    break;
                case "pattern":
                    if (direction1 == "up") items = items.OrderBy(x => x.pattern);
                    else items = items.OrderByDescending(x => x.pattern);
                    break;
                case "parentID":
                    if (direction1 == "up") items = items.OrderBy(x => x.parentID);
                    else items = items.OrderByDescending(x => x.parentID);
                    break;

                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.name);
                    else items = items.OrderByDescending(x => x.name);
                    break;
            }
            var total = items.Count();
            var res2 = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    url = x.url ?? "",
                    pattern = x.pattern ?? "",
                    x.parentID,
                    parentName = x.as_menu2 != null ? x.as_menu2.name : "",
                    roles = "<div class='usMenuRoles'>" + String.Join(",", mng.getRolesForMenu(x.id)) + "</div>"
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeMenuRole(string role, int itemID, bool turnOn)
        {
            var res = false;
            var mng = new MenuManager();
            try
            {
                if (turnOn)
                    mng.addMenuToRole(itemID, role);
                else
                    mng.removeMenuToRole(itemID, role);
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

        public ActionResult CreateMenu(string name, string url, string pattern, int parentID)
        {
            var mng = new MenuManager();
            int? parentID_ = null;
            if (parentID != 0) parentID_ = parentID;
            var item = new as_menu { id = 0, parentID = parentID_, name = name, url = url, pattern = pattern };
            mng.SaveMenu(item);

            return Json(new
            {
                result = item.id > 0,
                menuID = item.id
            });
        }

        public ActionResult MenuInline(int pk, string value, string name)
        {
            var mng = new MenuManager();
            mng.EditMenuField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Menu_remove(int id)
        {
            var res = false;
            var mng = new MenuManager();

            var itemchild = mng.GetMenuChild(id);
            if (itemchild.Count != 0)
            {
                return Json(new
                {
                    result = res,
                    msg = "Сперва удалите/измените дочерние пункты меню!"
                });
            }

            var item = mng.GetMenu(id);
            if (item != null)
            {
                mng.DeleteMenu(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = ""
            });
        }

        public ActionResult Test()
        {
            return View();
        }


        #region Orders
        public ActionResult Orders()
        {

            var mng = new OrderManager();
            ViewBag.OrderStatuses = mng.GetStatuses();
            ViewBag.Clients = mng.GetClients();

            return View();
        }

        public ActionResult Orders_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);


            var addedBy = "";
            var orderNum = "";
            var statusID = "";
            var clientID = "";
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                addedBy = parameters.filter.ContainsKey("addedBy") ? parameters.filter["addedBy"].ToString() : "";
                orderNum = parameters.filter.ContainsKey("orderNum") ? parameters.filter["orderNum"].ToString() : "";
                statusID = parameters.filter.ContainsKey("statusID") ? parameters.filter["statusID"].ToString() : "0";
                clientID = parameters.filter.ContainsKey("clientID") ? parameters.filter["clientID"].ToString() : "0";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("addedBy", addedBy);
            p.Add("orderNum", orderNum);
            p.Add("clientID", clientID);
            p.Add("statusID", statusID);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetOrders", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateOrder(string orderNum, int statusID, int clientID)
        {

            var mng = new OrderManager();
            var item = new crm_orders
            {
                id = 0,
                addedBy = User.Identity.Name,
                orderNum = orderNum,
                statusID = statusID,
                clientID = clientID,
                created = DateTime.Now.Date,
            };
            mng.SaveOrder(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrder(int id)
        {
            var mng = new OrderManager();
            var item = mng.GetOrder(id);
            return Json(new
            {
                id = item.id,
                addedBy = item.addedBy,
                orderNum = item.orderNum,
                statusID = item.statusID,
                clientID = item.clientID,
                created = item.created.ToString(),
            });
        }


        public ActionResult OrdersInline(int pk, string value, string name)
        {
            var mng = new OrderManager();
            mng.EditOrderField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }


        public ActionResult Orders_remove(int id)
        {
            var res = false;
            var mng = new OrderManager();

            var item = mng.GetOrder(id);
            if (item != null)
            {
                mng.DeleteOrder(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Документ удален!"
            });
        }

        #endregion



        #region Article

        // /Admin/GetArticleTable
        public RedirectToRouteResult GetArticleTable()
        {
            return RedirectToAction("GetArticleTable", "Article");
        }
        // /Admin/GetArticleTypeTable
        public RedirectToRouteResult GetArticleTypeTable()
        {
            return RedirectToAction("GetArticleTypeTable", "Article");
        }

        #endregion

        #region HotKeys
        public ActionResult HotKeys()
        {
            return View();
        }
        public ActionResult HotKeys_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new HotKeyManager();
            IQueryable<as_hotkeys> items = mng.GetHotKeys().AsQueryable();
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var KeyCode = parameters.filter.ContainsKey("KeyCodeName") && parameters.filter["KeyCodeName"] != "" ? Convert.ToInt32(parameters.filter["KeyCodeName"]) : 0;
                var KeyCodeJs = parameters.filter.ContainsKey("KeyCodeJs") ? parameters.filter["KeyCodeJs"].ToString() : "";
                var KeyCodeUrl = parameters.filter.ContainsKey("KeyCodeUrl") ? parameters.filter["KeyCodeUrl"].ToString() : "";
                List<string> roles = new List<string>();
                if (parameters.filter.ContainsKey("roles"))
                {
                    roles = (parameters.filter["roles"] as ArrayList).ToArray().Select(x => x.ToString()).ToList();
                }
                items = items.Where(x =>
                    (roles.Count == 0 || mng.getRolesForHotKey(x.Id).ToList().Intersect(roles).Count() > 0)
                );
                if (KeyCode != 0)
                {
                    items = items.ToList().Where(x => x.keyCode != null && x.keyCode == KeyCode).AsQueryable();
                }
                if (KeyCodeJs != "")
                {
                    items = items.ToList().Where(x => x.js != null && x.js == KeyCodeJs).AsQueryable();
                }
                if (KeyCodeUrl != "")
                {
                    items = items.ToList().Where(x => x.url != null && x.url == KeyCodeUrl).AsQueryable();
                }
            }
            var res = items;

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";


            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();
            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.Id,
                    keyCode = x.keyCode == 8 ? "backspace" :
                              x.keyCode == 9 ? "Tab" :
                              x.keyCode == 13 ? "enter" :
                              x.keyCode == 19 ? "pause/break" :
                              x.keyCode == 20 ? "caps lock" :
                              x.keyCode == 27 ? "escape" :
                              x.keyCode == 33 ? "page up" :
                              x.keyCode == 34 ? "page down" :
                              x.keyCode == 35 ? "end" :
                              x.keyCode == 36 ? "home" :
                              x.keyCode == 37 ? "left arrow" :
                              x.keyCode == 38 ? "up arrow" :
                              x.keyCode == 39 ? "right arrow" :
                              x.keyCode == 40 ? "down arrow" :
                              x.keyCode == 45 ? "insert" :
                              x.keyCode == 46 ? "delete" :
                              x.keyCode == 48 ? "0" :
                              x.keyCode == 49 ? "1" :
                              x.keyCode == 50 ? "2" :
                              x.keyCode == 51 ? "3" :
                              x.keyCode == 52 ? "4" :
                              x.keyCode == 53 ? "5" :
                              x.keyCode == 54 ? "6" :
                              x.keyCode == 55 ? "7" :
                              x.keyCode == 56 ? "8" :
                              x.keyCode == 57 ? "9" :
                              x.keyCode == 65 ? "A" :
                              x.keyCode == 66 ? "B" :
                              x.keyCode == 67 ? "C" :
                              x.keyCode == 68 ? "D" :
                              x.keyCode == 69 ? "E" :
                              x.keyCode == 70 ? "F" :
                              x.keyCode == 71 ? "G" :
                              x.keyCode == 72 ? "H" :
                              x.keyCode == 73 ? "I" :
                              x.keyCode == 74 ? "J" :
                              x.keyCode == 75 ? "K" :
                              x.keyCode == 76 ? "L" :
                              x.keyCode == 77 ? "M" :
                              x.keyCode == 78 ? "N" :
                              x.keyCode == 79 ? "O" :
                              x.keyCode == 80 ? "P" :
                              x.keyCode == 81 ? "Q" :
                              x.keyCode == 82 ? "R" :
                              x.keyCode == 83 ? "S" :
                              x.keyCode == 84 ? "T" :
                              x.keyCode == 85 ? "U" :
                              x.keyCode == 86 ? "V" :
                              x.keyCode == 87 ? "W" :
                              x.keyCode == 88 ? "X" :
                              x.keyCode == 89 ? "Y" :
                              x.keyCode == 90 ? "Z" :
                              x.keyCode == 91 ? "left window key" :
                              x.keyCode == 92 ? "right window key" :
                              x.keyCode == 93 ? "select key" :
                              x.keyCode == 96 ? "numpad 0" :
                              x.keyCode == 97 ? "numpad 1" :
                              x.keyCode == 98 ? "numpad 2" :
                              x.keyCode == 99 ? "numpad 3" :
                              x.keyCode == 100 ? "numpad 4" :
                              x.keyCode == 101 ? "numpad 5" :
                              x.keyCode == 102 ? "numpad 6" :
                              x.keyCode == 103 ? "numpad 7" :
                              x.keyCode == 104 ? "numpad 8" :
                              x.keyCode == 105 ? "numpad 9" :
                              x.keyCode == 106 ? "multiply" :
                              x.keyCode == 107 ? "add" :
                              x.keyCode == 109 ? "subtract" :
                              x.keyCode == 110 ? "decimal point" :
                              x.keyCode == 111 ? "divide" :
                              x.keyCode == 112 ? "f1" :
                              x.keyCode == 113 ? "f2" :
                              x.keyCode == 114 ? "f3" :
                              x.keyCode == 115 ? "f4" :
                              x.keyCode == 116 ? "f5" :
                              x.keyCode == 117 ? "f6" :
                              x.keyCode == 118 ? "f7" :
                              x.keyCode == 119 ? "f8" :
                              x.keyCode == 120 ? "f9" :
                              x.keyCode == 121 ? "f10" :
                              x.keyCode == 122 ? "f11" :
                              x.keyCode == 123 ? "f12" :
                              x.keyCode == 144 ? "num lock" :
                              x.keyCode == 145 ? "scroll lock" :
                              x.keyCode == 186 ? "semi-colon" :
                              x.keyCode == 187 ? "equal sign" :
                              x.keyCode == 188 ? "comma" :
                              x.keyCode == 189 ? "dash" :
                              x.keyCode == 190 ? "period" :
                              x.keyCode == 191 ? "forward slash" :
                              x.keyCode == 192 ? "grave accent" :
                              x.keyCode == 219 ? "open bracket" :
                              x.keyCode == 220 ? "back slash" :
                              x.keyCode == 221 ? "close braket" :
                              x.keyCode == 222 ? "single quote" : "",
                    js = x.js ?? "",
                    url = x.url ?? "",
                    isAlt = "<input class='usHotKeyisAlt' type='checkbox' id='isAlt." + x.Id.ToString() + "'" + ((x.isAlt ?? false) ? " checked='checked'" : "") + " />",
                    isCtrl = "<input class='usHotKeyisCtrl' type='checkbox' id='isCtrl." + x.Id.ToString() + "'" + ((x.isCtrl ?? false) ? " checked='checked'" : "") + " />",
                    isShift = "<input class='usHotKeyisShift' type='checkbox' id='isShift." + x.Id.ToString() + "'" + ((x.isShift ?? false) ? " checked='checked'" : "") + " />",
                    roles = "<div class='usHotKeyRoles'>" + x.roles + "</div>"

                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateHotKey(string keyCode, string js, string url)
        {
            var mng = new HotKeyManager();
            var item = new as_hotkeys
            {
                keyCode = Convert.ToInt32(keyCode),
                js = js,
                url = url,
                isAlt = false,
                isCtrl = false,
                isShift = false,
                roles = "admin"
            };
            mng.SaveHotKey(item);

            return Json(new
            {
                result = item.Id > 0
            });
        }
        public ActionResult HotKeys_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new HotKeyManager();
            try
            {
                mng.DeleteHotKey(int.Parse(id));
                return Json(new
                {
                    result = true,
                    msg = "Операция успешна"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult HotKeyInline(int pk, string value, string name)
        {
            try
            {
                var mng = new HotKeyManager();
                var hotkey = mng.GetHotKey(pk);
                if (hotkey != null)
                {
                    switch (name)
                    {
                        case "keyCode": hotkey.keyCode = Convert.ToInt32(value); break;
                        case "url": hotkey.url = value; hotkey.js = null; break;
                        case "js": hotkey.js = value; hotkey.url = null; break;
                        case "isAlt": hotkey.isAlt = bool.Parse(value); break;
                        case "isCtrl": hotkey.isCtrl = bool.Parse(value); break;
                        case "isShift": hotkey.isShift = bool.Parse(value); break;
                        case "roles":
                            {
                                List<string> m = hotkey.roles.Split(',').ToList();
                                string[] s = value.Split('=');
                                if (bool.Parse(s[1]))
                                {
                                    if (!m.Exists(x => x == s[0])) m.Add(s[0]);
                                }
                                else
                                {
                                    m.Remove(s[0]);
                                }
                                hotkey.roles = String.Join(",", m);
                                break;
                            }
                    }
                    mng.SaveHotKey(hotkey);
                    return Json(new
                    {
                        result = true,
                        msg = "Операция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Ошибка"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Access(int id)
        {
            bool res = false;
            try
            {
                var mng = new HotKeyManager();
                res = mng.HaveCurrentUserAccess(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(res);
        }
        public ActionResult HotKeysForUser()
        {
            var items = new List<as_hotkeys>();
            try
            {
                var mng = new HotKeyManager();
                items = mng.GetHotKeys();
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            var total = items.Count();
            return Json(new
            {
                items = items.Select(x => new
                {
                    id = x.Id,
                    key = x.keyCode,
                    isAlt = x.isAlt != null && x.isAlt.Value ? true : false,
                    isCtrl = x.isCtrl != null && x.isCtrl.Value ? true : false,
                    isShift = x.isShift != null && x.isShift.Value ? true : false,
                    js = x.js,
                    url = x.url,
                    roles = x.roles
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Metrics
        public ActionResult Metrics()
        {
            var mng = new MetricsManager();
            ViewBag.parentName = mng.GetAllMetrics(false);
            ViewBag.metricTypetName = mng.GetAllMetricTypes();
            return View();
        }
        public ActionResult Metrics_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new MetricsManager();
            IQueryable<as_mt_metrics> items = mng.GetAllMetrics(false).AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var metricName = parameters.filter.ContainsKey("metricname") ? parameters.filter["metricname"].ToString() : "";
                List<string> roles = new List<string>();
                if (parameters.filter.ContainsKey("roles"))
                {
                    roles = (parameters.filter["roles"] as ArrayList).ToArray().Select(x => x.ToString()).ToList();
                }
                items = items.Where(x =>
                    (roles.Count == 0 || mng.getRolesForMetric(x.id).ToList().Intersect(roles).Count() > 0)
                );
                if (metricName != "")
                {
                    items = items.ToList().Where(x => x.title != null && x.title.IndexOf(metricName, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
                }
            }
            var res = items;

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";


            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    title = x.title ?? "",
                    subtitle = x.subtitle ?? "",
                    sql = x.sql ?? "",
                    parentName = (x.as_mt_metrics2 != null) ? x.as_mt_metrics2.title : "",
                    parName = x.parName ?? "",
                    ord = x.ord,
                    typeName = (x.typeID != null) ? mng.GetType(x.typeID).name : "",
                    isSP = "<input class='usMetricisSP' type='checkbox' id='isSP." + x.id.ToString() + "'" + ((x.isSP ?? false) ? " checked='checked'" : "") + " />",
                    users = x.users ?? "",
                    roles = "<div class='usMetricRoles'>" + x.roles + "</div>"

                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateMetric(string title, string subtitle, string sql, int parentID, int metricTypeID)
        {
            var mng = new MetricsManager();
            int? parentID_ = null;
            if (parentID != 0) parentID_ = parentID;
            int? metricTypeID_ = null;
            if (metricTypeID != 0) metricTypeID_ = metricTypeID;
            var item = new as_mt_metrics
            {
                id = 0,
                parentID = parentID_,
                title = title,
                subtitle = subtitle,
                sql = sql,
                typeID = metricTypeID_,
                ord = 1,
                isSP = false,
                roles = "admin"
            };
            mng.SaveMetric(item);

            return Json(new
            {
                result = item.id > 0,
                metricID = item.id
            });
        }
        public ActionResult Metrics_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new MetricsManager();
            try
            {
                mng.DeleteMetric(int.Parse(id));
                return Json(new
                {
                    result = true,
                    msg = "Операция успешна"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MetricsInline(int pk, string value, string name)
        {
            try
            {
                var mng = new MetricsManager();
                var metrics = mng.GetAllMetrics(true).FirstOrDefault(x => x.id == pk);
                if (metrics != null)
                {
                    switch (name)
                    {
                        case "title": metrics.title = value; break;
                        case "subtitle": metrics.subtitle = value; break;
                        case "ord": metrics.ord = int.Parse((value == "") ? "0" : value); break;
                        case "sql": metrics.sql = value; break;
                        case "isSP": metrics.isSP = bool.Parse(value); break;
                        case "parentName":
                            {
                                if (value == "")
                                    metrics.parentID = null;
                                else
                                    metrics.parentID = int.Parse(value);
                                break;
                            }
                        case "roles":
                            {
                                List<string> m = metrics.roles.Split(',').ToList();
                                string[] s = value.Split('=');
                                if (bool.Parse(s[1]))
                                {
                                    if (!m.Exists(x => x == s[0])) m.Add(s[0]);
                                }
                                else
                                {
                                    m.Remove(s[0]);
                                }
                                metrics.roles = String.Join(",", m);
                                break;
                            }
                        case "parName": metrics.parName = value; break;
                        case "typeName": metrics.typeID = int.Parse(value); break;
                        case "users": metrics.users = value; break;
                    }
                    mng.SaveMetric(metrics);
                    return Json(new
                    {
                        result = true,
                        msg = "Операция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Ошибка"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MetricChangeRole(string role, int pk, bool turnOn)
        {
            var res = false;
            var mng = new MetricsManager();
            try
            {
                var metrics = mng.GetAllMetrics(true).FirstOrDefault(x => x.id == pk);
                if (metrics != null)
                {
                    if (turnOn)
                        metrics.roles += (metrics.roles.Length > 0) ? "," : "" + role;
                    else
                        metrics.roles = metrics.roles.Replace(role, "").Replace(",,", ",");
                    mng.SaveMetric(metrics);
                }
                return Json(new
                {
                    result = true,
                    msg = "Операция успешна"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region MetricType
        public ActionResult MetricsType()
        {
            return View();
        }
        public ActionResult metricsType_getItems()
        {
            var mng = new MetricsManager();
            var items = mng.GetAllMetricTypes();
            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    x.name,
                    x.code,
                    x.ord
                }),
                total = items.Count
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult metricsType_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new MetricsManager();
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newMetricsType = new as_mt_metricTypes
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = AjaxModel.GetValueFromSaveField("name", fields),
                    code = AjaxModel.GetValueFromSaveField("code", fields),
                    ord = int.Parse(AjaxModel.GetValueFromSaveField("ord", fields))
                };

                mng.SaveMetricType(newMetricsType);
                return Json(new
                {
                    result = true,
                    id = newMetricsType.id,
                    msg = "Операция успешна"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    id = 0,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult metricsType_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new MetricsManager();
            try
            {
                if (mng.GetMetrics(int.Parse(id)).Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Тип метрики связан с настроенное метрикой, сначало требуется удалить метрики данного типа"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeleteMetricType(int.Parse(id));
                    return Json(new
                    {
                        result = true,
                        msg = "Операция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MetricsTypeInline(int pk, string value, string name)
        {
            try
            {
                var mng = new MetricsManager();
                var metricsType = mng.GetAllMetricTypes().FirstOrDefault(x => x.id == pk);
                if (metricsType != null)
                {
                    switch (name)
                    {
                        case "name": metricsType.name = value; break;
                        case "code": metricsType.code = value; break;
                        case "ord": metricsType.ord = int.Parse(value); break;
                    }
                    mng.SaveMetricType(metricsType);
                    return Json(new
                    {
                        result = true,
                        msg = "Операция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Ошибка"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    msg = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region UserCheckSets
        // ------------------------------------- UserCheckSets -------------------------------------------------------
        public ActionResult UserCheckSets()
        {
            //var mng = new MenuManager();
            //ViewBag.parentName = mng.GetMenu(true); //список родительских пунктов меню
            return View();
        }

        public ActionResult UserCheckSets_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new UserCheckSetManager();
            IQueryable<as_userCheckSet> items = mng.GetUserCheckSets().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                if (name != "")
                {
                    items = items.ToList().Where(x =>
                        x.name != null && x.name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0).AsQueryable();
                }
                var code = parameters.filter.ContainsKey("code") ? parameters.filter["code"].ToString() : "";
                if (code != "")
                {
                    items = items.ToList().Where(x =>
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
                case "name":
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
                case "code":
                    if (direction1 == "up") res = res.OrderBy(x => x.code);
                    else res = res.OrderByDescending(x => x.code);
                    break;

                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }
            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    name = x.name ?? "",
                    code = x.code ?? "",
                    roles = x.roles ?? ""
                }),
                //items = res,
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateUserCheckSet(string name, string code, string roles)
        {
            var mng = new UserCheckSetManager();

            var item = mng.AddUserCheckSet(code, name, String.IsNullOrEmpty(roles) ? "admin" : roles);
            mng.SaveUserCheckSet(item);
            return Json(new
            {
                result = item.id > 0,
                textID = item.id
            });
        }

        public ActionResult UserCheckSetsInline(int pk, string name, string code, string roles)
        {
            var mng = new UserCheckSetManager();
            mng.EditUserCheckSet(pk, name, code, roles);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult GetUserCheckSet(int id)
        {
            var mng = new UserCheckSetManager();
            var t = mng.GetUserCheckSet(id);

            return Json(new
            {
                result = true,
                name = t.name,
                code = t.code,
                roles = t.roles
            });
        }

        public ActionResult UserCheckSets_remove(int id)
        {
            var res = false;
            var mng = new UserCheckSetManager();

            var item = mng.GetUserCheckSet(id);
            if (item != null)
            {
                mng.DeleteUserCheckSet(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Тип материалов для рассылки удален!"
            });
        }

        #endregion

        #region UserCheckItems
        // ------------------------------------- UserCheckItems -------------------------------------------------------
        public ActionResult UserCheckItems()
        {
            var mng = new UserCheckSetManager();
            ViewBag.CheckSets = mng.GetUserCheckSets();
            return View();
        }

        public ActionResult UserCheckItems_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new UserCheckSetManager();

            var name = "";
            var description = "";
            var emailText = "";
            var emailSubject = "";
            var setID = "";
            var isFilterDate = false;
            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                description = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                emailText = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                emailSubject = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                setID = parameters.filter.ContainsKey("setID") ? parameters.filter["setID"].ToString() : "";

                if (parameters.filter.ContainsKey("created") && parameters.filter["created"] != null)
                {
                    var dates = parameters.filter["created"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dates.Length > 0)
                    {
                        createdMin = RDL.Convert.StrToDateTime(dates[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                        isFilterDate = true;
                    }
                    if (dates.Length > 1)
                    {
                        createdMax = RDL.Convert.StrToDateTime(dates[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        createdMax = createdMax.AddDays(1).AddSeconds(-1);
                        isFilterDate = true;
                    }
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            var sets = mng.GetUserCheckSets();

            p.Add("name", name);
            p.Add("setID", setID);
            p.Add("isFilterDate", isFilterDate);
            p.Add("description", description);
            p.Add("emailText", emailText);
            p.Add("emailSubject", description);
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetUsersCheckItemsTable", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");

            foreach (var item in items)
            {
                if (item.name == null) item.name = string.Empty;
                if (item.description == null) item.customerText = string.Empty;
                if (item.emailText == null) item.whatToDo = string.Empty;
                if (item.emailSubject == null) item.statusName = string.Empty;
                if (item.created == null) item.created = "Не задано";
                item.set = sets.FirstOrDefault(s => s.id == item.setID).name;
            }

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");

            /*var json = JsonConvert.SerializeObject(new
            {
                items = items.Select(x => new
                {
                    id = item.id,
                    name = item.name != null ? item.name : "Не задано",
                    set = sets.FirstOrDefault(s => s.id == item.setID).name,
                    description = item.description != null ? item.description : "Не задано",
                    emailText = item.emailText != null ? item.emailText : "Не задано",
                    emailSubject = item.emailSubject != null ? item.emailSubject : "Не задано",
                    created = item.created != null ? item.created : "",
                }),
                total = total
            });
            return Content(json, "application/json");*/
        }

        public ActionResult CreateUserCheckItem(string name, string description, string emailText, string emailSubject, int setID)
        {
            var mng = new UserCheckItemManager();

            var item = mng.AddUserCheckItem(name, description, emailText, emailSubject, setID);
            mng.SaveUserCheckItem(item);
            return Json(new
            {
                result = item.id > 0,
                textID = item.id
            });
        }

        public ActionResult UserCheckItemsInline(int pk, string value, string name)
        {
            var mng = new UserCheckItemManager();
            mng.EditRightField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult GetUserCheckItem(int id)
        {
            var mng = new UserCheckItemManager();
            var t = mng.GetUserCheckItem(id);

            return Json(new
            {
                result = true,
                name = t.name,
                description = t.description,
                emailText = t.emailText,
                emailSubject = t.emailSubject,
                setID = t.setID
            });
        }

        public ActionResult UserCheckItems_remove(int id)
        {
            var res = false;
            var mng = new UserCheckItemManager();

            var item = mng.GetUserCheckItem(id);
            if (item != null)
            {
                mng.DeleteUserCheckItem(item);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Материал для рассылки удален!"
            });
        }

        #endregion

        #region UserChecks
        // ------------------------------------- UserChecks -------------------------------------------------------
        public ActionResult UserChecks()
        {
            //var mng = new MenuManager();
            //ViewBag.parentName = mng.GetMenu(true); //список родительских пунктов меню
            return View();
        }

        public ActionResult UserChecks_getItems()
        {
            //var parameters = AjaxModel.GetParameters(HttpContext);

            var mngCheckItems = new UserCheckItemManager();
            IQueryable<as_userCheckItems> checkItems = mngCheckItems.GetUserCheckItems().AsQueryable();

            var mngChecks = new UserCheckManager();

            IQueryable<MembershipUser> users = Membership.GetAllUsers().Cast<MembershipUser>().AsQueryable();

            var res = new List<object>();

            foreach (MembershipUser curUser in users)
            {
                var curUserRolesArray = System.Web.Security.Roles.GetRolesForUser(curUser.UserName);

                foreach (as_userCheckItems curCheckItem in checkItems)
                {
                    string curCheckRolesString = curCheckItem.as_userCheckSet.roles;
                    var curCheckRolesArray = curCheckRolesString.Split(',');

                    var intersect = curUserRolesArray.Intersect(curCheckRolesArray);
                    bool userHasRights = false;
                    bool isClosed = false;
                    bool isSended = false;
                    string modifiedDT = "";

                    if (intersect.Any())
                    {
                        userHasRights = true;
                    }

                    var curCheck = mngChecks.GetUserCheck(curCheckItem.id, curUser.UserName);
                    if (curCheck != null)
                    {
                        isClosed = curCheck.isClosed;
                        modifiedDT = curCheck.modifiedDate.ToLongDateString();
                        isSended = true;
                    }

                    res.Add(new
                    {
                        idCheckItem = curCheckItem.id,
                        nameCheckItem = curCheckItem.name,
                        nameUser = curUser.UserName,
                        modified = modifiedDT,
                        isSended = isSended,
                        isClosed = isClosed,
                        userHasRights = userHasRights

                    });

                }
            }

            return Json(new
            {
                items = res,
                total = res.Count,
                columns = checkItems.Count()

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserChecks_getItem()
        {
            var data = GetAjaxParameters(HttpContext);

            string nameUser = data["nameUser"].ToString();
            int idCheckItem = RDL.Convert.StrToInt(data["idCheckItem"].ToString(), 0);

            var mngChecks = new UserCheckManager();
            bool res = false;
            bool isClosed = false;
            bool isSended = false;
            string modifiedDT = "";


            var curCheck = mngChecks.GetUserCheck(idCheckItem, nameUser);
            if (curCheck != null)
            {
                isClosed = curCheck.isClosed;
                modifiedDT = curCheck.modifiedDate.ToLongDateString();
                isSended = true;
                res = true;
            }

            return Json(new
            {
                modified = modifiedDT,
                isSended = isSended,
                isClosed = isClosed,
                result = res

            }, JsonRequestBehavior.AllowGet);
        }

        public static Dictionary<string, object> GetAjaxParameters(HttpContextBase context)
        {
            var res = new Dictionary<string, object>();
            String jsonString = new StreamReader(context.Request.InputStream).ReadToEnd();
            var js = new JavaScriptSerializer();
            res = js.Deserialize<Dictionary<string, object>>(jsonString);
            return res;
        }


        public ActionResult UserChecks_sendMail()
        {
            bool res = true;

            try
            {
                var data = GetAjaxParameters(HttpContext);

                string emailNote = data["emailNote"].ToString();
                string nameUser = data["nameUser"].ToString();
                string emailText = data["emailText"].ToString();
                string emailSubject = data["emailSubject"].ToString();
                int idCheckItem = RDL.Convert.StrToInt(data["idCheckItem"].ToString(), 0);

                var mngUserCheckItems = new UserCheckItemManager();
                var checkItem = mngUserCheckItems.GetUserCheckItem(idCheckItem);

                MembershipUser user = Membership.GetUser(nameUser);

                if (user != null && checkItem != null)
                {
                    res = SendMail_Mail(user.Email, emailSubject, String.Format("{0}{1}{2}", emailText, Environment.NewLine, emailNote));
                }

                var mngUserChecks = new UserCheckManager();

                as_userChecks curCheck = mngUserChecks.GetUserCheck(idCheckItem, nameUser);

                if (curCheck == null)
                {
                    curCheck = mngUserChecks.AddUserCheck(nameUser, false, emailNote, DateTime.Now, idCheckItem);
                }
                else
                {
                    curCheck.modifiedDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(emailNote))
                    {
                        curCheck.note = emailNote;
                    }
                }

                mngUserChecks.SaveUserCheck(curCheck);

            }
            catch (Exception ex)
            {
                res = false;
                throw;
            }

            return Json(new
            {
                result = res

            }, JsonRequestBehavior.AllowGet);
        }

        private bool SendMail_Mail(string email, string subject, string body)
        {
            var res = true;
            var mng = new CoreManager();
            try
            {
                mng.SendEmail(email, subject, body);
            }
            catch (Exception e)
            {
                res = false;
                RDL.Debug.LogError(e, "SendMail_Mail, user= " + email);
            }
            return res;
        }

        public ActionResult UserChecks_changeIsClosed()
        {
            bool res = true;

            try
            {
                var data = GetAjaxParameters(HttpContext);

                bool isClosed = data["isClosed"].CastTo<bool>();
                string nameUser = data["nameUser"].ToString();
                int idCheckItem = RDL.Convert.StrToInt(data["idCheckItem"].ToString(), 0);

                var mngUserChecks = new UserCheckManager();

                as_userChecks curCheck = mngUserChecks.GetUserCheck(idCheckItem, nameUser);

                if (curCheck == null)
                {
                    curCheck = mngUserChecks.AddUserCheck(nameUser, isClosed, "", DateTime.Now, idCheckItem);
                }
                else
                {
                    curCheck.isClosed = isClosed;
                    curCheck.modifiedDate = DateTime.Now;
                }

                mngUserChecks.SaveUserCheck(curCheck);

            }
            catch (Exception)
            {
                res = false;
                throw;
            }

            return Json(new
            {
                result = res

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UserChecks_CloseColumn()
        {
            bool res = true;

            try
            {

                var data = GetAjaxParameters(HttpContext);

                int idCheckItem = RDL.Convert.StrToInt(data["idCheckItem"].ToString(), 0);
                var mngCheckItems = new UserCheckItemManager();

                var mngUserChecks = new UserCheckManager();
                var existingChecks = mngUserChecks.GetUserChecks().Where(x => x.itemID == idCheckItem);

                as_userCheckItems chkItem = mngCheckItems.GetUserCheckItem(idCheckItem);
                string curCheckRolesString = chkItem.as_userCheckSet.roles;
                var curCheckRolesArray = curCheckRolesString.Split(',');


                IQueryable<MembershipUser> users = Membership.GetAllUsers().Cast<MembershipUser>().AsQueryable();

                foreach (MembershipUser curUser in users)
                {
                    var curUserRolesArray = System.Web.Security.Roles.GetRolesForUser(curUser.UserName);

                    var intersect = curUserRolesArray.Intersect(curCheckRolesArray);
                    bool userHasRights = false;
                    DateTime curDateTime = DateTime.Now;

                    if (intersect.Any())
                    {
                        userHasRights = true;
                    }

                    if (userHasRights)
                    {
                        as_userChecks curCheck = existingChecks.Where(x => x.username.Equals(curUser.UserName)).FirstOrDefault();
                        if (curCheck == null)
                        {
                            curCheck = mngUserChecks.AddUserCheck(curUser.UserName, true, "", curDateTime, idCheckItem);
                        }
                        else
                        {
                            curCheck.isClosed = true;
                            curCheck.modifiedDate = curDateTime;
                        }

                        mngUserChecks.SaveUserCheck(curCheck);
                    }
                }
            }
            catch (Exception)
            {
                res = false;
                throw;
            }

            return Json(new
            {
                result = res

            }, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region TextMistakes

        // /Admin/GetTextMistakesTable
        public RedirectToRouteResult GetTextMistakesTable()
        {
            return RedirectToAction("GetTextMistakesTable", "TextMistakes");
        }


        // /Admin/GetTextMistakesTableLulchenko
        public RedirectToRouteResult GetTextMistakesTableLulchenko()
        {
            return RedirectToAction("GetTable", "LulchenkoTextMistakes");
        }
        #endregion

        #region MailDelivery

        [Authorize]
        public ActionResult Mail()
        {


            var userList = new List<MembershipUser>();
            var users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
            {
                userList.Add(user);
            }
            ViewBag.Users = userList;
            var mng = new MailManager();
            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            ViewBag.Mails = mng.GetMailList();
            return View();
        }
        public ActionResult Mail_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new MailManager();
            var mailList = mng.GetMailLog();
            DateTime dateBegin = DateTime.MinValue;
            DateTime dateEnd = DateTime.MinValue;
            var p = mailList.Select(x => new { id = x.id, date = x.create, author = x.as_mail.userName, subject = x.as_mail.subject, addressee = x.userName, body = x.as_mail.body, mailID = x.mailID, attachment = x.as_mail.as_mailAttachment != null ? x.as_mail.as_mailAttachment.link.ToString() : "" });
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var subject = parameters.filter.ContainsKey("subject") && parameters.filter["subject"].ToString() != "0" ? parameters.filter["subject"].ToString() : "";
                var author = parameters.filter.ContainsKey("author") ? parameters.filter["author"].ToString() : "";
                var addressee = parameters.filter.ContainsKey("addressee") ? parameters.filter["addressee"].ToString() : "";
                var date = parameters.filter.ContainsKey("date") ? parameters.filter["date"].ToString() : DateTime.MinValue.ToString();
                string[] dateRange = date.Split('-');
                if (dateRange.Length > 1)
                {
                    dateBegin = RDL.Convert.StrToDateTime(dateRange[0], DateTime.MinValue);
                    dateEnd = RDL.Convert.StrToDateTime(dateRange[1], DateTime.MinValue);
                }
                if (!String.IsNullOrWhiteSpace(subject))
                {
                    p = p.Where(x => x.subject != null && x.subject.IndexOf(subject, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.body.IndexOf(subject, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
                if (!String.IsNullOrWhiteSpace(author) && author != "0")
                {
                    p = p.Where(x => x.author != null && x.author == author);
                }
                if (!String.IsNullOrWhiteSpace(addressee) && addressee != "0")
                {
                    p = p.Where(x => x.addressee != null && x.addressee == addressee);
                }
                if (dateBegin != null && dateEnd != null && dateBegin != DateTime.MinValue && dateEnd != DateTime.MinValue)
                {

                    p = p.Where(x => x.date.Value.Date >= dateBegin && x.date.Value.Date <= dateEnd);
                }

            }


            //var roles = Roles.GetAllRoles();

            var items = p;
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";
            var temp = items;


            switch (sort1)
            {
                case "date":
                    if (direction1 == "up") items = items.OrderBy(x => x.date);
                    else items = items.OrderByDescending(x => x.date);
                    break;
                case "author":
                    if (direction1 == "up") items = items.OrderBy(x => x.author);
                    else items = items.OrderByDescending(x => x.author);
                    break;
                case "subject":
                    if (direction1 == "up") items = items.OrderBy(x => x.subject);
                    else items = items.OrderByDescending(x => x.subject);
                    break;
                case "addressee":
                    if (direction1 == "up") items = items.OrderBy(x => x.addressee);
                    else items = items.OrderByDescending(x => x.addressee);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.id);
                    else items = items.OrderByDescending(x => x.id);
                    break;
            }

            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();
            return Json(new
            {
                items = res.Select(x => new
                {
                    id = x.id,
                    date = x.date.ToString(),
                    author = x.author,
                    subject = x.subject,
                    addressee = x.addressee,
                    body = x.body,
                    mailID = x.mailID,
                    attach = x.attachment
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Mail_remove(int id)
        {
            bool res = false;
            var mng = new MailManager();
            as_mailLog item = mng.GetMailLog(id);
            if (item != null)
            {
                mng.DeleteMailLogItem(id);
                res = true;
            }
            return Json(new
            {
                result = res,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UsersTable_getItems()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var userList = new List<MembershipUser>();
            string direct = parameters.Where(x => x.Key == "direction").FirstOrDefault().Value.ToString();
            var users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
            {
                userList.Add(user);
            }

            var p = userList.Select(x => new { id = x.ProviderUserKey, name = x.UserName, email = x.Email });
            if (direct == "up") p = p.OrderBy(x => x.name);
            else p = p.OrderByDescending(x => x.name);

            return Json(new
            {
                items = p.Select(x => new
                {
                    id = x.id,
                    name = x.name

                })

            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult RolesTable_getItems()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            string[] roles = System.Web.Security.Roles.GetAllRoles();
            string direct = parameters.Where(x => x.Key == "direction").FirstOrDefault().Value.ToString();
            var items = roles.Select(x => new { role = x.ToString() }).OrderBy(x => x.role);

            if (direct == "up") items = items.OrderBy(x => x.role);
            else items = items.OrderByDescending(x => x.role);


            return Json(new
            {
                items = items.Select(x => new
                {

                    role = x.role

                })

            }, JsonRequestBehavior.AllowGet);

        }
        private as_mail CreateMail(string body, string subject, string attachmentURL, int emailId = 0)
        {
            int attachmentId = 0;
            var mng = new MailManager();
            try
            {
                as_mail item = new as_mail { subject = subject, body = body, create = DateTime.Now, userName = HttpContext.User.Identity.Name };
                if (!String.IsNullOrWhiteSpace(attachmentURL))
                {
                    as_mailAttachment attach = new as_mailAttachment { link = attachmentURL };
                    attachmentId = mng.SaveMailAttachmentItem(attach);
                    item.attachmentID = attachmentId;
                }

                emailId = mng.SaveMailItem(item);
                as_mail emailItem = mng.GetMail(emailId);
                return emailItem;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult SendMail()
        {

            bool isRole = false;
            bool isNew = false;
            string body = "";
            string subject = "";
            string attachmentURL = "";
            int attachmentId = 0;

            int emailId = -1;
            ArrayList items = new ArrayList();
            var mng = new MailManager();
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            bool isRoleRes = bool.TryParse(parameters.Where(x => x.Key == "isRole").FirstOrDefault().Value.ToString(), out isRole);
            bool isNewRes = bool.TryParse(parameters.Where(x => x.Key == "isNew").FirstOrDefault().Value.ToString(), out isNew);
            items = (ArrayList)parameters.Where(x => x.Key == "items").FirstOrDefault().Value;
            body = parameters.Where(x => x.Key == "body").FirstOrDefault().Value.ToString();
            subject = parameters.Where(x => x.Key == "subject").FirstOrDefault().Value.ToString();
            emailId = RDL.Convert.StrToInt(parameters.Where(x => x.Key == "mailID").FirstOrDefault().Value.ToString(), -1);
            attachmentURL = parameters.Where(x => x.Key == "attachmentURL").FirstOrDefault().Value != null ? parameters.Where(x => x.Key == "attachmentURL").FirstOrDefault().Value.ToString() : "";
            attachmentId = RDL.Convert.StrToInt(parameters.Where(x => x.Key == "attachmentID").FirstOrDefault().Value.ToString(), -1);

            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    if (isNew)
                    {
                        if (isRole)
                        {

                            var users = GetUsersByRoles(items);
                            as_mail emailItem = CreateMail(body, subject, attachmentURL);
                            SendMail_SaveLog(emailItem, users);
                            SendMail_Mail(users, subject, body, attachmentURL);
                        }
                        else
                        {
                            as_mail emailItem = CreateMail(body, subject, attachmentURL);
                            SendMail_SaveLog(emailItem, items);
                            SendMail_Mail(items, subject, body, attachmentURL);
                        }
                    }
                    else
                    {
                        if (isRole)
                        {
                            var users = GetUsersByRoles(items);
                            as_mail emailItem = mng.GetMail(emailId);
                            string existAttach = emailItem.as_mailAttachment != null ? emailItem.as_mailAttachment.link : "";
                            if (emailItem.body != body || emailItem.subject != subject || existAttach != attachmentURL)
                            {
                                as_mail emailItem2 = CreateMail(body, subject, attachmentURL);
                                SendMail_SaveLog(emailItem2, users);
                                SendMail_Mail(users, subject, body, attachmentURL);
                            }
                            else
                            {
                                SendMail_SaveLog(emailItem, users);
                                SendMail_Mail(users, subject, body, attachmentURL);
                            }
                        }
                        else
                        {
                            as_mail emailItem = mng.GetMail(emailId);
                            string existAttach = emailItem.as_mailAttachment != null ? emailItem.as_mailAttachment.link : "";
                            if (emailItem.body != body || emailItem.subject != subject || existAttach != attachmentURL)
                            {
                                as_mail emailItem2 = CreateMail(body, subject, attachmentURL);
                                SendMail_SaveLog(emailItem2, items);
                                SendMail_Mail(items, subject, body, attachmentURL);
                            }
                            else
                            {
                                SendMail_SaveLog(emailItem, items);
                                SendMail_Mail(items, subject, body, attachmentURL);
                            }
                        }
                    }
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else { return Json(new { status = false }, JsonRequestBehavior.AllowGet); }
        }

        private ArrayList GetUsersByRoles(ArrayList roles)
        {
            ArrayList users = new ArrayList();
            foreach (var role in roles)
            {
                string[] temp = System.Web.Security.Roles.GetUsersInRole(role.ToString());
                for (int i = 0; i < temp.Length; i++)
                {
                    var userKey = Membership.GetUser(temp[i]).ProviderUserKey;
                    if (!users.Contains(userKey))
                    {
                        users.Add(userKey);
                    }
                }
            }

            return users;
        }
        private void SendMail_SaveLog(as_mail mail, ArrayList usersKeys)
        {
            var mng = new MailManager();
            List<System.Net.Mail.MailAddress> emails = new List<System.Net.Mail.MailAddress>();
            List<as_mailLog> mailingLog = new List<as_mailLog>();
            if (mail != null)
            {
                foreach (var i in usersKeys)
                {
                    MembershipUser user = Membership.GetUser(RDL.Convert.StrToGuid(i.ToString(), Guid.Empty));
                    if (user != null)
                    {
                        string m = user.Email;
                        emails.Add(new System.Net.Mail.MailAddress(m));
                        mailingLog.Add(new as_mailLog { userName = user.UserName, create = DateTime.Now, mailID = mail.id });
                    }
                }

                mng.SaveMailLogList(mailingLog);
            }
        }
        private bool SendMail_Mail(ArrayList usersKeys, string subject, string body, string attach = "")
        {
            var res = true;
            var mng = new CoreManager();


            foreach (var i in usersKeys)
            {
                MembershipUser user = Membership.GetUser(RDL.Convert.StrToGuid(i.ToString(), Guid.Empty));
                if (user != null)
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(attach))
                        {
                            mng.SendEmail(user.Email, subject, body);
                        }
                        else
                        {
                            mng.SendEmail(user.Email, subject, body, Server.MapPath(attach));
                        }
                    }
                    catch (Exception e)
                    {
                        res = false;
                        RDL.Debug.LogError(e, "SendMail_Mail, user= " + user.Email);
                    }
                }
            }
            return res;
        }

        #endregion

        #region cl ListItems

        public ActionResult clListItems()
        {
            var mng = new CRMManager();
            ViewBag.Lists = mng.GetLists();
            return View();
        }

        public ActionResult clListItems_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            int ord = -1;
            var listID = 0;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                listID = parameters.filter.ContainsKey("listName") ? RDL.Convert.StrToInt(parameters.filter["listName"].ToString(), 0) : 0;
                p.Add("listID", listID);

                ord = parameters.filter.ContainsKey("ord") ? RDL.Convert.StrToInt(parameters.filter["ord"].ToString(), -1) : -1;
                p.Add("ord", ord);
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("cl_GetListItemsTable", p, CommandType.StoredProcedure);
            var total = p.Get<int>("total");

            foreach (var item in items)
            {
                if (item.name == null) item.name = string.Empty;
            }

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult clListItemsInline(int pk, string value, string name)
        {
            var mng = new CRMManager();
            mng.clEditListItemField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult clListItemsCreate(string name, int listID, int ord)
        {
            var mng = new CRMManager();
            var item = new cl_listItems
            {
                id = 0,
                name = name,
                listID = listID,
                ord = ord
            };
            mng.clSaveListItem(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult clListItems_remove(int id)
        {
            var res = false;
            var mng = new CRMManager();
            var item = mng.clGetListItem(id);
            var msg = "";
            if (item != null)
            {
                mng.clDeleteListItem(id);
                msg = "List Item удален!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        #endregion

        #region cl Lists

        public ActionResult clLists()
        {
            CRMManager mng = new CRMManager();
            ViewBag.Users = mng.GetUserNames();
            return View();
        }

        public ActionResult clLists_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();
            IQueryable<cl_lists> items = mng.clGetLists().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {

            }

            var res = items;

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;

                case "code":
                    if (direction1 == "up") res = res.OrderBy(x => x.code);
                    else res = res.OrderByDescending(x => x.code);
                    break;

                default:
                    if (direction1 == "up") res = res.OrderBy(x => x.name);
                    else res = res.OrderByDescending(x => x.name);
                    break;
            }
            var total = res.Count();
            var res2 = res.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            List<string> listUsers = mng.GetUserNames();

            var res3 = res2.Select(x => new
               {
                   x.id,
                   name = x.name ?? "",
                   code = x.code ?? "",
                   users = x.users.Replace(",", "<br>"),
                   usersID = GetCheckedItems(listUsers, x.users),
                   roles = "<div class='usUserRoles'>" + x.roles + "</div>"
               });

            return Json(new
            {
                items = res3,
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetCheckedItems(List<string> list, string s)
        {
            var res = string.Empty;
            var checkedItems = s.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            foreach (var item in list)
            {
                foreach (var item2 in checkedItems)
                {
                    if (item == item2)
                        res += Convert.ToInt32(i) + ",";
                }
                i++;
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        public ActionResult clListInline(int pk, List<string> value, string name)
        {
            var mng = new CRMManager();
            string res = string.Empty;
            switch (name)
            {
                case "users":
                    {
                        List<string> listUsers = mng.GetUserNames();
                        string users = string.Empty;
                        foreach (var item in value)
                        {
                            users += listUsers[Convert.ToInt32(item)] + ",";
                        }
                        res = users.Substring(0, users.Length - 1);
                        break;
                    }
                case "name":
                    {
                        res = value[0];
                        break;
                    }
                case "code":
                    {
                        res = value[0];
                        break;
                    }
            }
            mng.clEditListField(pk, name, res);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult ChangeRoleForList(int pk, string roles)
        {
            var mng = new CRMManager();
            mng.ChangeRoleForList(pk, roles);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult clListCreate(string name, string code, string users, string roles)
        {
            var mng = new CRMManager();
            var item = new cl_lists
            {
                id = 0,
                name = name,
                code = code,
                users = users.Replace("<br>", ","),
                roles = roles.Replace("<br>", ",")
            };
            mng.clSaveList(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult clLists_remove(int id)
        {
            var res = false;
            var mng = new CRMManager();
            var item = mng.clGetList(id);
            var msg = "";
            if (item != null)
            {
                mng.clDeleteList(id);
                msg = "List удален!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        #endregion

        #region ProcessItem

        public ActionResult ProcessItem()
        {
            var mng = new BLL.ProcessItem.ProcessItemManager();
            ViewBag.Items = mng.GetProcessList();
            ViewBag.Users = mng.GetUserNames();
            return View();
        }

        public ActionResult ProcessItem_getItems()
        {
            var mng = new BLL.ProcessItem.ProcessItemManager();
            var parameters = AjaxModel.GetParameters(HttpContext);
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            int ord = -1;
            var processID = 0;
            var name = "";
            var desc = "";

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                processID = parameters.filter.ContainsKey("processName") ? RDL.Convert.StrToInt(parameters.filter["processName"].ToString(), 0) : 0;
                ord = parameters.filter.ContainsKey("ord") ? RDL.Convert.StrToInt(parameters.filter["ord"].ToString(), -1) : -1;
                name = parameters.filter.ContainsKey("nameTask") ? parameters.filter["nameTask"].ToString() : "";
                desc = parameters.filter.ContainsKey("desc") ? parameters.filter["desc"].ToString() : "";
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";

            p.Add("processID", processID);
            p.Add("ord", ord);
            p.Add("name", name);
            p.Add("desc", desc);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("GetProcessItems", p, CommandType.StoredProcedure);
            List<string> listUsers = mng.GetUserNames();
            var total = p.Get<int>("total");

            foreach (var item in items)
            {
                if (item.name == null) item.name = string.Empty;
                item.usersID = GetCheckedItemsUser(listUsers, item.users);
                item.users = item.users.Replace(",", "<br>");
                item.roles = "<div class='usUserRoles'>" + item.roles + "</div>";
            }

            var json = JsonConvert.SerializeObject(new
            {
                items = items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult ProcessItemCreate(string name, int processID, int ord, string roles, string desc, string users, bool isFinish, string color)
        {
            var mng = new BLL.ProcessItem.ProcessItemManager();
            var item = new proc_processItems
            {
                id = 0,
                processID = processID,
                name = name,
                ord = ord,
                desc = desc,
                roles = roles.Replace("<br>", ","),
                users = users.Replace("<br>", ","),
                isFinish = isFinish,
                color = color
            };
            mng.SaveProcessItem(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult processItemInline(int pk, List<string> value, string name)
        {
            var mng = new BLL.ProcessItem.ProcessItemManager();
            mng.processEditListField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        private string GetCheckedItemsUser(List<string> list, string s)
        {
            var res = string.Empty;
            var checkedItems = s.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            foreach (var item in list)
            {
                foreach (var item2 in checkedItems)
                {
                    if (item == item2)
                        res += Convert.ToInt32(i) + ",";
                }
                i++;
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }

        public ActionResult ProcessItem_remove(int id)
        {
            var res = false;
            var mng = new BLL.ProcessItem.ProcessItemManager();
            var item = mng.GetProcessItem(id);
            var msg = "";
            if (item != null)
            {
                mng.DeleteProcessItem(id);
                msg = "Item удален!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        public ActionResult ChangeRoleForProcessItem(int pk, string roles)
        {
            var mng = new BLL.ProcessItem.ProcessItemManager();
            mng.ChangeRoleForProcessItem(pk, roles);

            return Json(new
            {
                result = true
            });
        }

        #endregion

        #region Calc_Parameters
        public ActionResult CalcParams()
        {

            var mng = new CalcManager();
            ViewBag.CalcCalcs = mng.GetCalcs();
            ViewBag.CalcDataTypes = mng.GetDataTypes();

            return View();
        }

        public ActionResult CalcParams_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);

            var calcID = "";
            var name = "";
            var code = "";
            var datatypeID = "";

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                calcID = parameters.filter.ContainsKey("calcID") ? parameters.filter["calcID"].ToString() : "0";
                name = parameters.filter.ContainsKey("name") ? parameters.filter["name"].ToString() : "";
                code = parameters.filter.ContainsKey("code") ? parameters.filter["code"].ToString() : "";
                datatypeID = parameters.filter.ContainsKey("datatypeID") ? parameters.filter["datatypeID"].ToString() : "0";
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("calcID", calcID);
            p.Add("name", name);
            p.Add("code", code);
            p.Add("datatypeID", datatypeID);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = rep.GetSQLData<dynamic>("GetCalcParams", p, CommandType.StoredProcedure) as List<dynamic> ?? new List<dynamic>();

            var total = p.Get<int>("total");

            var json = JsonConvert.SerializeObject(new
            {
                items,
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult CreateCalcParam(int calcID, string name, string code, int datatypeID)
        {

            var mng = new CalcManager();
            var item = new calc_parameters
            {
                id = 0,
                calcID = calcID,
                name = name,
                code = code,
                datatypeID = datatypeID,
            };
            mng.SaveCalcParameter(item);
            return Json(new
            {
                result = item.id > 0,
                savedID = item.id,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCalcParam(int id)
        {
            var mng = new CalcManager();
            var item = mng.GetCalcParameter(id);
            return Json(new
            {
                id = item.id,
                calcID = item.calcID,
                name = item.name,
                code = item.code,
                datatypeID = item.datatypeID,
            });
        }

        public ActionResult CalcParamsInline(int pk, string value, string name)
        {
            var mng = new CalcManager();
            mng.EditCalcField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult CalcParams_remove(int id)
        {
            var res = false;
            var mng = new CalcManager();

            var item = mng.GetCalcParameter(id);
            if (item != null)
            {
                mng.DeleteCalcParameter(id);
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = "Запись удалена!"
            });
        }
        #endregion
    }
}