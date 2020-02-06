using arkAS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arkAS.Infrastructure
{
    public class ArkActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if(context.HttpContext.User.Identity.IsAuthenticated){
                var mng = new CoreManager();
                mng.LogUserAction(context.HttpContext.Request.RawUrl, String.Join("; ", context.ActionParameters.Select(x => x.Key + " = " + x.Value)));
            }
        }
    }
}