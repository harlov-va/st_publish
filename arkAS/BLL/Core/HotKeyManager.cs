using RDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace arkAS.BLL.Core
{
    public class HotKeyManager
    {
        #region System
        public CoreRepository db;
        private bool _disposed;

        public HotKeyManager()
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

        public as_hotkeys GetHotKey(int id)
        {
            var res = new as_hotkeys();
            var key = "as_hotkey_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_hotkeys)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetHotKey(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public List<as_hotkeys> GetHotKeys()
        {
            var res = new List<as_hotkeys>();
            var key = "as_hotkeys";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<as_hotkeys>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetHotKeys();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public void SaveHotKey(as_hotkeys item)
        {
            try
            {

                db.SaveHotKey(item);

                RDL.CacheManager.PurgeCacheItems("as_hotkeys");
                RDL.CacheManager.PurgeCacheItems("hotKeys");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteHotKey(int id)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_hotkeys");
                RDL.CacheManager.PurgeCacheItems("hotKeys");
                db.DeleteHotKey(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public string[] getRolesForHotKey(int id)
        {
            List<String> res = new List<String>();
            as_hotkeys hotkeys = db.GetHotKey(id);
            var r = hotkeys.roles;
            res.AddRange(r.Split(','));
            return res.ToArray();
        }
        public bool HaveCurrentUserAccess(int id)
        {
            var res = false;
            var h = new as_hotkeys();
            var key = "as_hotkey_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                h = (as_hotkeys)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    h = db.GetHotKey(id);
                    CacheManager.CacheData(key, h);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            var roles = Roles.GetRolesForUser().Count() != 0 ? Roles.GetRolesForUser(): new String[]{"guest"};

            var mRoles = (h.roles ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            res = mRoles.Any(x => roles.Contains(x));

            return res;
        }

        private bool HaveCurrentUserAccess(as_hotkeys h, string[] roles)
        {
            var res = false;

            var mRoles = (h.roles ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            res = mRoles.Any(x => roles.Contains(x));
            return res;
        }
        internal void EditHotKeyField(int id, string name, string value)
        {
            var item = GetHotKey(id);
            switch (name)
            {
                case "isAlt": item.isAlt = bool.Parse(value); break;
                case "isCtrl": item.isCtrl = bool.Parse(value); break;
                case "isShift": item.isShift = bool.Parse(value); break;
                case "js": item.js = value; break;
                case "keyCode": item.keyCode = RDL.Convert.StrToInt(value, 0); break;
                case "roles":
                    {
                        List<string> m = item.roles.Split(',').ToList();
                        string[] s = value.Split('=');
                        if (bool.Parse(s[1]))
                        {
                            if (!m.Exists(x => x == s[0])) m.Add(s[0]);
                        }
                        else
                        {
                            m.Remove(s[0]);
                        }
                        item.roles = String.Join(",", m);
                        break;
                    }
                case "url": item.url = value; break;
            }
            SaveHotKey(item);
        }
    }
}