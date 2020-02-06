using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL.TextMistakes
{
    public class TextMistakesRepository
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public TextMistakesRepository()
        {
            db = new LocalSqlServer();
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

        #region TextMistakes

        public List<as_textMistakes> GetItems()
        {
            var res = new List<as_textMistakes>();
            res = db.as_textMistakes.OrderByDescending(x => x.aspnet_Users.UserName).ToList();
            return res;
        }

        public as_textMistakes GetItem(int id)
        {
            var res = new as_textMistakes();
            res = db.as_textMistakes.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveItem(as_textMistakes element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_textMistakes.Add(element);
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
                var item = db.as_textMistakes.SingleOrDefault(x => x.id == id);
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