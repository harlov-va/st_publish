using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;

namespace arkAS.BLL.Core
{
    public class CategoryManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public CategoryManager()
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

        public List<as_categories> GetCategories()
        {
            var res = new List<as_categories>();
            res = db.GetCategories();
            return res;
        }

        public List<as_categories> GetCategories(string code) {
            var res = new List<as_categories>();
            var key = "as_categories_code_" + code;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null) {
                res = (List<as_categories>)CacheManager.Cache[key];
            }else{
                try {
                    res = db.GetCategories(code);
                    CacheManager.CacheData(key, res);
                }catch(Exception ex){
                    Debug.LogError(ex);
                }           
            }
            return res;        
        }

        public as_categories GetCategory(int id)
        {
            var res= db.GetCategory(id);
            return res;
        }

        public List<as_categories> GetCategories(string code, int parentID)
        {
            var res = new List<as_categories>();
            var key = "as_categories_code_" + code + "_parentid_" + parentID;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<as_categories>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = GetCategories(code).Where(x => x.parentID == parentID).OrderBy(x => x.name).ToList();      
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;  
        }

        public void SaveCategory(as_categories item)
        {
            try {
                db.SaveCategory(item);
                RDL.CacheManager.PurgeCacheItems("as_categories");

            }catch(Exception ex){
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteCategory(as_categories item)
        {
            try
            {
                db.DeleteCategory(item.id);
                RDL.CacheManager.PurgeCacheItems("as_categories");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public List<as_categories> GetCategoryChild(int parentID)
        {
            var res = new List<as_categories>();
            res = GetCategories().Where(x => x.parentID == parentID).ToList();
            return res;
        }
        public List<as_texts> GetCategoryText(int id)
        {
            var res = new List<as_texts>();
            var mng =new TextManager();
            res = mng.GetTexts().Where(x => x.categoryID == id).ToList();
            return res;
        }
        public int SetNullCategoryText(int id)
        {
            var list = new List<as_texts>();
            var mng = new TextManager();
            try
            {
                list = mng.GetTexts().Where(x => x.categoryID == id).ToList();
                foreach (as_texts item in list)
                {
                    item.categoryID = null;
                    db.SaveText(item);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
            
        }

    }
}