using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.HR
{
    public class HRManager
    {
        #region System
        public HRRepository db;
        private bool _disposed;

        public HRManager()
        {
            db = new HRRepository();
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

        #region human

        internal hr_humans GetHuman(int id)
        {
            var res = new hr_humans();          
            res = db.GetHuman(id);
            return res;
        }

        internal List<hr_humans> GetHumans()
        {
           var res = new List<hr_humans>();
           res = db.GetHumans();
           return res;
        }

        internal void SaveHuman(hr_humans item)
        {
            try
            {
                db.SaveHuman(item);                
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void DeleteHuman(int id)
        {
            try
            {               
                db.DeleteHuman(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
       
        internal void EditHumanField(int pk, string name, string value)
        {
            var item = db.GetHuman(pk);
            switch (name)
            {
                case "fio": item.fio = value; break;
                case "city": item.city = value; break;
                case "note": item.note = value; break;
                case "sourceName":
                    if (value != "")
                        item.sourceID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.sourceID = null;
                    break;
                case "statusName":
                    if (value != "")
                        item.statusID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.statusID = null;
                    break;
                case "addedBy": item.addedBy = value; break;
                case "subchannel": item.subchannel = value; break;
                case "username": item.username = value; break;
                case "needActive": item.needActive = (value == "Да"); break;
                case "pay": item.pay = value; break;
                case "hourRate": item.hourRate = RDL.Convert.StrToDecimal(value, 0); break;
            }
            db.SaveHuman(item);
        }
        internal List<hr_humans> GetHumanSpecs()
        {
            return db.GetHumanSpecs();
        }

        #endregion
        
        #region humanStatus

        internal hr_statuses GetHumanStatus(int id)
        {
            var res = new hr_statuses();
            res = db.GetHumanStatus(id);
            return res;
        }

        internal List<hr_statuses> GetHumanStatuses()
        {
            var res = new List<hr_statuses>();
            res = db.GetHumanStatuses();
            return res;
        }

        internal void SaveHumanStatus(hr_statuses item)
        {
            try
            {
                db.SaveHumanStatus(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void DeleteHumanStatus(int id)
        {
            try
            {
                db.DeleteHumanStatus(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
       
        #endregion

        #region humanSource

        internal hr_sources GetHumanSource(int id)
        {
            var res = new hr_sources();
            res = db.GetHumanSource(id);
            return res;
        }

        internal List<hr_sources> GetHumanSources()
        {
            var res = new List<hr_sources>();
            res = db.GetHumanSources();
            return res;
        }

        internal void SaveHumanSource(hr_sources item)
        {
            try
            {
                db.SaveHumanSource(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteHumanSource(int id)
        {
            try
            {
                db.DeleteHumanSource(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region humanSpec
        internal void SaveHumanSpec(int idHuman, int idSpec)
        {
            try
            {
                db.SaveHumanSpec(idHuman, idSpec);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void DeleteHumanSpec(int idHuman, int idSpec)
        {
            try
            {
                db.DeleteHumanSpec( idHuman, idSpec);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        #endregion

        #region Specialization
        internal List<hr_specializations> GetSpecs()
        {
            var res = new List<hr_specializations>();
            res = db.GetSpecs();
            return res;
        }
        internal void SaveSpec(hr_specializations item)
        {
            try
            {
                db.SaveSpec(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        internal void DeleteSpec(int id)
        {
            try
            {
                db.DeleteSpec(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        internal hr_specializations GetSpec(int id)
        {
            var res = new hr_specializations();
            res = db.GetSpec(id);
            return res;
        }

        internal bool IsSpecInUse(int idSpec)
        {
            return db.IsSpecInUse(idSpec);
        }
        #endregion
    }
}