using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arkAS.BLL.Finance;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Management.Instrumentation;
using RDL;

namespace arkAS.BLL.Finance
{
    public class FinanceManager
    {
         #region System
        public FinanceRepository db;
        private bool _disposed;

        public FinanceManager()
        {
            db = new FinanceRepository();
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

        #region projects

        public tt_projects GetProject(int id)
        {
            var res = new tt_projects();
            res = db.GetProject(id);
            return res;
        }

        public List<tt_projects> GetProjects()
        {
            var res = new List<tt_projects>();
            res = db.GetProjects();
            return res;
        }

        public void SaveProject(tt_projects item)
        {
            try
            {
                db.SaveProject(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteProject(int id)
        {
            try
            {
                db.DeleteProject(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        
        #endregion

        #region finStatus
        public fin_statuses GetFinStatus(int id)
        {
            var res = new fin_statuses();
            res = db.GetFinStatus(id);
            return res;
        }

        public List<fin_statuses> GetFinStatuses()
        {
            var res = new List<fin_statuses>();
            res = db.GetFinStatuses();
            return res;
        }

        public void SaveFinStatus(fin_statuses item)
        {
            try
            {
                db.SaveFinStatus(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteFinStatus(int id)
        {
            try
            {
                db.DeleteFinStatus(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        
        #endregion

        #region finTypes

        public fin_types GetFinType(int id)
        {
            var res = new fin_types();
            res = db.GetFinType(id);
            return res;
        }

        public List<fin_types> GetFinTypes()
        {
            var res = new List<fin_types>();
            res = db.GetFinTypes();
            return res;
        }

        public void SaveFinType(fin_types item)
        {
            try
            {
                db.SaveFinType(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteFinType(int id)
        {
            try
            {
                db.DeleteFinType(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region finChannels

        public fin_channels GetFinChannel(int id)
        {
            var res = new fin_channels();
            res = db.GetFinChannel(id);
            return res;
        }

        public List<fin_channels> GetFinChannels()
        {
            var res = new List<fin_channels>();
            res = db.GetFinChannels();
            return res;
        }

        public void SaveFinChannel(fin_channels item)
        {
            try
            {
                db.SaveFinChannel(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteFinChannel(int id)
        {
            try
            {
                db.DeleteFinChannel(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region finContragents

        public fin_contragents GetFinContragent(int id)
        {
            var res = new fin_contragents();
            res = db.GetFinContragent(id);
            return res;
        }

        public List<fin_contragents> GetFinContragents()
        {
            var res = new List<fin_contragents>();
            res = db.GetFinContragents();
            return res;
        }

        public void SaveFinContragents(fin_contragents item)
        {
            try
            {
                db.SaveFinContragents(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditFinContragentField(int pk, string name, string value)
        {
            var item = GetFinContragent(pk);
            switch (name)
            {
                case "name":
                    item.name = value;
                    break;
                case "clientName":
                    item.clientID = (!String.IsNullOrWhiteSpace(value)) ? RDL.Convert.StrToInt(value, 0) : (int?) null;
                    break;
                case "humanName":
                    item.humanID = (!String.IsNullOrWhiteSpace(value)) ? RDL.Convert.StrToInt(value, 0) : (int?)null;
                    break;
            }
            SaveFinContragents(item);
        }

        public bool DeleteFinContragent(int id)
        {
            try
            {
                return db.DeleteFinContragent(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return false;
            }
        }

        #endregion

        #region finFinance

        public fin_finances GetFinFinance(int id)
        {
            var res = new fin_finances();
            res = db.GetFinFinance(id);
            return res;
        }

        public List<fin_finances> GetFinFinances()
        {
            var res = new List<fin_finances>();
            res = db.GetFinFinances();
            return res;
        }

        public void SaveFinFinance(fin_finances item)
        {
            try
            {
                db.SaveFinFinance(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteFinFinance(int id)
        {
            try
            {
                db.DeleteFinFinance(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditFinanceField(int pk, string name, string value)
        {
            var item = GetFinFinance(pk);
            switch (name)
            {
                case "desc": item.desc = value; break;
                case "sum": item.sum = RDL.Convert.StrToInt(value, 0); break;               
                case "fromName":
                    if (value != "")
                        item.fromID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.fromID = null;
                    break;
                case "toName":
                    if (value != "")
                        item.toID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.toID = null;
                    break;
                case "projectName":
                    if (value != "")
                        item.projectID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.projectID = null;
                    break;
                case "typeName":
                    if (value != "")
                        item.typeID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.typeID = null;
                    break;
                case "statusName":
                    if (value != "")
                        item.statusID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.statusID = null;
                    break;
                
            }
            SaveFinFinance(item);
        }

        #endregion
 
    }
}