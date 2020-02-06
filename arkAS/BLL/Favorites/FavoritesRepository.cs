using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arkAS.BLL;
using System.Data.Entity;
using System.Data.Entity.Core;

namespace arkAS.BLL.Favorites
{
    public class FavoritesRepository
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public FavoritesRepository()
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

        public as_favorites GetItem(int id)
        {
            as_favorites res = db.as_favorites.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_favorites> GetItems()
        {
            return db.as_favorites.OrderByDescending(x => x.created).ToList();
        }
        public List<as_favorites> GetItems(Guid user)
        {
            return db.as_favorites.Where(x => x.userGuid == user).OrderByDescending(x => x.created).ToList();
        }

        public int SaveItem(as_favorites favorite)
        {
            if (favorite.id == 0)
            {
                db.as_favorites.Add(favorite);
                db.SaveChanges();
            }
            else
            {
                try
                {
                    db.Entry(favorite).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    RDL.Debug.LogError(ex);
                }
            }
            return favorite.id;
        }

        public void DeleteItem(int id)
        {
            var favorite = db.as_favorites.SingleOrDefault(x => x.id == id);
            if (favorite != null)
            {
                db.Entry(favorite).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
        }

    }
}