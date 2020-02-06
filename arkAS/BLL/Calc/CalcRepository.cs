using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Calc
{
    public class CalcRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public CalcRepository()
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

        #region Calc_Parameters
        public calc_parameters GetCalcParameter(int id)
        {
            var res = db.calc_parameters.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<calc_parameters> GetCalcParameters()
        {
            var res = db.calc_parameters.ToList();
            return res;

        }

        public int SaveCalcParameter(calc_parameters item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.calc_parameters.Add(item);
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
        public bool DeleteCalcParameter(int id)
        {
            var res = false;

            try
            {
                var item = db.calc_parameters.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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
        
        #region calc_calcs

        public List<calc_calcs> GetCalcs()
        {
            var res = db.calc_calcs.ToList();
            return res;

        }
        #endregion
      
        #region calc_dataTypes

        public List<as_dataTypes> GetDataTypes()
        {
            var res = db.as_dataTypes.ToList();
            return res;

        }
        #endregion
    }
}