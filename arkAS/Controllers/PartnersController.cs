using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.Models;
using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.Partners;
using System.Web.Security;
using System.Collections;
using Dapper;
using System.Data;
using Newtonsoft.Json;
using System.Data;
using System.IO;

namespace arkAS.Controllers
{
    public class PartnersController : Controller
    {
        public ActionResult Index()
        {
            var mng = new PartnersManager();
            ViewBag.statusName = mng.GetPartnerStatuses();
            ViewBag.specsName = mng.GetPartnersSpec();

            return View();
        }

        public ActionResult Partners_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new PartnersManager();
            var rep = new CoreRepository();
            var p = new DynamicParameters();
            var specialization = mng.GetPartnersSpec();

            List<int?> statusIDs = new List<int?>();
            List<int?> specsIDs = new List<int?>();

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                p.Add("text", parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString().Trim() : string.Empty);
                if (parameters.filter.ContainsKey("statusIDs"))
                    statusIDs = parameters.filter["statusIDs"].ToString() != string.Empty ? parameters.filter["statusIDs"].ToString().Split(',').Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList() : null;

                if (parameters.filter.ContainsKey("specIDs"))
                    specsIDs = parameters.filter["specIDs"].ToString() != string.Empty ? parameters.filter["specIDs"].ToString().Split(',').Select(x => (int?)RDL.Convert.StrToInt(x, 0)).ToList() : null;
            }

            string[] sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string sort1 = sorts.Length > 0 ? sorts[0] : "";
            string[] directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string direction1 = directions.Length > 0 ? directions[0] : "";
            
            if (specsIDs != null) p.Add("specsIDs", String.Join(",", specsIDs));
            else p.Add("specsIDs", String.Join(",", string.Empty));

            if (statusIDs != null) p.Add("statusIDs", String.Join(",", statusIDs));
            else p.Add("statusIDs", String.Join(",", string.Empty));

            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var items = rep.GetSQLData<dynamic>("[GetPartners]", p, CommandType.StoredProcedure);
            var total = p.Get<object>("total");

            var res = new List<object>();

            foreach (var group in (items as List<dynamic>).GroupBy(x => x.partnerID))
            {
                var partner = group.FirstOrDefault();
                var specs = string.Empty;
                

                foreach(var item in specialization)
                    specs += string.Format("<input type='checkbox' class='specialization' " + (group.FirstOrDefault(x=>(int)x.specsID == item.id) != null ? "checked": string.Empty) + " specID='" + item.id + "' id='spec" + item.id + "' /><label for='spec" + item.id + "'>{0}</label><br />", item.name);
                res.Add(new
                {
                    partnerID = group.Key,
                    partner.fio,
                    partner.statusName,
                    partner.url,
                    partner.desc,
                    partner.experience,
                    partner.technologies,
                    partner.conditions,
                    specs = specs
                });

            }

