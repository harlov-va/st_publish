using RDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Core
{
    public class ProfileManager
    {   
        
        #region System
        public CoreRepository db;
        private bool _disposed;

        public ProfileManager()
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

        public string GetProperty(string code, string def, string guid = "")
        {
            string res = def;

            CoreManager mng = new CoreManager();    
            Guid userGuid = Guid.Empty;
            if (guid == "")
                userGuid = mng.GetUserGuid();
            else
            {
                userGuid = new Guid(guid);
            }

            string key = "as_profile_prop_code_" + code + "_user_" + userGuid + "_def_" + def;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = CacheManager.Cache[key].ToString();
            }
            else
            {
                var item = db.GetProfileProperty(code);
                if (item != null)
                {                    
                    var itemVal = db.GetProfilePropertyValue(item.id, userGuid);
                    if(itemVal!=null)
                    {                        
                        res = itemVal.value;
                    }
                }
                CacheManager.CacheData(key, res);
            }
            return res;
        }

        public void SetProperty(string code, string value, string guid = "")
        {            
            CoreManager mng = new CoreManager();    
            Guid userGuid = Guid.Empty;
            if (guid == "")
                userGuid = mng.GetUserGuid();
            else
            {
                userGuid = new Guid(guid);
            }

            var prop = db.GetProfileProperty(code);

            if (prop == null)
            {
                prop = new as_profileProperties { id = 0, code = code, name = code };
                db.SaveProfileProperty(prop);                
            }

            var item = db.GetProfilePropertyValue(prop.id, userGuid);
            if (item != null)
            {
                item.value = value;

                db.SaveProfilePropertyValue(item);                
            }
            else
            {
                item = new as_profilePropertyValues { id = 0, propertyID = prop.id, userGuid = userGuid, value = value };
                db.SaveProfilePropertyValue(item);
                
            }
            string key = "as_profile_prop_code_" + code + "_user_" + userGuid;
            CacheManager.PurgeCacheItems(key);
        }

        public static string GetCookieProperty(string name, string def)
        {
            string res = def;

            try
            {
                // если уже что то записывали в этот Postback?
                if (HttpContext.Current.Session[name] != null)
                {
                    res = HttpContext.Current.Session[name].ToString();
                    return res;
                }
            }
            catch (Exception ex) { }


            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                res = HttpContext.Current.Request.Cookies[name].Value.ToString();
            }
            else
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(name, def));
            }
            return res;
        }
        public static void SetCookieProperty(string name, string value)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(name, value));
            HttpContext.Current.Session.Add(name, value);

        }
    }
    
}