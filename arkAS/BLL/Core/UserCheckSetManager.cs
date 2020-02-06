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
    public class UserCheckSetManager
    {
        #region System
        public CoreRepository db;
        private bool _disposed;

        public UserCheckSetManager()
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

        public List<as_userCheckSet> GetUserCheckSets()
        {
            var res = new List<as_userCheckSet>();
            res = db.GetUserCheckSets();
            return res;
        }

        public as_userCheckSet GetUserCheckSet(int id)
        {
            var res = new as_userCheckSet();
            //var key = "as_userCheckSet_" + id.ToString();
            //if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            //{
            //    res = (as_userCheckSet)CacheManager.Cache[key];
            //}
            //else
            //{
            try
            {
                res = db.GetUserCheckSet(id);
                //CacheManager.CacheData(key, res);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            //}

            return res;
        }

        public void SaveUserCheckSet(as_userCheckSet item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_userCheckSet_" + item.id);
                db.SaveUserCheckSet(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteUserCheckSet(as_userCheckSet item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_userCheckSet_" + item.id);
                db.DeleteUserCheckSet(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public as_userCheckSet AddUserCheckSet(string name, string code, string roles)
        {
            var res = new as_userCheckSet();
            try
            {
                res = new as_userCheckSet { id = 0, name = name, code = code, roles = String.IsNullOrEmpty(roles) ? "admin" : roles };
                db.SaveUserCheckSet(res);
            }
            catch (Exception ex) { }
            return res;
        }
        public void EditUserCheckSet(int id, string name, string code, string roles)
        {
            var element = db.GetUserCheckSet(id);
            try
            {
                element.name = name;
                element.code = code;
                element.roles = roles;

                db.SaveUserCheckSet(element);
            }
            catch (Exception ex) { }
        }
    }
}