            var json = JsonConvert.SerializeObject(new
            {
                items = res,
                total = res.Count
            });
            return Content(json, "application/json");
 
        }

        public ActionResult CreatePartner(string fio, string url, string desc, string experience, string technologies, string conditions, int[] specID, int statusID)
        {
            var mng = new PartnersManager();

            int? statusID_ = null;
            if (statusID != 0) statusID_ = statusID;

            var partner = new ps_partners
            {
                id = 0,
                fio = fio,
                url = url,
                desc = desc,
                specID = null,
                statusID = statusID_,
                experience = experience,
                technologies = technologies,
                conditions = conditions,
            };

            mng.SavePartner(partner);

            foreach (int _specID in specID)
            {
                var item = new ps_specsPartners
                {
                    id = 0,
                    specID = _specID,
                    partnerID = partner.id
                };

                mng.SavePartnersSpecList(item);
            }
            
            return Json(new
            {
                result = partner.id > 0,
                partnerID = partner.id
            });
        }

        public ActionResult PartnersInline(int pk, string value, string name)
        {
            var mng = new PartnersManager();
            mng.EditPartnerField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult Partners_remove(int id)
        {
            var res = false;
            var mng = new PartnersManager();
            var item = mng.GetPartner(id);
            var msg = "";
            if (item != null)
            {
                mng.DeletePartner(id);
                msg = "Партнер удален!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        public ActionResult PartnersStatuses()
        {
            return View();
        }

        public ActionResult PartnersStatuses_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new PartnersManager();
            var items = mng.GetPartnerStatuses();

            var res = items.Select(item => new ps_statuses
            {
                id = item.id,
                name = item.name.Trim(),
                code = item.code.Trim(),
                color = item.color.Trim(),
                order = item.order
            }).AsQueryable();

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
                case "color":
                    if (direction1 == "up") res = res.OrderBy(x => x.color);
                    else res = res.OrderByDescending(x => x.color);
                    break;
                case "order":
                    if (direction1 == "up") res = res.OrderBy(x => x.order);
                    else res = res.OrderByDescending(x => x.order);
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
                    x.name,
                    x.code,
                    x.color,
                    x.order
                }),
                total = items.Count

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartnersStatuses_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new PartnersManager();
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newPartnerStatus = new ps_statuses
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = (AjaxModel.GetValueFromSaveField("name".Trim(), fields)),
                    code = (AjaxModel.GetValueFromSaveField("code".Trim(), fields)),
                    color = (AjaxModel.GetValueFromSaveField("color".Trim(), fields)),
                    order = (AjaxModel.GetValueFromSaveField("order".Trim(), fields))
                    
                };

                mng.SavePartnerStatus(newPartnerStatus);
                return Json(new
                {
                    result = true,
                    id = newPartnerStatus.id,
                    mng = "Операция прошла успешно"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    id = 0,
                    mng = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult PartnersStatuses_remove(string id)
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new PartnersManager();

            try
            {
                if (mng.GetPartnersStatus(int.Parse(id)).ps_partners.Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        mng = "Статус связан с партнером, сначало требуется снять данный статус со всех партнеров"

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeletePartnerStatus(int.Parse(id));
                    return Json(new
                    {
                        result = true,
                        mng = "Оперция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    mng = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PartnersSpec()
        {
            return View();
        }

        public ActionResult PartnersSpec_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new PartnersManager();
            var items = mng.GetPartnersSpec();

            var res = items.Select(item => new ps_statuses
            {
                id = item.id,
                name = item.name,
            }).AsQueryable();

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
                    x.name,
                }),
                total = items.Count

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartnersSpec_save()
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new PartnersManager();
            try
            {
                var fields = (parameters["fields"] as ArrayList).ToArray().ToList().Select(x => x as Dictionary<string, object>).ToList();
                var newPartnerSpec = new ps_specs
                {
                    id = (AjaxModel.GetValueFromSaveField("id", fields) == "") ? 0 : int.Parse(AjaxModel.GetValueFromSaveField("id", fields)),
                    name = (AjaxModel.GetValueFromSaveField("name", fields)),
                };

                mng.SavePartnersSpec(newPartnerSpec);
                return Json(new
                {
                    result = true,
                    id = newPartnerSpec.id,
                    mng = "Операция прошла успешно"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    id = 0,
                    mng = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }

        }
  
        public ActionResult PartnersSpec_remove(string id)      ////
        {
            var parameters = AjaxModel.GetAjaxParameters(HttpContext);
            var mng = new PartnersManager();

            try
            {
                if (mng.GetPartnersSpec(int.Parse(id)).ps_specsPartners.Count > 0)
                {
                    return Json(new
                    {
                        result = false,
                        mng = "Статус связан с партнером, сначало требуется снять данный статус со всех партнеров"

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mng.DeletePartnersSpec(int.Parse(id));
                    return Json(new
                    {
                        result = true,
                        mng = "Оперция успешна"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return Json(new
                {
                    result = false,
                    mng = "Ошибка"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PartnersComments()
        {
            var mng = new PartnersManager();
            ViewBag.Statuses = mng.GetPartnerStatuses();
            //ViewBag.Clients = mng.GetClients();
            return View();
        }
 
        public ActionResult Partners_getComments(int itemID)
        {
            var mng = new CommentManager();
            var res = true;

            return Json(new
            {
                result = res,
                items = mng.GetComments("customer", itemID.ToString()).Select(x => new
                {
                    x.id,
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    x.username,
                    x.text,
                    audio = PathMap(x.audio)
                })
            }, JsonRequestBehavior.AllowGet);
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

        public static void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }

        public ActionResult Partners_getAudio(int id)
        {
            var mng = new CommentManager();
            string path = mng.GetComment(id).audio;
            return File(path, "audio/vnd.wave", "voice_" + id + Path.GetExtension(path));
        }

        public ActionResult Partners_addComment(string itemID, string text, HttpPostedFileBase audioBlob)
        {
            var mng = new CommentManager();

            string audio = string.Empty;

            if (audioBlob != null)
            {
                var folderPath = System.Web.HttpContext.Current.Server.MapPath(@"/uploads/comm_voice");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                audio = Path.Combine(folderPath, Guid.NewGuid() + ".mp3");

                //http://stackoverflow.com/questions/20088743/mvc4-app-unable-to-load-dll-libmp3lame-32-dll
                CheckAddBinPath();

                using (var writer = new NAudio.Lame.LameMP3FileWriter(audio, new NAudio.Wave.WaveFormat(44100, 16, 2), NAudio.Lame.LAMEPreset.STANDARD))
                {
                    audioBlob.InputStream.CopyTo(writer);
                }

            }

            var item = mng.AddComment("customer", itemID, text, audio);

            var res = true;
            return Json(new
            {
                result = res,
                item = new
                {
                    item.id,
                    created = item.created.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm"),
                    item.username,
                    item.text,
                    audio = PathMap(item.audio)
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePartnersSpecs(int specID, int partnerID, bool turnOn)
        {
            var res = false;
            var mng = new PartnersManager();
            try
            {
                if (turnOn)
                {
                    mng.addPartnersSpecs(specID, partnerID);
                    res = true;
                }
                else
                    mng.removePartnersSpecs(specID, partnerID);
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
 
        

    }
}
