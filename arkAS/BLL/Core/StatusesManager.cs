using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;

namespace arkAS.BLL.Core
{
    public class StatusesManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public StatusesManager()
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

        public as_statuses GetStatus(string code,string typecode) {
            var res = new as_statuses();
            var key = "as_statuses_code_" + code+"_typecode_"+typecode;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null) {
                res = (as_statuses)CacheManager.Cache[key];
            }else{
                try {
                    res = db.GetStatus(code,typecode);
                    CacheManager.CacheData(key, res);
                }catch(Exception ex){
                    Debug.LogError(ex);
                }           
            }
            return res;        
        }
        public as_statuses GetStatus(int id)
        {
            var res = new as_statuses();
            var key = "as_statuses_id_" + id.ToString();
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_statuses)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetStatusById(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }       
        public void SaveStatus(as_statuses item)
        {
            try {

                db.SaveStatus(item);
                RDL.CacheManager.PurgeCacheItems("as_statuses");

                as_statusLog sl = new as_statusLog()
                {
                    id = 0,
                    created = DateTime.Now,
                    typeCode = item.typeCode,
                    statusID = item.id,
                    username = HttpContext.Current.User.ToString()
                };
                db.SaveStatusLog(sl);

            }catch(Exception ex){
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteStatus(as_statuses item)
        {
            try
            {
                db.DeleteStatus(item.id);
                RDL.CacheManager.PurgeCacheItems("as_statuses" );
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }


        public List<as_statusLog> GetStatusLog(int itemID, string typecode)
        {
            var res = new List<as_statusLog>();
            try
            {
                res = db.GetStatusesLog(typecode).Where(p => p.itemID == itemID).ToList();

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return res;
        }
    }
}