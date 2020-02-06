using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Calc
{
    public class CalcManager
    {
        #region System
        
        public CalcRepository db;
        private bool _disposed;

        public CalcManager()
        {
            db = new CalcRepository();
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

        #region calc_parameters
        public calc_parameters GetCalcParameter(int id)
        {
            var res = new calc_parameters();
            res = db.GetCalcParameter(id);
            return res;
        }

        public List<calc_parameters> GetCalcParameters()
        {
            var res = new List<calc_parameters>();
            res = db.GetCalcParameters();
            return res;

        }

        public void SaveCalcParameter(calc_parameters item)
        {
            try
            {
                db.SaveCalcParameter(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteCalcParameter(int id)
        {
            try
            {
                db.DeleteCalcParameter(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditCalcField(int pk, string name, string value)
        {
            var item = GetCalcParameter(pk);
            switch (name)
            {
                case "name": item.name = value; break;
                case "code": item.code = value; break;
                case "calcName": if (value != "") item.calcID = RDL.Convert.StrToInt(value, 0); break;
                case "dataTypeName": if (value != "") item.datatypeID = RDL.Convert.StrToInt(value, 0); break;

            }
            SaveCalcParameter(item);
        }
        #endregion
  
        #region crm_calcs
        public List<calc_calcs> GetCalcs()
        {
            var res = new List<calc_calcs>();
            res = db.GetCalcs();
            return res;

        }
        #endregion
 
        #region calcDataTypes
        public List<as_dataTypes> GetDataTypes()
        {
            var res = new List<as_dataTypes>();
            res = db.GetDataTypes();
            return res;

        }
        #endregion
    }
}