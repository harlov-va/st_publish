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
    public class SettingsManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public SettingsManager()
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

        public as_settings GetSetting(int id)
        {
            var res = new as_settings();
            var key = "as_settings_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null) {
                res = (as_settings)CacheManager.Cache[key];
            }else{
                try {
                    res = db.GetSetting(id);
                    CacheManager.CacheData(key, res);
                }catch(Exception ex){
                    Debug.LogError(ex);
                }           
            }
            return res;        
        }


        public string GetSetting(string code, string defValue)
        {
            var res = "";
            var item = GetSetting(code);
            if (item != null && item.id > 0)
            {
                res = item.value;
            }
            else {
                res = defValue;
            }
            return res;
        }


        public as_settings GetSetting(string code)
        {
            var res = new as_settings();
            var key = "as_settings_code_" + code;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_settings)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSetting(code);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }

        public List<as_settings> GetSettings()
        {
            var res = new List<as_settings>();
            var key = "as_settings";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null) {
                res = (List<as_settings>)CacheManager.Cache[key];
            }else{
                try {
                    res = db.GetSettings();
                    CacheManager.CacheData(key, res);
                }catch(Exception ex){
                    Debug.LogError(ex);
                }           
            }
            return res;        
        }
        
       

        public bool SaveSetting(as_settings item)
        {
            try {

                db.SaveSetting(item);

                RDL.CacheManager.PurgeCacheItems("as_settings");
                return true;
            }
            catch (Exception ex){
                RDL.Debug.LogError(ex);
                return false;
            }
        }
        public void DeleteSetting(as_settings item)
        {
            try
            {

                RDL.CacheManager.PurgeCacheItems("as_settings");
                db.DeleteSetting(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public as_settingCategories GetSettingCategories(int id)
        {
            var res = new as_settingCategories();
            var key = "as_settingcategories_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (as_settingCategories)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSettingCategory(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }

        public List<as_settingCategories> GetSettingCategories()
        {
            var res = new List<as_settingCategories>();
            var key = "as_settingcategories";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null)
            {
                res = (List<as_settingCategories>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSettingCategories();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public List<as_settings> GetSettingsByCategoryID(int categoryID) 
        {
            var res = new List<as_settings>();
            res = GetSettings().Where(x => x.categoryID == categoryID).ToList();
            return res;

        }
        public List<as_settingAvailableValues> GetSettingsAvalValByID(int id)
        {
            var res = new List<as_settingAvailableValues>();
            res = db.GetSettingAvailableValues().Where(x => x.settingID == id).ToList();
            return res;

        }
        public int DeleteSettingsAvalValueById(int id)
        {           
            var list = new List<as_settingAvailableValues>();
            try
            {
                list = db.GetSettingAvailableValues().Where(x => x.settingID == id).ToList();
                foreach (as_settingAvailableValues item in list)
                {
                    db.DeleteSettingAvailableValue(item.id);
                }
                return 1;
            }
            catch (Exception ex)
            {               
                RDL.Debug.LogError(ex);
                return -1;
            }                      
        }
        public void SaveSettingCategory(as_settingCategories item)
        {
            try
            {

                db.SaveSettingCategory(item);

                RDL.CacheManager.PurgeCacheItems("as_settingcategories");

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteSettingCategory(as_settingCategories item)
        {
            try
            {

                RDL.CacheManager.PurgeCacheItems("as_settingcategories");
                db.DeleteSettingCategory(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal bool EditSettingField(int pk, string name, string value)
        {
            try
            {
                var item = GetSetting(pk);
                switch (name)
                {
                    case "name": item.name = value; break;
                    case "code": item.code = value; break;
                    case "categoryName": item.categoryID = RDL.Convert.StrToInt(value, 0); break;
                    case "value": item.value = value; break;
                    case "value2": item.value2 = value; break;
                }
                SaveSetting(item);
                return true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return false;
            }
        }
       
    }
}