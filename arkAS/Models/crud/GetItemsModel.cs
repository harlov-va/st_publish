using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Models.crud
{

    public class GetItemsModel
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string sort { get; set; }
        public string direction { get; set; }
        public Dictionary<string, object> filter { get; set; }
        public Dictionary<string, object> mode { get; set; }

        public static GetItemsModel GetParameters(HttpContextBase context)
        {
            var res = new GetItemsModel();
            var form = context.Request.Form;

            res.page = RDL.Convert.StrToInt(form["page"], 1);
            res.pageSize = RDL.Convert.StrToInt(form["pageSize"], 10);

            res.sort = form["sort"];
            res.direction = form["direction"];
            res.mode = (object)form["mode"] as Dictionary<string, object>;
            return res;
        }

        public static object GetFilterParameter(HttpContextBase context, string name)
        {
            string res = context.Request.Form[name];
            if (res == null) res = "";
            return res;
        }
    }
}
