using RDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Article
{
    public class ArticleManager
    {
        #region System
        public ArticleRepository db;
        private bool _disposed;

        public ArticleManager()
        {
            db = new ArticleRepository();
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
            res = db.GetItems();
            return res;
        }

        public art_news GetItem(int id)
        {
            var res = new art_news();
            res = db.GetItem(id);
            return res;
        }

        public void SaveItem(art_news item)
        {
            try
            {
                db.SaveItem(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteItem(int id)
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

        internal void EditItemField(int pk, string name, string value)
        {
            var item = GetItem(pk);
            switch (name)
            {
                case "title": item.title = value; break;
                case "imgPath": item.imgPath = value; break;
                case "anouns": item.anouns = value; break;
                case "text": item.text = value; break;
                case "typeID":
                    if (value != "")
                        item.typeID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.typeID = null;
                    break;
            }
            SaveItem(item);
        }

        #endregion
        

        #region NewsType

        public List<art_newsType> GetItemsType()
        {
            var res = new List<art_newsType>();
            res = db.GetItemsType();
            return res;
        }

        public art_newsType GetItemType(int id)
        {
            var res = new art_newsType();
            res = db.GetItemType(id);
            return res;
        }

        public void SaveItemType(art_newsType item)
        {
            try
            {
                db.SaveItemType(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteItemType(int id)
        {
            try
            {
                db.DeleteItemType(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditItemTypeField(int pk, string name, string value)
        {
            var item = GetItemType(pk);
            switch (name)
            {
                case "name": item.name = value; break;
                case "code": item.code = value; break;
                default: break;
            }
            SaveItemType(item);
        }

        #endregion

    }
}