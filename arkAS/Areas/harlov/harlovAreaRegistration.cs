using System.Web.Mvc;

namespace arkAS.Areas.harlov
{
    public class harlovAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "harlov";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "harlov_default",
                "harlov/{controller}/{action}/{id}",
                new { controller = "Documents", action = "DocumentsList", id = UrlParameter.Optional },
                new [] {"arkAS.Areas.harlov.Controllers"}
            );
        }
    }
}