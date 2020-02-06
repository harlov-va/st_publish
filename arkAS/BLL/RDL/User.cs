using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;


namespace RDL
{
    public abstract class User
    {
        public static IPrincipal CurrentUser
        {
            get { return HttpContext.Current.User; }
        }

        public static string UserName
        {
            get
            {
                string userName = "";
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                    userName = HttpContext.Current.User.Identity.Name;
                return userName;
            }
        }

        public static string UserIP
        {
            get { return HttpContext.Current.Request.UserHostAddress; }
        }
    } 
}