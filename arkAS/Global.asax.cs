using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using arkAS.BLL.Core;
using arkAS.Infrastructure;
using Glimpse.Ado;
using RDL;
using StackExchange.Profiling;

namespace arkAS
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
         
        }
        
        //private void Seed()
        //{
        //    MembershipUser newUser = Membership.CreateUser("rosh@email.com", "password", "rosh@email.com");
        //    Roles.AddUsersToRole(new string[]{ "rosh@email.com"}, "user");            
        //}
        //private void Seed()
        //{
        //    MembershipUser newUser = Membership.CreateUser("roshAdmin@email.com", "password", "roshAdmin@email.com");
        //    Roles.AddUsersToRole(new string[] { "roshAdmin@email.com" }, "admin");
        //}

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalFilters.Filters.Add(new ArkActionFilterAttribute());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleMobileConfig.RegisterBundles(BundleTable.Bundles);

            MiniProfiler.Settings.Results_Authorize = IsUserAllowedToSeeMiniProfilerUI;
            Glimpse.Settings.Initialize.Ado();
            
            if (Context.IsDebuggingEnabled) Debug.EnableErrorLogEmail = false;
            //Seed();

        }

        private bool IsUserAllowedToSeeMiniProfilerUI(HttpRequest httpRequest)
        {
            // Implement your own logic for who 
            // should be able to access ~/mini-profiler-resources/results
            var principal = httpRequest.RequestContext.HttpContext.User;
            return principal.IsInRole("admin") || principal.IsInRole("user") || httpRequest.IsLocal;
        }


        public DateTime startTime = DateTime.Now;
        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
            if (WebConfigurationManager.AppSettings["TracePagePerfomance"] == "1")
            {
                startTime = DateTime.Now;
            }           
            //MiniProfiler.Start();

            try
            {
                string session_param_name = "ASPSESSID";
                string session_cookie_name = "ASP.NET_SessionId";

                if (HttpContext.Current.Request.Form[session_param_name] != null)
                {
                    UpdateCookie(session_cookie_name, HttpContext.Current.Request.Form[session_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[session_param_name] != null)
                {
                    UpdateCookie(session_cookie_name, HttpContext.Current.Request.QueryString[session_param_name]);
                }
            }
            catch
            {
            }

            try
            {
                string auth_param_name = "AUTHID";
                string auth_cookie_name = FormsAuthentication.FormsCookieName;

                if (HttpContext.Current.Request.Form[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.QueryString[auth_param_name]);
                }

            }
            catch
            {
            }

        }

        private void UpdateCookie(string cookie_name, string cookie_value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookie_name);
            if (null == cookie)
            {
                cookie = new HttpCookie(cookie_name);
            }
            cookie.Value = cookie_value;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
            if (WebConfigurationManager.AppSettings["TracePagePerfomance"] == "1")
            {
                var endTime = DateTime.Now;
                var mng = new TraceManager();
                var duration = (endTime - startTime).TotalSeconds;
                mng.Warn(HttpContext.Current.Request.RawUrl, "", (int)Math.Round(duration * 1000, 0), "perf");                
            }
        }

    }
}
