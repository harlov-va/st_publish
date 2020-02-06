using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace arkAS.BLL.Imp
{
    public class ImpRepository
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public ImpRepository()
        {
            db = new LocalSqlServer();
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
        #region Items
        public List<imp_items> GetItems()
        {
            var res = new List<imp_items>();
            res = db.imp_items.ToList();
            return res;
        }
        public List<imp_items> GetItems(string code)
        {
            var res = new List<imp_items>();
            res = db.imp_items.Where(x => x.code == code).ToList();
            return res;
        }

        public imp_items GetItem(int id)
        {
            var res = new imp_items();
            res = db.imp_items.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveItem(imp_items element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.imp_items.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteItem(int id)
        {
            bool res = false;
            try
            {
                var item = db.imp_items.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion
        #region ItemLogs
        public List<imp_itemLog> GetItemLogs()
        {
            var res = new List<imp_itemLog>();
            res = db.imp_itemLog.ToList();
            return res;
        }
        public List<imp_itemLog> GetItemLogs(int itemID)
        {
            var res = new List<imp_itemLog>();
            res = db.imp_itemLog.Where(x => x.itemID == itemID).ToList();
            return res;
        }

        public imp_itemLog GetItemLog(int id)
        {
            var res = new imp_itemLog();
            res = db.imp_itemLog.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveItemLog(imp_itemLog element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.imp_itemLog.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteItemLog(int id)
        {
            bool res = false;
            try
            {
                var item = db.imp_itemLog.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        
        internal object GetUserAction()
        {
            throw new NotImplementedException();
        }
        public T GetSQLData<T>(string sql, object parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure);
                    return (T)els;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(T);
            }
        }
    }
}