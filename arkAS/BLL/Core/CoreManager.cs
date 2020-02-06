using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;
using System.Web.Security;
using System.Net.Mail;

namespace arkAS.BLL.Core
{
    public class CoreManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public CoreManager()
        {
            db = new CoreRepository();
            _disposed = false;
            //SERIALIZE WILL FAIL WITH PROXIED ENTITIES
            //dbContext.Configuration.ProxyCreationEnabled = false;
            //ENABLING COULD CAUSE ENDLESS LOOPS AND PERFORMANCE PROBLEMS
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (db != null)
                        db.Dispose();
                }
                db = null;
                _disposed = true;
            }
        }
        #endregion
        public List<as_dataTypes> GetDataTypes()
        {
            var res = new List<as_dataTypes>();
            var key = "as_datatypes";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<as_dataTypes>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetDataTypes();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }

        public bool ClearCache()
        {
            var res = false;
            try {
                CacheManager.PurgeCacheItems("as_");
                res = true;
            }
            catch(Exception ex){
                RDL.Debug.LogError(ex);
            }            
            return res;
        }

        public string BackupDatabase(ref string DataBaseFile)
        {
            string errors = "Error";
            try
            {
                switch (db.BackupDatabase(ref DataBaseFile))
                {
                    case -1: { errors = "Error"; } break;
                    case -2: { errors = "database="; } break;
                    case -3: { errors = "LenCopy"; } break;
                    case 0: { errors = "SUCCESS"; } break;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return errors;
        }

        public bool LogUserAction(string url, string  @params)
        {
            var res = false;
            try
            {
                var item = new as_userActions { id = 0, created = DateTime.Now, username = HttpContext.Current.User.Identity.Name, @params = RDL.Text.GetValidCrop(@params, 255, ""), url = url };
                db.SaveUserAction(item);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        public bool SendEmail(List<MailAddress> to, string subject, string body, string attach = "", string cc= "", string bcc="")
        {
            var res = false;
            var mng = new SettingsManager();

            foreach (var item in to) { 
            RDL.Email.SendMail(
                   mng.GetSetting("mail.from", ""),
                   mng.GetSetting("mail.displayName", ""),
                   item.Address, bcc, cc, subject, body,
                   mng.GetSetting("mail.server", ""),
                   mng.GetSetting("mail.username", ""), mng.GetSetting("mail.password", ""), int.Parse(mng.GetSetting("mail.port", "0")),
                   mng.GetSetting("mail.ssl", "").ToLower()=="true", 
                   attach
                   );
            }

            return res;        
        }

        public bool SendEmail(string email, string subject, string body, string attach = "", string cc="", string bcc="")
        {
            return SendEmail(new List<MailAddress> { new MailAddress(email) }, subject, body, attach, cc, bcc);         
        }
        
        public Guid GetUserGuid()
        {
            Guid g = Guid.Empty;            

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                MembershipUser us = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                if (us != null) g = new Guid(us.ProviderUserKey.ToString());
            }
            else
            {
                if (HttpContext.Current.Request.Cookies["userGuid"] == null)
                {
                    g = Guid.NewGuid();
                    var cook = new HttpCookie("userGuid", g.ToString());
                    cook.Expires = DateTime.Now.AddYears(1);
                    HttpContext.Current.Response.Cookies.Add(cook);
                }
                else
                {
                    g = new Guid(HttpContext.Current.Request.Cookies["userGuid"].Value);
                }
            }
            return g;
        }
      

        public static string StyleForDev()
        {
            var res = "";
            var mng = new SettingsManager();
            var color = mng.GetSetting("BodyColor", "");
    	    if(color!=""){
    	        res = "<style type='text/css'> body{ background: "+color+" !important; }</style>";
            }
            return res;
        }

        public static string GetMainFormLink(string code="")
        {
            var res = "";
            res = (code != "defaultTop" ? "<br /><br />" : "") + "<a href='#' class='as-form-simple btn btn-success btn-lg' data-code='lead' data-email='E-mail *' data-name='Ваше имя *' data-phone='Телефон' data-text='Дополнительно' " +
                "data-email-placeholder='ivanov@mail.ru' data-name-placeholder='Иванов Иван Иванович' data-phone-placeholder='+7 900 800 7777' data-text-placeholder='Комментарии к заявке' " +
                " data-button='Подать заявку' data-title='Заявка на проработку проекта' data-subtitle='Пожалуйста укажите следующие поля и мы свяжемся с Вами в ближайшее время'>Отправьте заявку на проработку Вашего проекта</a>";
            return res;
        }


        public static string GetDownloadBookLink(string mode = "")
        {
            var res = "";
            res = "<a href='#' class='as-form-simple btn btn-success btn-sm' data-code='book' data-email='E-mail *' data-name='Ваше имя *' data-phone='' data-text='' " +
                "data-email-placeholder='ivanov@mail.ru' data-name-placeholder='Иванов Иван Иванович' data-phone-placeholder='' data-text-placeholder='' " +
                " data-button='Получить книгу на почту' data-title='Получить бесплатно книгу по CRM на почту' data-subtitle='Укажите email для отправки книги вам на почту'>Получить книгу</a>";
            return res;
        }

        public static string GetCabinetLayout()
        {
            var res = "";
            res = "~/Views/Shared/_CabinetLayout.cshtml";
            return res;
        }

        public IQueryable<aspnet_Users> GetUsers()
        {
            return db.GetUsers();
        }






    }
}