using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL.ProcessItem
{
    public class ProcessItemRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public ProcessItemRepository()
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

        #region ProcessItem

        public Dictionary<int, string> GetProcessList()
        {
            Dictionary<int, string> res = db.proc_processes.Select(x => new { id = x.id, name = x.name }).Distinct().ToDictionary(a => a.id, a => a.name);
            return res;
        }
        
        public List<proc_processItems> GetProcessItems()
        {
            var res = new List<proc_processItems>();
            res = db.proc_processItems.ToList();
            return res;
        }
        
        public proc_processItems GetProcessItem(int id)
        {
            var res = new proc_processItems();
            res = db.proc_processItems.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<string> GetUserNames()
        {
            List<string> res = db.aspnet_Users.Select(x => x.UserName).ToList();
            return res;
        }
       
        public int SaveProcessItem(proc_processItems element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.proc_processItems.Add(element);
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

        public bool DeleteProcessItem(int id)
        {
            bool res = false;
            try
            {
                var item = db.proc_processItems.SingleOrDefault(x => x.id == id);
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

    }
}