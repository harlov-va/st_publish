using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Article
{
    public class ArticleRepository
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public ArticleRepository()
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

        #region News

        public List<art_news> GetItems()
        {
            var res = new List<art_news>();
            res = db.art_news.OrderByDescending(x => x.created).ToList();
            return res;
        }

        public art_news GetItem(int id)
        {
            var res = new art_news();
            res = db.art_news.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveItem(art_news element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.art_news.Add(element);
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
                var item = db.art_news.SingleOrDefault(x => x.id == id);
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

        #region NewsType

        public List<art_newsType> GetItemsType()
        {
            var res = new List<art_newsType>();
            res = db.art_newsType.Select(x => x).ToList();
            return res;
        }

        public art_newsType GetItemType(int id)
        {
            var res = new art_newsType();
            res = db.art_newsType.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveItemType(art_newsType element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.art_newsType.Add(element);
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

        public bool DeleteItemType(int id)
        {
            bool res = false;
            try
            {
                var item = db.art_newsType.SingleOrDefault(x => x.id == id);
                
                if (item != null)
                {
                    foreach (var i in item.art_news)
                    {
                        i.typeID = null;
                    }
                    db.SaveChanges();
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