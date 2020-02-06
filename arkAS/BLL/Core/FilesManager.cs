using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Web;
using RDL;

namespace arkAS.BLL.Core
{
    public class FilesManager
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public FilesManager()
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

        private const string CachePrefix = "as_files_";

        private string GetCacheKey(int itemID)
        {
            return string.Format("{0}{1}", CachePrefix, itemID);
        }

        private string GetCacheKey(as_files item)
        {
            return GetCacheKey(item.itemID ?? 0);
        }

        internal void UploadFile(as_files item, HttpPostedFileBase file)
        {
            var folderPath = HttpContext.Current.Server.MapPath(GetFileFolderLink(item));
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            file.SaveAs(Path.Combine(folderPath, item.filename));
        }

        internal string GetFileFolderLink(as_files item)
        {
            string res;
            res = string.Format("/uploads/images/{0}", item.itemID);
            return res;
        }

        public List<as_files> GetFiles(int itemID)
        {
            var res = new List<as_files>();

            var key = GetCacheKey(itemID);
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                return (List<as_files>)CacheManager.Cache[key];
            }

            res = db.as_files.Where(x => x.itemID == itemID).OrderBy(x => x.ord).ToList();
            CacheManager.CacheData(key, res);

            return res;
        }

        public as_files GetFile(int id)
        {
            return db.as_files.Find(id);
        }

        public int SaveFile(as_files item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.as_files.Add(item);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        Debug.LogError(ex);
                    }
                }
                CacheManager.PurgeCacheItems(GetCacheKey(item));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return item.id;
        }

        internal bool DeleteFile(int id)
        {
            var res = false;
            var item = db.as_files.SingleOrDefault(x => x.id == id);

            try
            {
                if (item != null)
                {                   
                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();

                    var folderPath = HttpContext.Current.Server.MapPath(GetFileFolderLink(item));

                    File.Delete(Path.Combine(folderPath, item.filename));
                }
                res = true;
                CacheManager.PurgeCacheItems(GetCacheKey(item));

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return res;
        }

        public int GetNextOrder()
        {            
            return (db.as_files.Max(f => f.ord) ?? 0) + 1;
        }

        public void UpdateFilesOrder(int itemID, string type, IEnumerable<int> ids)
        {            
            var files = db.as_files.Where(x => x.itemID == itemID).ToArray();
            var order = 1;
            foreach (var id in ids)
            {
                var file = files.First(f => f.id == id);
                if (file != null)
                {
                    file.ord = order++;
                }
            }
            db.SaveChanges();
            CacheManager.PurgeCacheItems(GetCacheKey(itemID));
        }
    }
}