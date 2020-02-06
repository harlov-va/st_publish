using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Favorites
{
    public class FavoritesManager
    {
        #region System
        public FavoritesRepository db;
        private bool _disposed;

        public FavoritesManager()
        {
            db = new FavoritesRepository();
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

        internal as_favorites GetItem(int id)
        {
            var res = db.GetItem(id);
            return res;
        }

        internal List<as_favorites> GetItems()
        {
            return db.GetItems();
        }
        internal List<as_favorites> GetItems(Guid user)
        {
            return db.GetItems(user);
        }

        internal void SaveFavorite(as_favorites favorite)
        {
            try
            {

                db.SaveItem(favorite);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal int AddFavorites(as_favorites favorite)
        {
            try
            {
                return db.SaveItem(favorite);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return -1;
        }

        internal void DeleteFavorites(int id)
        {
            try
            {
                db.DeleteItem(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditFavoriteField(int pk, string name, string value)
        {
            var item = GetItem(pk);
            switch (name)
            {
                case "itemID": item.itemID = int.Parse(value); break;
                case "appName": item.appName = value; break;
            }
             SaveFavorite(item);
        }
    }
}