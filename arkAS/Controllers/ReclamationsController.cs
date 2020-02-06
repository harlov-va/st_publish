using arkAS.BLL;
using arkAS.BLL.Core;
using arkAS.BLL.CRM;
using arkAS.Models;
using arkAS.Models.Reclamations;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class ReclamationsController : Controller
    {
        public ActionResult ReclamationsTable()
        {
            var mng = new CRMManager();
            var haveWOW = new List<string>();
            haveWOW.Add(":)");
            ViewBag.Statuses = mng.GetReclamationStatuses();
            ViewBag.Projects = mng.GetProjects();
            ViewBag.haveWOW = haveWOW;
            return View();
        }
        
        
        //#region Admin
        //[Authorize(Roles = "admin")]
        public ActionResult ReclamationsTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new CRMManager();

            var addedBy = "";
            //var clientID = 0;
            List<int?> statusIDs = new List<int?>();
            List<int?> projectIDs = new List<int?>();

            DateTime createdMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            DateTime createdMinRep = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            DateTime createdMaxRep = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            bool isFilterDate = false;
            bool isFilterDateRep = false;

            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                addedBy = parameters.filter.ContainsKey("addedBy") ? parameters.filter["addedBy"].ToString() : ""; // фильтр для текстового поля
                //statusIDs = parameters.filter.ContainsKey("statusName") ? (int?)RDL.Convert.StrToInt(parameters.filter["statusName"].ToString(), 0) : 0; // для селекта
                statusIDs = new List<int?>();
                if (parameters.filter.ContainsKey("statusName"))
                {
                    statusIDs = (parameters.filter["statusName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0) == 0 ? null : (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList(); // для checkboxes
                }

                if (parameters.filter.ContainsKey("projectName"))
                {
                    projectIDs = (parameters.filter["projectName"] as ArrayList).ToArray().Select(x => (int?)RDL.Convert.StrToInt(x.ToString(), 0) == 0 ? null : (int?)RDL.Convert.StrToInt(x.ToString(), 0)).ToList();
                }

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
                        isFilterDate = true;
                    }
                }

                if (parameters.filter.ContainsKey("reportDate") && parameters.filter["reportDate"] != null)
                {
                    var datesRep = parameters.filter["reportDate"].ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (datesRep.Length > 0)
                    {
                        createdMinRep = RDL.Convert.StrToDateTime(datesRep[0].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                        isFilterDateRep = true;
                    }
                    if (datesRep.Length > 1)
                    {
                        createdMaxRep = RDL.Convert.StrToDateTime(datesRep[1].Trim(), (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue);
                        isFilterDateRep = true;
                    }
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";
            var sort2 = sorts.Length > 1 ? sorts[1] : "";
            var direction2 = directions.Length > 1 ? directions[1] : "";

            var rep = new CoreRepository();
            var p = new DynamicParameters();
            p.Add("addedBy", addedBy);
            //p.Add("clientID", clientID);
            p.Add("statusIDs", String.Join(",", statusIDs));
            p.Add("projectIDs", String.Join(",", projectIDs));
            p.Add("createdMin", createdMin);
            p.Add("createdMax", createdMax);
            p.Add("createdMinRep", createdMinRep);
            p.Add("createdMaxRep", createdMaxRep);
            p.Add("isFilterDate", isFilterDate);
            p.Add("isFilterDateRep", isFilterDateRep);
            p.Add("sort1", sort1);
            p.Add("direction1", direction1);
            p.Add("sort2", sort2);
            p.Add("direction2", direction2);
            p.Add("page", parameters.page);
            p.Add("pageSize", parameters.pageSize);
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var items = GetSQLData<ReclamationItem>("GetReclamationsTable", p, CommandType.StoredProcedure);

            var total = p.Get<int>("total");
            var json = JsonConvert.SerializeObject(new
            {
                items = items.Select(x => new
                {
                    x.id,
                    created = x.created != null ? x.created : "Не задано" ,
                    addedBy = x.addedBy != null ? x.addedBy : "",
                    name = x.name != null ? x.name : "",
                    reportDate = x.reportDate != null ? x.reportDate : "Не задано",
                    statusName = x.statusName != null ? x.statusName : "",
                    projectName = x.projectName != null ? x.projectName : "",
                    haveWOW = x.haveWOW ? ":)" : ""
                }),
                total = total
            });
            return Content(json, "application/json");
        }

        public ActionResult ReclamationsTable_remove(int id)
        {
            var res = false;
            var msg = "";
            var mng = new CRMManager();
            var item = mng.GetReclamation(id);
            
            if (item != null)
            {
                mng.DeleteReclamation(id);
                msg = "Запись успешно удалена!";
                res = true;
            }

            return Json(new
            {
                result = res,
                msg = msg
            });
        }

        public ActionResult ReclamationsTableInline(int pk, string value, string name)
        {
            var mng = new CRMManager();
            mng.EditReclamationField(pk, name, value);
            return Json(new
            {
                result = true
            });
        }

        public ActionResult ReclamationsSaveItem(string addedBy, string name, /*string ord,*/ string projectName)
        {
            var res = false;
            try
            {
                var mng = new CRMManager();

                var item = new recl_items
                {
                    id = 0,
                    created = DateTime.Now,
                    addedBy = addedBy != "" ? addedBy : null,
                    name = name != "" ? name : null,
                    projectID = (int?)RDL.Convert.StrToInt(projectName, 0) != 0 ? (int?)RDL.Convert.StrToInt(projectName, 0) : null,
                };
                mng.SaveReclamation(item);
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReclamationsGetText(int id, string code)
        {
            var res = false;
            var text = "";
            var mng = new CRMManager();
            var r = new recl_items();
            try
            {
                r = mng.GetReclamation(id);
                switch (code) { 
                    case "customerText": text = r.customerText; break;
                    case "whatToDo": text = r.whatToDo; break;
                    case "report": text = r.report; break;
                }
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return Json(new
            {
                result = res,
                text = text
            });
        }

        private IEnumerable<T> GetSQLData<T>(string sql, object parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure);
                    return (IEnumerable<T>)els;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(IEnumerable<T>);
            }
        }

        //#endregion
    }
}