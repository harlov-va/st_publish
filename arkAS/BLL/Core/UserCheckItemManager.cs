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
    public class UserCheckItemManager
    {
        #region System
        public CoreRepository db;
        private bool _disposed;

        public UserCheckItemManager()
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

        public List<as_userCheckItems> GetUserCheckItems(bool withSets = false)
        {
            var res = new List<as_userCheckItems>();
            res = db.GetUserCheckItems(withSets);
            return res;
        }

        public as_userCheckItems GetUserCheckItem(int id)
        {
            var res = new as_userCheckItems();
            //var key = "as_userCheckItems_" + id.ToString();
            //if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            //{
            //    res = (as_userCheckItems)CacheManager.Cache[key];
            //}
            //else
            //{
            try
            {
                res = db.GetUserCheckItem(id);
                //CacheManager.CacheData(key, res);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            //}

            return res;
        }

        public void SaveUserCheckItem(as_userCheckItems item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_userCheckItems_" + item.id);
                db.SaveUserCheckItem(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteUserCheckItem(as_userCheckItems item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_userCheckItems_" + item.id);
                db.DeleteUserCheckItem(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public as_userCheckItems AddUserCheckItem(string name, string description, string emailText, string emailSubject, int setID)
        {
            var res = new as_userCheckItems();
            var date = DateTime.Now;
            try
            {
                res = new as_userCheckItems { id = 0, name = name, description = description, emailText = emailText, emailSubject = emailSubject, setID = setID, created = date };
                db.SaveUserCheckItem(res);
            }
            catch (Exception ex) { }
            return res;
        }
        public void EditUserCheckItem(int id, string name, string description, string emailText, string emailSubject, int setID)
        {
            var element = db.GetUserCheckItem(id);
            try
            {
                element.name = name;
                element.description = description;
                element.emailText = emailText;
                element.emailSubject = emailSubject;
                element.setID = setID;

                db.SaveUserCheckItem(element);
            }
            catch (Exception ex) { }
        }

        internal void EditRightField(int pk, string name, string value)
        {
            var item = GetUserCheckItem(pk);
            switch (name)
            {
                case "name": item.name = value; break;
                case "description": item.description = value; break;
                case "emailText": item.emailText = value; break;
                case "emailSubject": item.emailSubject = value; break;
                case "setID": item.setID = System.Convert.ToInt32(value); break;

            }
            SaveUserCheckItem(item);
        }
       
    }
}