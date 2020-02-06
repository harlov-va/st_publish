using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.TextMistakes
{
    public class TextMistakesManager
    {
        #region System
        public TextMistakesRepository db;
        private bool _disposed;

        public TextMistakesManager()
        {
            db = new TextMistakesRepository();
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
            res = db.GetItems();
            return res;
        }

        public as_textMistakes GetItem(int id)
        {
            var res = new as_textMistakes();
            res = db.GetItem(id);
            return res;
        }

        public void SaveItem(as_textMistakes item)
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
                case "comment": item.comment = value; break;
                case "selectText": item.selectText = value; break;
                case "url": item.url = value; break;
                case "correct":
                    if (value == "да")
                        item.correct = true;
                    else
                        item.correct = false;
                    break;
            }
            SaveItem(item);
        }

        #endregion
    }
}