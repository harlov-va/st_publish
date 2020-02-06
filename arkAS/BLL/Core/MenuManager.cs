using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arkAS.BLL;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;
using System.Web.Security;

namespace arkAS.BLL.Core
{
    public class MenuManager
    {

        #region System
        public CoreRepository db;
        private bool _disposed;

        public MenuManager()
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


        #region Menu
        public as_menu GetMenu(int id)
        {
            var res = new as_menu();
            var key = "as_menu_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_menu)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetMenu(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;  
      }

        public List<as_menu> GetMenu(bool withoutRoles=false)
        {

            var res = new List<as_menu>();
            var key = "as_menu_"+withoutRoles;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false)
            {
                res = (List<as_menu>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    var roles = Roles.GetRolesForUser(HttpContext.Current.User.Identity.Name);
                    res = db.GetMenu().Where(x => withoutRoles || x.as_menuRoles.Any(y => roles.Contains(y.role))).ToList();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;        

        }

        public void SaveMenu(as_menu item)
        {
            try
            {
                db.SaveMenu(item);
                RDL.CacheManager.PurgeCacheItems("as_menu");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }            
        }

        public void DeleteMenu(int id)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_menu");
                db.DeleteMenu(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditMenuField(int pk, string name, string value)
        {
            var item = GetMenu(pk);
            switch (name)
            {
                case "name": item.name = value; break;
                case "url": item.url = value; break;
                case "pattern": item.pattern = value; break;
                case "parentName":
                    if (value != "")
                        item.parentID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.parentID = null;
                    break;
            }
            SaveMenu(item);
            RDL.CacheManager.PurgeCacheItems("as_menu");
        }

        public List<as_menu> GetMenuChild(int parentID)
        {
            var res = new List<as_menu>();
            res = GetMenu().Where(x => x.parentID == parentID).ToList();            
            return res;
        }

        public string[] getRolesForMenu(int menuID)
        {
            List<String> res = new List<String>();
            List<as_menuRoles> list=db.getRolesForMenu(menuID);
            foreach (as_menuRoles item in list)
            {
                res.Add(item.role);
            }
            return res.ToArray();
        }

        public void addMenuToRole(int menuID, string role)
        {
            var item = new as_menuRoles { id = 0, itemID = menuID, role = role, ord = null };
            db.SaveMenuRole(item);
            RDL.CacheManager.PurgeCacheItems("as_menu");
        }

        public void removeMenuToRole(int menuID, string role)
        {
            var item = db.GetMenuRole(menuID, role);
            if (item != null) {
                db.DeleteMenuRole(item.id);
                RDL.CacheManager.PurgeCacheItems("as_menu");
            }
        }

        #endregion
    }
}