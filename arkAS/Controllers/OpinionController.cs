using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arkAS.BLL.Core;
using arkAS.BLL;
using System.Web.Script.Serialization;
using RDL;

namespace arkAS.Controllers
{
    public class OpinionController : Controller
    {
        private readonly OpinionManager _mng;

        public OpinionController()
        {
            _mng = new OpinionManager();
        }

        public ActionResult GetCacheOpinions()
        {
            var opinions = new List<string>();

            try
            {
                opinions = _mng.GetCacheOpinions();

                return Json(new
                {
                    result = true,
                    items = opinions
                });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return Json(new { result = false });
            }

        }

        public ActionResult Save(string itemID, string like, string comment, int cacheDuration)
        {
            try
            {
                var item = new as_opinion
                {
                    id = 0,
                    itemID = RDL.Convert.StrToNullableInt32(itemID),
                    like = System.Convert.ToBoolean(like),
                    comment = comment,
                    created = DateTime.Now,
                    username = RDL.User.UserName,
                    type = "customer"
                };

                _mng.SaveOpinion(item, cacheDuration);

                return Json(new { result = true });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return Json(new { result = false });
            }


        }

    }
}