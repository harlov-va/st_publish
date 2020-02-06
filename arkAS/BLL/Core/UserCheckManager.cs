using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using DocumentFormat.OpenXml.EMMA;
using RDL;

namespace arkAS.BLL.Core
{
    public class UserCheckManager
    {
        #region System
        public CoreRepository db;
        private bool _disposed;

        public UserCheckManager()
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

        public List<as_userChecks> GetUserChecks()
        {
            var res = new List<as_userChecks>();
            res = db.GetUserChecks();
            return res;
        }

        public as_userChecks GetUserCheck(int id)
        {
            var res = db.GetUserCheck(id);

            return res;
        }

        public as_userChecks GetUserCheck(int idCheckItem, string nameUser)
        {
            var res = db.GetUserCheck(idCheckItem, nameUser);

            return res;
        }

        public void SaveUserCheck(as_userChecks item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_userCheck_" + item.id);
                db.SaveUserCheck(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteUserCheck(as_userChecks item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_userCheck_" + item.id);
                db.DeleteUserCheck(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }


        public as_userChecks AddUserCheck(string username, bool isClosed, string note, System.DateTime modifiedDate, int itemID)
        {
            var res = new as_userChecks();
            try
            {
                res = new as_userChecks { id = 0, username = username, isClosed = isClosed, note = note, modifiedDate = modifiedDate, itemID = itemID };
                db.SaveUserCheck(res);
            }
            catch (Exception ex) { }
            return res;
        }



    }
}