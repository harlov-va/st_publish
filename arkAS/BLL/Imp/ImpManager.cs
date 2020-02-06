using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arkAS.BLL.Core;
using RDL;

namespace arkAS.BLL.Imp
{
    public class ImpManager
    {
         #region System
        public ImpRepository db;
        private bool _disposed;

        public ImpManager()
        {
            db = new ImpRepository();
            _disposed = false;
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

        public List<imp_items> GetItems()
        {
            var res = new List<imp_items>();
            res = db.GetItems();
            return res;
        }
        public List<imp_items> GetItems(string code)
        {
            var res = new List<imp_items>();
            var key = "imp_items_code_" + code;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<imp_items>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetItems(code);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public imp_items GetItem(int id)
        {
            var res = db.GetItem(id);
            return res;
        }
        public void SaveItem(imp_items item)
        {
            try
            {
                db.SaveItem(item);
                RDL.CacheManager.PurgeCacheItems("as_items");

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteItem(imp_items item)
        {
            try
            {
                db.DeleteItem(item.id);
                RDL.CacheManager.PurgeCacheItems("as_items");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public List<imp_itemLog> GetItemLogs()
        {
            var res = new List<imp_itemLog>();
            res = db.GetItemLogs();
            return res;
        }
        public List<imp_itemLog> GetItemLogs(int itemID)
        {
            var res = new List<imp_itemLog>();
            var key = "imp_itemLog_itemID_" + itemID;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<imp_itemLog>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetItemLogs(itemID);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public imp_itemLog GetItemLog(int id)
        {
            var res = db.GetItemLog(id);
            return res;
        }
        public void SaveItemLog(imp_itemLog item)
        {
            try
            {
                db.SaveItemLog(item);
                RDL.CacheManager.PurgeCacheItems("imp_itemLog");

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteItemLog(imp_itemLog item)
        {
            try
            {
                db.DeleteItemLog(item.id);
                RDL.CacheManager.PurgeCacheItems("imp_itemLog");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }


    }
}