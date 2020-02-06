using arkAS.BLL.Favorites;
using arkAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Controllers
{
    public class FavoritesController : Controller
    {
        public ActionResult Favorites()
        {
            return View();
        }
        public ActionResult SetFavorite()
        {
            var res = false;
            string message = null;
            var id = -1;
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                var itemId = AjaxModel.GetAjaxParameter("itemId", parameters);
                var appName = AjaxModel.GetAjaxParameter("appName", parameters);
                var favorite = new BLL.as_favorites { appName = appName, created = DateTime.Now, itemID = int.Parse(itemId), userGuid = new BLL.Core.CoreManager().GetUserGuid() };
                id = new FavoritesManager().AddFavorites(favorite);
                if (id > 0) 
                {
                    message = "Добавлено в избранное";
                    res = true;
                }
                else
                {
                    message = "Ошибка добавления в избранное.";
                    res = false;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                message = "Ошибка добавления в избранное";
            }
            return Json(new
            {
                result = res,
                msg = message,
                id = id
            });
        }
        public ActionResult DelFavorite()
        {
            var res = false;
            string message = null;
            var id = -1;
            try
            {
                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                id = int.Parse(AjaxModel.GetAjaxParameter("idFavorite", parameters));
                new BLL.Favorites.FavoritesManager().DeleteFavorites(id);
                message = "Успешно удалено";
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                message = "Ошибка удаления";
                id = -1;
            }
            return Json(new
            {
                result = res,
                msg = message,
                idFavorite = id
            });
        }
        public ActionResult GetFavoritesItems()
        {
            try
            {
                var items = new BLL.Favorites.FavoritesManager().GetItems(new BLL.Core.CoreManager().GetUserGuid());
                return Json(new
                {
                    items = items.Select(x => new
                    {
                        idFavorite=x.id,
                        created = x.created.ToString("dd.MM.yyyy hh:mm:ss"),
                        x.appName,
                        itemId = x.itemID,
                        userGuid = x.userGuid.ToString()
                    }),
                        result = true,
                        message = "",
                        total = items.Count()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {
                result = false,
                msg = "Ошибка выборки данных"
            });
        }
        public ActionResult SimpleTable_getItems()
        {
            var parameters = AjaxModel.GetParameters(HttpContext);
            var mng = new BLL.CRM.CRMManager();

            var items = mng.GetOrders().AsQueryable();
            var sorts = parameters.sort.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var directions = parameters.direction.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var sort1 = sorts.Length > 0 ? sorts[0] : "";
            var direction1 = directions.Length > 0 ? directions[0] : "";

            switch (sort1)
            {
                case "orderNum":
                    if (direction1 == "up") items = items.OrderBy(x => x.id);
                    else items = items.OrderByDescending(x => x.id);
                    break;
                case "statusName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_orderStatuses.name);
                    else items = items.OrderByDescending(x => x.crm_orderStatuses.name);
                    break;
                case "clientName":
                    if (direction1 == "up") items = items.OrderBy(x => x.crm_clients.fio);
                    else items = items.OrderByDescending(x => x.crm_clients.fio);
                    break;
                default:
                    if (direction1 == "up") items = items.OrderBy(x => x.created);
                    else items = items.OrderByDescending(x => x.created);
                    break;
            }
            var total = items.Count();
            var res = items.Skip(parameters.pageSize * (parameters.page - 1)).Take(parameters.pageSize).ToList();


            return Json(new
            {
                items = res.Select(x => new
                {
                    x.id,
                    x.orderNum,
                    x.statusID,
                    statusName = x.crm_orderStatuses != null ? x.crm_orderStatuses.name : "",
                    x.addedBy,
                    x.clientID,
                    clientName = x.crm_clients != null ? x.crm_clients.fio : "",
                    created = x.created.GetValueOrDefault().ToString("dd.MM.yyyy"),
                    setFavorite = "<a href='' class='as-favorites-set' itemId='" + x.id.ToString() + "' appName='Заказ'>Добавить</a>"
                }),
                total = items.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdditionData()
        {
            try
            {
                BLL.CRM.CRMManager crm = new BLL.CRM.CRMManager();

                var parameters = AjaxModel.GetAjaxParameters(HttpContext);
                string orderId = AjaxModel.GetAjaxParameter("itemId", parameters);
                BLL.crm_orders order = crm.GetOrders().FirstOrDefault( x => x.id == int.Parse(orderId));
                BLL.crm_orderStatuses statuse = crm.GetOrderStatuses().FirstOrDefault(x => x.id == order.statusID);
                BLL.crm_clients client = new BLL.crm_clients() { fio="Клиент не найден", username="" }; 
                if (order != null)
                {
                    client = crm.GetClient(order.clientID ?? 0);
                }

                return Json(new
                {
                    item = new {
                        client.fio,
                        client.username,
                        status = statuse.name
                    },
                    result = true,
                    message = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return Json(new
            {
                result = false,
                msg = "Ошибка выборки данных"
            });
        }
	}
}