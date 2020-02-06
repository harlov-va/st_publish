using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.Areas.harlov.BLL;
using Newtonsoft.Json;
using arkAS.Models;
using System.Web.Security;
using arkAS.BLL;

namespace arkAS.Areas.harlov.Controllers
{
    [Authorize(Roles = "Admin, Guest")]
    public class ContragentController : BaseController
    {
        public ContragentController(IManager mng) : base(mng)
        {

        }
        public ActionResult ContragentsList()
        {
            return View();
        }

        public ActionResult ContragentsList_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var user = mng.GetUser();
            string msg = "";
            var items = mng.Contragents.GetContragents(user, out msg);
            if (parameters.filter != null && parameters.filter.Count > 0)
            {
                var text = parameters.filter.ContainsKey("text") ? parameters.filter["text"].ToString() : "";
                if (text != "")
                {
                    items = items.Where(x => x.name != null && x.name.Contains(text)).ToList();
                }
            }
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "name":
                    if (direction1 == "up") items = items.OrderBy(x => x.name).ToList();
                    else items = items.OrderByDescending(x => x.name).ToList();
                    break;
                case "email":
                    if (direction1 == "up") items = items.OrderBy(x => x.email).ToList();
                    else items = items.OrderByDescending(x => x.email).ToList();
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.name).ToList();
                    else items = items.OrderByDescending(x => x.name).ToList();
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();

            var json = JsonConvert.SerializeObject(new
            {
                items = res.Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    email = x.email
                }),
                msg = msg,
                total = total
            });
            return Content(json, "application/json");
        }




        public ActionResult ContragentsList_save()
        {
            var parameters = CRUDToDictionary(AjaxModel.GetAjaxParameters(HttpContext));
            var user = mng.GetUser();
            string msg = "";
            h_contragents item;
            int currentID = RDL.Convert.StrToInt(parameters["id"].ToString(), 0);
            if (currentID == 0)
            {
                item = mng.Contragents.CreateContragent(parameters, user, out msg);
            }
            else
            {
                item = mng.Contragents.EditContragent(parameters, currentID, user, out msg);
            }
            var json = JsonConvert.SerializeObject(new { result = true, msg });
            return Content(json, "application/json");
        }

        public ActionResult ContragentsList_remove(int id)
        {
            var user = mng.GetUser();
            string msg = "";
            var result = false;
            result = mng.Contragents.RemoveContragent(id, user, out msg);
            if (!result)
            {
                return Json(new { result = result, msg = msg });
            }
            else
            {
                return Json(new { result = result });
            }

        }
    }
}