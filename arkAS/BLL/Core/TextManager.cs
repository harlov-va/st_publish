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
    public class TextManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public TextManager()
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

        public static string Get(string code)
        {
            var res = "<i>Необходимо отредактировать текст...</i>";
            var mng2 = new TextManager();
            var t = mng2.GetText(code);
            if (t != null && t.text!=null)
            {
                string separator = "|||";
                if (t.text.IndexOf(separator) >= 0)
                {
                    int startPos;
                    int LenCopy;
                    startPos = t.text.IndexOf(separator) + separator.Length;
                    LenCopy = t.text.IndexOf(separator, startPos) - startPos;
                    string url = t.text.Substring(startPos, LenCopy);
                    startPos = t.text.IndexOf(separator, startPos) + separator.Length;
                    LenCopy = t.text.Length - startPos;
                    string alt = t.text.Substring(startPos, LenCopy);
                    res = "<img id=\"" + code + "\" src=\"" + url + "\" alt=\"" + alt + "\" width=\"auto\" height=\"200px\"></img>";
                }
                else
                {
                    res = t.text ?? "";
                }
            }
            return res;
        }

        public List<as_texts> GetTexts()
        {
            var res = new List<as_texts>();
            res = db.GetTexts();
            return res;
        }

        public List<as_texts> GetTexts(string[] codes)
        {
            var res = new List<as_texts>();
            res = db.db.as_texts.Where(x=> codes.Contains(x.code)).ToList();
            return res;
        }


        public as_texts GetText(string code)
        {
            var res = new as_texts();
            var key = "as_text_" + code;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_texts)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = GetTexts().FirstOrDefault(x=>x.code==code);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;  
        }
        public as_texts GetText(int id)
        {
            var res = new as_texts();
            var key = "as_text_" + id.ToString();
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_texts)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = GetTexts().FirstOrDefault(x => x.id == id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public void SaveText(as_texts item)
        {
            try {
                RDL.CacheManager.PurgeCacheItems("as_text_" + item.code);
                db.SaveText(item);
            }catch(Exception ex){
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteText(as_texts item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_text_" + item.code);
                db.DeleteText(item.id);                
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditTextField(int pk, string name, string value)
        {
            var item = db.GetText(pk);
            switch (name)
            {
                case "name": item.name = value; break;
                case "code": item.code = value; break;
                case "categoryName": item.categoryID = RDL.Convert.StrToInt(value, 0); break;
                case "text": item.text = value; break;
                
            }
            SaveText(item);
        }

        public as_texts GetCodeIs(string code)
        {
            as_texts res = db.db.as_texts.SingleOrDefault(el => el.code == code);
            return res;
        }
        public int CodeIs(as_texts item)
        {
            int error = -1;
            using (var dbContextTransaction = db.db.Database.BeginTransaction())
            {
                try
                {
                    RDL.CacheManager.PurgeCacheItems("as_text_" + item.code);
                    as_texts res = db.db.as_texts.SingleOrDefault(el => el.code == item.code);
                    if (res == null)
                    {

                        db.db.as_texts.Add(item);
                        error = db.db.SaveChanges();
                        CacheManager.CacheData("as_text_" + item.code, res);
                    }
                    else
                    {
                        res.code = "Temp|ID" + res.id + "|" + res.code;
                        as_texts resTemp = db.db.as_texts.SingleOrDefault(el => el.code == res.code);
                        item.id = res.id;
                        if (string.IsNullOrEmpty(item.name))
                        {
                            item.name = res.name;
                        }
                        if (resTemp == null)
                        {
                            db.db.Entry(res).State = EntityState.Added;
                            error = db.db.SaveChanges();
                        }
                        else
                        {
                            error = 0;
                        }
                        if (error >= 0)
                        {
                            error = -1;
                            res = db.db.as_texts.SingleOrDefault(el => el.id == item.id);
                            res.text = item.text;
                            res.code = item.code;
                            res.name = item.name;
                            error = db.db.SaveChanges();
                            CacheManager.CacheData("as_text_" + item.code, res);
                        }
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    error = -1;
                    dbContextTransaction.Rollback();
                    Debug.LogError(ex);
                }
            }
            return error;
        }
    }
}