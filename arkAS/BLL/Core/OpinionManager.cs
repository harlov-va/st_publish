using RDL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Core
{
    public class OpinionManager
    {

        //#region System

        //public LocalSqlServer db;
        //private bool _disposed;

        //public OpinionManager()
        //{
        //    db = new LocalSqlServer();
        //    _disposed = false;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //            if (db != null)
        //                db.Dispose();
        //        }
        //        db = null;
        //        _disposed = true;
        //    }
        //}

        //#endregion


        #region System
        public CoreRepository db;
        private bool _disposed;

        public OpinionManager()
        {
            db = new CoreRepository();
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

        private const string CachePrefix = "as_opinion_";

        private string GetCacheKey(int itemID)
        {
            return string.Format("{0}{1}", CachePrefix, itemID);
        }

        private string GetCacheKey(as_opinion item)
        {
            return GetCacheKey(item.itemID ?? 0);
        }

        public List<string> GetCacheOpinions()
        {
            var res = new List<string>();

            IDictionaryEnumerator enumerator = CacheManager.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(CachePrefix))
                    res.Add(enumerator.Value.ToString());
            }

            return res;
        }

        public int SaveOpinion(as_opinion item, int cacheDuration = 0)
        {
            try
            {
                db.SaveOpinion(item);
                if (cacheDuration > 0)
                {
                    CacheManager.CacheData(GetCacheKey(item.itemID ?? 0), item.itemID ?? 0, cacheDuration);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return item.id;
        }

        public List<string> GetOpinionTypes()
        {
            var res = new List<string>();

            var key = "as_opinion_types";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<string>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetOpinions().Select(x => x.type).Distinct().OrderBy(x => x).ToList();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;

        }

        public as_opinion GetOpinion(int id)
        {
            var res = db.GetOpinion(id);

            return res;
        }

        public void EditTextField(int pk, string name, string value)
        {
            var item = db.GetOpinion(pk);
            switch (name)
            {
                case "comment": item.comment = value; break;
                case "like": item.like = System.Convert.ToBoolean(value); break;
            }
            SaveOpinion(item);
        }

        public void DeleteOpinion(as_opinion item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_opinion_" + item.id);
                db.DeleteOpinion(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
    }
}