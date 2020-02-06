using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.HR;
using arkAS.Models;
using System.Web.Security;
using System.Collections;
using arkAS.BLL.CRM;

namespace arkAS.Controllers
{
    public class HRController : Controller
    {
        // GET: HR
        public ActionResult Index()
        {

            var mng = new HRManager();
            ViewBag.statusName = mng.GetHumanStatuses();
            ViewBag.sourceName = mng.GetHumanSources();
            return View();
        }

        public ActionResult Humans_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new HRManager();
            var items = mng.GetHumans().AsQueryable();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";

                var sourceID = parameters.filter.ContainsKey("sourceName") ? RDL.Convert.StrToInt(parameters.filter["sourceName"].ToString(), 0) : 0;
                items = items.Where(x =>
                    (sourceID == 0 || x.sourceID == sourceID)
                );

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


                if (text != "")
                {
                    items = items.ToList().Where(x =>
                        x.fio != null && x.fio.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.city != null && x.city.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.note != null && x.note.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.username != null && x.username.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.addedBy != null && x.addedBy.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.pay != null && x.pay.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0
                        ).AsQueryable();
                }
            }

            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            IOrderedQueryable<hr_humans> orderedItems = items.OrderByDescending(p => p.created);

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
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.hr_statuses.name);
                    else orderedItems = items.OrderByDescending(x => x.hr_statuses.name);
                    break;
                case "sourceName":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.hr_sources.name);
                    else orderedItems = items.OrderByDescending(x => x.hr_sources.name);
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
                case "pay":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.pay);
                    else orderedItems = items.OrderByDescending(x => x.pay);
                    break;
                case "hourRate":
                    if (direction1 == "up") orderedItems = items.OrderBy(x => x.hourRate);
                    else orderedItems = items.OrderByDescending(x => x.hourRate);
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
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.hr_statuses.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.hr_statuses.name);
                        break;
                    case "sourceName":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.hr_sources.name);
                        else orderedItems = orderedItems.ThenByDescending(x => x.hr_sources.name);
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
                    case "pay":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.pay);
                        else orderedItems = orderedItems.ThenByDescending(x => x.pay);
                        break;
                    case "hourRate":
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.hourRate);
                        else orderedItems = orderedItems.ThenByDescending(x => x.hourRate);
                        break;

                    default:
                        if (direction2 == "up") orderedItems = orderedItems.ThenBy(x => x.created);
                        else orderedItems = orderedItems.ThenByDescending(x => x.created);
                        break;
                }
            }

            var total = orderedItems.Count();
            var res2 = orderedItems.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res2.Select(x => new
                {
                    x.id,
                    fio = x.fio ?? "",
                    city = x.city ?? "",
                    note = x.note ?? "",
                    x.sourceID,
                    sourceName = x.hr_sources != null ? x.hr_sources.name : "",
                    x.statusID,
                    statusName = x.hr_statuses != null ? x.hr_statuses.name : "",
                    addedBy = x.addedBy ?? "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                    subchannel = x.subchannel ?? "",
                    username = x.username ?? "",
                    needActive = x.needActive.HasValue && x.needActive == true ? "Да" : "Нет",
                    pay = x.pay ?? "",
                    hourRate = x.hourRate ?? 0
                }),
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateHuman(string fio, string note, int sourceID, int statusID)
        {
            var mng = new HRManager();

            int? sourceID_ = null;
            if (sourceID != 0) sourceID_ = sourceID;

            int? statusID_ = null;
            if (statusID != 0) statusID_ = statusID;

            var item = new hr_humans
            {
                id = 0,
                fio = fio,
                city = null,
                note = note,
                sourceID = sourceID_,
                statusID = statusID_,
                addedBy = null,
                created = DateTime.Now,
                subchannel = null,
                username = null,
                needActive = null,
                sourceGuid = null,
                pay = null,
                hourRate = null
            };
            mng.SaveHuman(item);

            return Json(new
            {
                result = item.id > 0,
                id = item.id
            });
        }

        public ActionResult HumansInline(int pk, string value, string name)
        {
            var mng = new HRManager();
            mng.EditHumanField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Humans_remove(int id)
        {
            var res = false;
            var mng = new HRManager();
            var item = mng.GetHuman(id);
            var msg = "";
            if (item != null)
            {
                mng.DeleteHuman(id);
                msg = "HR удален!";
                res = true;
            }
            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        public ActionResult Humans_getComments(int itemID)
        {
            var mng = new CommentManager();
            var res = true;
            return Json(new
            {
                result = res,
                items = mng.GetComments("humans", itemID.ToString()).Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    x.username,
                    x.text
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Humans_addComment(string itemID, string text)
        {
            var mng = new CommentManager();
            var item = mng.AddComment("humans", itemID, text);

            var res = true;
            return Json(new
            {
                result = res,
                item = new
                {
                    item.id,
                    created = item.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    item.username,
                    item.text
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statuses()
        {
            return View();
        }

        public ActionResult Statuses_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new HRManager();
            var items = mng.GetHumanStatuses();
            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    x.name,
                    x.state,
                    x.color,
                    x.code
                }),
                total = items.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statuses_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new HRManager();
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newStatuses = new hr_statuses
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    color = AjaxModel.GetValueFromSaveField("color", fields),
                    name = AjaxModel.GetValueFromSaveField("name", fields),
                    code = AjaxModel.GetValueFromSaveField("code", fields),
                    state = AjaxModel.GetValueFromSaveField("state", fields)
                };

                mng.SaveHumanStatus(newStatuses);
                return Json(new
                {
                    result = true,
                    id = newStatuses.id,
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

        public ActionResult Statuses_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new HRManager();
            try
            {
                if (mng.GetHumanStatus(int.Parse(id)).hr_humans.Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Статус связан с сотрудником, сначало требуется снять данный статус со всех сотрудников"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeleteHumanStatus(int.Parse(id));
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

        public ActionResult Spec()
        {
            return View();
        }

        public ActionResult Spec_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new HRManager();
            var items = mng.GetSpecs();
            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    name = x.name,
                    code = x.code
                }),
                total = items.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Spec_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new HRManager();
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newSpec = new hr_specializations
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = AjaxModel.GetValueFromSaveField("name", fields),
                    code = AjaxModel.GetValueFromSaveField("code", fields)
                };
                mng.SaveSpec(newSpec);
                return Json(new
                {
                    result = newSpec.id > 0,
                    id = newSpec.id,
                    msg = (newSpec.id > 0) ? "Операция успешна" : "Ошибка"
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

        public ActionResult Spec_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new HRManager();
            try
            {
                if (mng.IsSpecInUse(int.Parse(id)))
                {
                    return Json(new
                    {
                        result = false,
                        msg = "Специализация связана с сотрудниками, сначало требуется удалить данную специализацию у сотрудников"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeleteSpec(int.Parse(id));
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

        public ActionResult HumanSpec()
        {
            var mng = new HRManager();
            ViewBag.Spec = mng.GetSpecs();
            return View();
        }

        public ActionResult HSpec_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new HRManager();
            var items = mng.GetHumanSpecs();
            return Json(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    fio = x.fio,
                    specIds = "<div class='usHSpec'>" + String.Join(",", x.hr_humanSpecializations.Select(y => ((y.specializationID != null) ? y.specializationID.ToString() : "")).ToArray()) + "</div>"
                }),
                total = items.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HSpecInline(int pk, string value)
        {
            try
            {
                var mng = new HRManager();
                var idSpec = int.Parse(value.Split('=')[0]);
                var valSpec = bool.Parse(value.Split('=')[1]);
                if (valSpec)
                {
                    mng.SaveHumanSpec(pk, idSpec);
                }
                else
                {
                    mng.DeleteHumanSpec(pk, idSpec);
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
    }
}