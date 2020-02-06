using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using DocumentFormat.OpenXml.EMMA;
using RDL;

namespace arkAS.BLL.Core
{
    public class CommentManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public CommentManager()
        {
            db = new CoreRepository();
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

        public List<as_comments> GetComments(string type, string itemID) {
            var res = new List<as_comments>();
            res = db.GetComments(type, itemID);
            return res;        
        }

        public as_comments GetComment(int id)
        {
            var res = db.GetComment(id);

            return res;
        }

        public void SaveComment(as_comments item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_comment_" + item.id);
                db.SaveComment(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteComment(as_comments item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_comments" + item.id);
                db.DeleteComment(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }



        public as_comments AddComment(string type, string itemID, string text, string audio = "")
        {
            var res = new as_comments();
            try
            {
                if (audio != string.Empty)
                {
                    res = new as_comments { id = 0, itemID = itemID, type = type, created = DateTime.Now, username = User.UserName, text = text, audio = audio };
                }
                else
                {
                    res = new as_comments { id = 0, itemID = itemID, type = type, created = DateTime.Now, username = User.UserName, text = text };
                }
                db.SaveComment(res);
            }
            catch (Exception ex) { }
            return res;
        }

        public List<string> GetCommentTypes()
        {
            var res = new List<string>();

            var key = "as_comments_types";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<string>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetComments().Select(x => x.type).Distinct().OrderBy(x => x).ToList();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;        

        }

        internal void EditTextField(int pk, string name, string value)
        {
            var item = db.GetComment(pk);
            switch (name)
            {
                case "text": item.text = value; break;

            }
            SaveComment(item);
        }

    }
}