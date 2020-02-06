using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Dapper;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.IO;
namespace arkAS.BLL.Core
{
    public class CoreRepository
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public CoreRepository()
        {
            db = new LocalSqlServer();
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

        #region Categories
        public List<as_categories> GetCategories()
        {
            var res = new List<as_categories>();
            res = db.as_categories.ToList();
            return res;
        }
        public List<as_categories> GetCategories(string typeCode)
        {
            var res = new List<as_categories>();
            res = db.as_categories.Where(x => x.typeCode == typeCode).ToList();
            return res;
        }

        public as_categories GetCategory(int id)
        {
            var res = new as_categories();
            res = db.as_categories.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveCategory(as_categories element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_categories.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteCategory(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_categories.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion
        public doc_docs GetDocsById(int id)
        {

            doc_docs res = db.doc_docs.FirstOrDefault(x => x.id == id);
            return res;
        }
        #region dataTypes

        public List<as_dataTypes> GetDataTypes()
        {
            var res = new List<as_dataTypes>();
            res = db.as_dataTypes.ToList();
            return res;
        }

        public int SaveDataType(as_dataTypes element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_dataTypes.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        #endregion

        #region settings
        public as_settings GetSetting(int id)
        {

            as_settings res = db.as_settings.FirstOrDefault(x => x.id == id);
            return res;
        }
        public as_settings GetSetting(string code)
        {

            as_settings res = db.as_settings.FirstOrDefault(x => x.code == code);
            return res;
        }

        public List<as_settings> GetSettings()
        {
            List<as_settings> res = db.as_settings.ToList();
            return res;
        }

        public int SaveSetting(as_settings element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_settings.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteSetting(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_settings.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region settingAvailableValues


        public as_settingAvailableValues GetSettingAvailableValue(int id)
        {

            as_settingAvailableValues res = db.as_settingAvailableValues.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_settingAvailableValues> GetSettingAvailableValues()
        {
            List<as_settingAvailableValues> res = db.as_settingAvailableValues.ToList();
            return res;
        }

        public int SaveSettingAvailableValue(as_settingAvailableValues element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_settingAvailableValues.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteSettingAvailableValue(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_settingAvailableValues.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region Menu
        public as_menu GetMenu(int id)
        {

            as_menu res = db.as_menu.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_menu> GetMenu()
        {
            List<as_menu> res = db.as_menu.Include("as_menuRoles").ToList();
            return res;
        }

        public int SaveMenu(as_menu element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_menu.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteMenu(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_menu.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        public List<as_menuRoles> getRolesForMenu(int menuID)
        {
            List<as_menuRoles> res = db.as_menuRoles.Where(x => x.itemID == menuID).ToList();
            return res;
        }

        #endregion

        #region MenuRoles
        public as_menuRoles GetMenuRole(int id)
        {
            as_menuRoles res = db.as_menuRoles.FirstOrDefault(x => x.id == id);
            return res;
        }

        public as_menuRoles GetMenuRole(int itemID, string role)
        {
            as_menuRoles res = db.as_menuRoles.FirstOrDefault(x => x.itemID == itemID && x.role == role);
            return res;
        }

        public List<as_menuRoles> GetMenuRoles()
        {
            List<as_menuRoles> res = db.as_menuRoles.ToList();
            return res;
        }

        public int SaveMenuRole(as_menuRoles element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_menuRoles.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteMenuRole(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_menuRoles.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region SettingCategories


        public as_settingCategories GetSettingCategory(int id)
        {

            as_settingCategories res = db.as_settingCategories.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_settingCategories> GetSettingCategories()
        {
            List<as_settingCategories> res = db.as_settingCategories.ToList();
            return res;
        }

        public int SaveSettingCategory(as_settingCategories element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_settingCategories.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteSettingCategory(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_settingCategories.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region mt_metricTypes


        public as_mt_metricTypes GetMt_metricType(int id)
        {

            as_mt_metricTypes res = db.as_mt_metricTypes.FirstOrDefault(x => x.id == id);
            return res;
        }
        public as_mt_metricTypes GetMt_metricType(string code)
        {

            as_mt_metricTypes res = db.as_mt_metricTypes.FirstOrDefault(x => x.code == code);
            return res;
        }
        public List<as_mt_metricTypes> GetMt_metricTypes()
        {
            List<as_mt_metricTypes> res = db.as_mt_metricTypes.ToList();
            return res;
        }

        public int SaveMt_metricType(as_mt_metricTypes element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_mt_metricTypes.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteMt_metricType(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_mt_metricTypes.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region Mt_metrics


        public as_mt_metrics GetMt_metric(int id)
        {

            as_mt_metrics res = db.as_mt_metrics.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_mt_metrics> GetMt_metrics()
        {
            List<as_mt_metrics> res = db.as_mt_metrics.ToList();
            return res;
        }

        public int SaveMt_metric(as_mt_metrics element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_mt_metrics.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteMt_metrics(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_mt_metrics.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region Statuses
        public as_mt_metrics GetStatus(int id)
        {

            as_mt_metrics res = db.as_mt_metrics.FirstOrDefault(x => x.id == id);
            return res;
        }

        public as_statuses GetStatus(string code, string typecode)
        {

            as_statuses res = db.as_statuses.FirstOrDefault(x => x.code == code && x.typeCode == typecode);
            return res;
        }
        public as_statuses GetStatusById(int id)
        {

            as_statuses res = db.as_statuses.FirstOrDefault(x => x.id == id);
            return res;
        }
        public List<as_statuses> GetStatuses()
        {
            List<as_statuses> res = db.as_statuses.ToList();
            return res;
        }

        public int SaveStatus(as_statuses element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_statuses.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_statuses.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region Trace
        public as_mt_metrics GetTrace(int id)
        {
            as_mt_metrics res = db.as_mt_metrics.FirstOrDefault(x => x.id == id);
            return res;
        }
      
        public as_trace GetTrace(string code, string typecode)
        {

            as_trace res = db.as_trace.FirstOrDefault(x => x.code == code && x.text== typecode);
            return res;
        }
        public as_trace GetTraceById(int id)
        {

            as_trace res = db.as_trace.FirstOrDefault(x => x.id == id);
            return res;
        }
        public List<as_trace> GetTrace(int pageSize,int page)
        {
            List<as_trace> res = db.as_trace.OrderBy(x=>x.id).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return res;
        }

        public int SaveTrace(as_trace element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_trace.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteTrace(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_trace.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion
        public as_sqlGet GetSqlGet(int id)
        {

            as_sqlGet res = db.as_sqlGet.FirstOrDefault(x => x.id == id);
            return res;
        }
        #region Rights

        public as_rights GetRight(int id)
        {

            as_rights res = db.as_rights.FirstOrDefault(x => x.id == id);
            return res;
        }
        public as_sqlGet GetSql(string code)
        {

            as_sqlGet res = db.as_sqlGet.FirstOrDefault(x => x.code == code);
            return res;
        }
        public as_rights GetRight(string code)
        {

            as_rights res = db.as_rights.FirstOrDefault(x => x.code == code);
            return res;
        }
        public List<as_rights> GetRights()
        {
            List<as_rights> res = db.as_rights.ToList();
            return res;
        }
        public int SaveSql(as_sqlGet element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_sqlGet.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public int SaveRight(as_rights element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_rights.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteRight(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_rights.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion
        public int SaveSqlRole(as_sqlRole element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_sqlRole.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        #region RightsRoles


        public as_rightsRoles GetRightsRole(int id)
        {

            as_rightsRoles res = db.as_rightsRoles.FirstOrDefault(x => x.id == id);
            return res;
        }
        public as_rightsRoles GetRightsRole(string role)
        {

            as_rightsRoles res = db.as_rightsRoles.FirstOrDefault(x => x.role == role);
            return res;
        }
        public List<as_rightsRoles> GetRightsRoles()
        {
            List<as_rightsRoles> res = db.as_rightsRoles.ToList();
            return res;
        }

        public int SaveRightRole(as_rightsRoles element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_rightsRoles.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteRightRole(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_rightsRoles.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region StatusLog

        public List<as_statusLog> GetStatusesLog(string typeCode)
        {

            List<as_statusLog> res = db.as_statusLog.Where(x => x.typeCode == typeCode).ToList();
            return res;
        }

        public int SaveStatusLog(as_statusLog element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_statusLog.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteStatusLog(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_statusLog.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region ProfileProperties


        public as_profileProperties GetProfileProperty(int id)
        {

            as_profileProperties res = db.as_profileProperties.FirstOrDefault(x => x.id == id);
            return res;
        }
        public as_profileProperties GetProfileProperty(string code)
        {

            as_profileProperties res = db.as_profileProperties.FirstOrDefault(x => x.code == code);
            return res;
        }

        public List<as_profileProperties> GetProfileProperties()
        {
            List<as_profileProperties> res = db.as_profileProperties.ToList();
            return res;
        }

        public int SaveProfileProperty(as_profileProperties element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_profileProperties.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteProfileProperty(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_profileProperties.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }



        public as_profilePropertyValues GetProfilePropertyValue(int propertyID, Guid userGuid)
        {
            var res = db.as_profilePropertyValues.FirstOrDefault(x => x.propertyID == propertyID && x.userGuid == userGuid);
            return res;
        }

        public int SaveProfilePropertyValue(as_profilePropertyValues element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_profilePropertyValues.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteProfilePropertyValue(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_profilePropertyValues.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region ProfileMenu

        public List<as_profileMenu> GetProfileMenu(Guid userGuid)
        {
            var res = db.as_profileMenu.Where(x => x.userGuid == userGuid).ToList();
            return res;
        }

        public as_profileMenu GetProfileMenu(int id)
        {
            var res = db.as_profileMenu.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveProfileMenu(as_profileMenu element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_profileMenu.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteProfileMenu(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_profileMenu.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region Comments

        public List<as_comments> GetComments()
        {
            var res = new List<as_comments>();

            res = db.as_comments.ToList();

            return res;
        }
        public List<as_comments> GetComments(string type, string itemID)
        {
            var res = new List<as_comments>();

            if (String.IsNullOrEmpty(type) && String.IsNullOrEmpty(itemID))
            {
                res = db.as_comments.ToList();
            }
            else if (String.IsNullOrEmpty(type))
            {
                res = db.as_comments.Where(x => x.itemID == itemID).ToList();
            }
            else if (String.IsNullOrEmpty(itemID))
            {
                res = db.as_comments.Where(x => x.type == type).ToList();
            }
            else
            {
                res = db.as_comments.Where(x => x.type == type && x.itemID == itemID).ToList();
            }

            return res;
        }

        public as_comments GetComment(int id)
        {
            var res = new as_comments();
            res = db.as_comments.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveComment(as_comments element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_comments.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteComment(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_comments.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    File.Delete(item.audio);
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region Opinions

        public as_opinion GetOpinion(int id)
        {
            var res = new as_opinion();
            res = db.as_opinion.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_opinion> GetOpinions()
        {
            var res = new List<as_opinion>();

            res = db.as_opinion.ToList();

            return res;
        }

        public void SaveOpinion(as_opinion item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.as_opinion.Add(item);
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
                        RDL.Debug.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public bool DeleteOpinion(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_opinion.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        #endregion
        public bool DeleteSqlCrud(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_sqlGet.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        public List<as_sqlGet> GetSqlCruds()
        {
            List<as_sqlGet> res = db.as_sqlGet.ToList();
            return res;
        }
        public bool DeleteSqlRole(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_sqlRole.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #region Texts
        public List<as_texts> GetTexts()
        {
            var res = new List<as_texts>();
            res = db.as_texts.ToList();
            return res;
        }
        public List<as_texts> GetTexts(string code)
        {
            var res = new List<as_texts>();
            res = db.as_texts.Where(x => x.code == code).ToList();
            return res;
        }

        public as_texts GetText(int id)
        {
            var res = new as_texts();
            res = db.as_texts.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveText(as_texts element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_texts.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteText(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_texts.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region BackupDatabase
        public Int16 BackupDatabase(ref string DataBaseFile)
        {
            Int16 errors = -1;
            try
            {
                string StrCon = db.Database.Connection.ConnectionString;
                string DataBase;
                int startPos;
                int LenCopy;
                string[] StrFind = new string[] { "database=", "initial catalog=" };
                for (int j = 0; j < StrFind.Length; j++)
                {
                    startPos = StrCon.IndexOf(StrFind[j]);
                    if (startPos < 0)
                    {
                        errors = -2;
                        continue;
                    }
                    else
                    {
                        startPos += StrFind[j].Length;
                        LenCopy = StrCon.IndexOf(";", startPos) - startPos;
                        if (LenCopy < 0)
                        {
                            LenCopy = StrCon.Length - startPos;
                        }
                        if (LenCopy >= StrCon.Length)
                        {
                            LenCopy = 0;
                            errors = -3;
                            continue;
                        }
                        else
                        {
                            DataBase = StrCon.Substring(startPos, LenCopy);

                            string patch = @"C:\temp1\";
                            if (!System.IO.Directory.Exists(patch))
                            {
                                System.IO.Directory.CreateDirectory(patch);
                            }

                            DataBaseFile = patch + DataBase + "-" + String.Format("{0:dd.MM.yyyy}", DateTime.Now) + ".bak";
                            string cmdSql = string.Format("BACKUP DATABASE [{0}] TO DISK = '{1}' WITH INIT;", DataBase, DataBaseFile);
                            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmdSql);

                            errors = 0;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return errors;
        }
        #endregion
        #region UserActions
        public IQueryable<as_userActions> GetUserActions()
        {
            var res = db.as_userActions;
            return res;
        }

        public int SaveUserAction(as_userActions element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_userActions.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteUserAction(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_userActions.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        public List<as_userActions> GetUserActionsList(string username, DateTime dtbeg, DateTime dtend)
        {
            var res = new List<as_userActions>();
            res = db.as_userActions.Where(x => x.username == username && x.created >= dtbeg && x.created < dtend).ToList();
            return res;
        }

        #endregion

        #region as_Mail

        public as_mail GetMail(int id)
        {

            as_mail res = db.as_mail.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_mail> GetMailList()
        {
            List<as_mail> res = db.as_mail.ToList();
            return res;
        }

        public int SaveMailItem(as_mail element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_mail.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }

        public bool DeleteMail(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_mail.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region as_MailLOG

        public as_mailLog GetMailLog(int id)
        {

            as_mailLog res = db.as_mailLog.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_mailLog> GetMailLog()
        {
            List<as_mailLog> res = db.as_mailLog.ToList();
            return res;
        }

        public int SaveMailLogItem(as_mailLog element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_mailLog.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool SaveMailLogList(List<as_mailLog> elements)
        {
            bool res = false;

            try
            {
                db.as_mailLog.AddRange(elements);
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }
        public bool DeleteMailLog(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_mailLog.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region as_mailAttachment

        public as_mailAttachment GetMailAttachment(int id)
        {

            as_mailAttachment res = db.as_mailAttachment.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<as_mailAttachment> GetMailAttachments()
        {
            List<as_mailAttachment> res = db.as_mailAttachment.ToList();
            return res;
        }

        public int SaveMailAttachmentItem(as_mailAttachment element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_mailAttachment.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool SaveMailAttachmentList(List<as_mailAttachment> elements)
        {
            bool res = false;

            try
            {
                db.as_mailAttachment.AddRange(elements);
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }
        public bool DeleteAttachment(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_mailAttachment.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region UserCheckSets
        public List<as_userCheckSet> GetUserCheckSets()
        {
            var res = new List<as_userCheckSet>();
            res = db.as_userCheckSet.ToList();
            return res;
        }

        public as_userCheckSet GetUserCheckSet(int id)
        {
            var res = new as_userCheckSet();
            res = db.as_userCheckSet.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveUserCheckSet(as_userCheckSet element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_userCheckSet.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteUserCheckSet(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_userCheckSet.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region UserCheckItems
        public List<as_userCheckItems> GetUserCheckItems(bool withSets = false)
        {
            var res = new List<as_userCheckItems>();

            //if we need relative data from sets 
            if (withSets)
            {
                res = db.as_userCheckItems.Include(x => x.as_userCheckSet).ToList();
            }
            else
            {
                res = db.as_userCheckItems.ToList();
            }

            return res;
        }

        public as_userCheckItems GetUserCheckItem(int id)
        {
            var res = new as_userCheckItems();
            res = db.as_userCheckItems.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveUserCheckItem(as_userCheckItems element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_userCheckItems.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteUserCheckItem(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_userCheckItems.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region UserChecks
        public List<as_userChecks> GetUserChecks()
        {
            var res = new List<as_userChecks>();
            res = db.as_userChecks.ToList();
            return res;
        }

        public as_userChecks GetUserCheck(int id)
        {
            var res = new as_userChecks();
            res = db.as_userChecks.FirstOrDefault(x => x.id == id);
            return res;
        }
        public as_userChecks GetUserCheck(int idCheckItem, string nameUser)
        {
            var res = new as_userChecks();
            res = db.as_userChecks.FirstOrDefault(x => x.itemID == idCheckItem && x.username.Equals(nameUser));
            return res;
        }

        public int SaveUserCheck(as_userChecks element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.as_userChecks.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteUserCheck(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_userChecks.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region HotKeys
        public List<as_hotkeys> GetHotKeys()
        {
            List<as_hotkeys> res = db.as_hotkeys.ToList();
            return res;
        }

        public as_hotkeys GetHotKey(int id)
        {
            as_hotkeys res = db.as_hotkeys.FirstOrDefault(x => x.Id == id);
            return res;
        }

        public int SaveHotKey(as_hotkeys element)
        {
            try
            {
                if (element.Id == 0)
                {
                    db.as_hotkeys.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        RDL.Debug.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.Id;
        }
        public bool DeleteHotKey(int id)
        {
            bool res = false;
            try
            {
                var item = db.as_hotkeys.SingleOrDefault(x => x.Id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region as_ctrl
        public List<as_ctrl> GetItemsAsCtrl()
        {
            return db.as_ctrl.ToList();
        }

        public as_ctrl GetItemAsCtrl(int id)
        {
            return db.as_ctrl.FirstOrDefault(x => x.id == id);
        }

        public int SaveItemAsCtrl(as_ctrl item, ref int error)
        {

            if (item.id == 0)
            {
                error = -2;

                db.as_ctrl.Add(item);
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
                    error = -3;
                    RDL.Debug.LogError(ex);
                }
            }
            return item.id;
        }

        public void DeleteItemAsCtrl(int id)
        {
            var cmpMonit = db.as_ctrl.SingleOrDefault(x => x.id == id);
            if (cmpMonit != null)
            {
                db.Entry(cmpMonit).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
        }
        #endregion
        public int SaveSqlCrud(as_sqlGet item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.as_sqlGet.Add(item);
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
                        RDL.Debug.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return item.id;
        }
        public IQueryable<aspnet_Users> GetUsers()
        {
            return db.aspnet_Users;
        } 

        internal object GetUserAction()
        {
            throw new NotImplementedException();
        }

        public T GetSQLData<T>(string sql, object parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure);
                    return (T)els;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(T);
            }
        }
    }


}