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
    public class TraceManager
    {
        #region System
        public CoreRepository db;
        private bool _disposed;

        public TraceManager()
        {
            db = new CoreRepository();
            _disposed = false;
            //SERIALIZE WILL FAIL WITH PROXIED ENTITIES
            //dbContext.Configuration.ProxyCreationEnabled = false;
            //ENABLING COULD CAUSE ENDLESS LOOPS AND PERFORMANCE PROBLEMS
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }
        internal void EditTextField(int pk, string name, string value)
        {
            var item = db.GetTraceById(pk);
            switch (name)
            {
                case "header": item.header= value; break;
                case "code": item.code = value; break;
                case "itemId": item.itemID= RDL.Convert.StrToInt(value, 0); break;
                case "text": item.text = value; break;
                case "created": item.created=RDL.Convert.StrToDateTime(value,DateTime.Now); break;
            }
            SaveTrace(item);
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

        public void SaveTrace(as_trace item)
        {
            try
            {

                db.SaveTrace(item);
                RDL.CacheManager.PurgeCacheItems("as_trace");

               

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public as_trace GetTrace(int id)
        {
            var res = new as_trace();
            var key = "as_trace_id_" + id.ToString();
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_trace)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetTraceById(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
      
        public void DeleteTrace(as_trace item)
        {
            try
            {
                db.DeleteTrace(item.id);
                RDL.CacheManager.PurgeCacheItems("as_trace");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        //public List<as_trace> GetTrace()
        //{
        //    var res = db.GetTrace();
        //    return res;
        //}



        public void Warn(string header, string text = "", int itemID = 0, string code = "")
        {
            try
            {
                db.SaveTrace(new as_trace { code = code, id = 0, created = DateTime.Now, header = header, itemID = itemID, text = text });
            }
            catch (Exception ex)
            {

            }
        }



    }
}