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
    public class MenuProfileManager
    {

        #region System
        public CoreRepository db;
        private bool _disposed;

        public MenuProfileManager()
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
        public List<as_profileMenu> GetProfileMenu(Guid userGuid)
        {
            var res = new List<as_profileMenu>();
            res = db.GetProfileMenu(userGuid);

            return res;
        }

        public as_profileMenu GetProfileMenu(int id)
        {
            var res = new as_profileMenu();
            res = db.GetProfileMenu(id);

            return res;
        }

        public void SaveProfileMenu(as_profileMenu item)
        {
            try
            {
                db.SaveProfileMenu(item);
                RDL.CacheManager.PurgeCacheItems("as_profileMenu");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditProfileMenu(int pk, string name, string href, string cssClass)
        {
            var item = GetProfileMenu(pk);

            item.name = name;
            item.href = href;
            item.cssClass = cssClass;

            SaveProfileMenu(item);
            RDL.CacheManager.PurgeCacheItems("as_menu");
        }

        public void DeleteProfileMenu(int id)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_profileMenu");
                db.DeleteProfileMenu(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion
    }
}