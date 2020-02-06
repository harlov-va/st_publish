using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace arkAS.BLL.Finance
{
    public class FinanceRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public FinanceRepository()
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

        #region Projects

        public tt_projects GetProject(int id)
        {
            tt_projects res = db.tt_projects.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<tt_projects> GetProjects()
        {
            List<tt_projects> res = db.tt_projects.ToList();
            return res;
        }

        public int SaveProject(tt_projects item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.tt_projects.Add(item);
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

        public bool DeleteProject(int id)
        {
            bool res = false;
            try
            {
                var item = db.tt_projects.SingleOrDefault(x => x.id == id);
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

        #region finStatus

        public fin_statuses GetFinStatus(int id)
        {
            fin_statuses res = db.fin_statuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<fin_statuses> GetFinStatuses()
        {
            List<fin_statuses> res = db.fin_statuses.ToList();
            return res;
        }

        public int SaveFinStatus(fin_statuses item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.fin_statuses.Add(item);
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

        public bool DeleteFinStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.fin_statuses.SingleOrDefault(x => x.id == id);
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
    
        #region finTypes

        public fin_types GetFinType(int id)
        {
            fin_types res = db.fin_types.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<fin_types> GetFinTypes()
        {
            List<fin_types> res = db.fin_types.ToList();
            return res;
        }

        public int SaveFinType(fin_types item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.fin_types.Add(item);
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

        public bool DeleteFinType(int id)
        {
            bool res = false;
            try
            {
                var item = db.fin_types.SingleOrDefault(x => x.id == id);
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

        #region finChannels

        public fin_channels GetFinChannel(int id)
        {
            fin_channels res = db.fin_channels.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<fin_channels> GetFinChannels()
        {
            List<fin_channels> res = db.fin_channels.ToList();
            return res;
        }

        public int SaveFinChannel(fin_channels item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.fin_channels.Add(item);
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

        public bool DeleteFinChannel(int id)
        {
            bool res = false;
            try
            {
                var item = db.fin_channels.SingleOrDefault(x => x.id == id);
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

        #region finContragents

        public fin_contragents GetFinContragent(int id)
        {
            fin_contragents res = db.fin_contragents.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<fin_contragents> GetFinContragents()
        {
            List<fin_contragents> res = db.fin_contragents.ToList();
            return res;
        }

        public int SaveFinContragents(fin_contragents item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.fin_contragents.Add(item);
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

        public bool DeleteFinContragent(int id)
        {
            bool res = false;
            try
            {
                var item = db.fin_contragents.SingleOrDefault(x => x.id == id);
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

        #region finFinances

        public fin_finances GetFinFinance(int id)
        {
            fin_finances res = db.fin_finances.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<fin_finances> GetFinFinances()
        {
            List<fin_finances> res = db.fin_finances.ToList();
            return res;
        }

        public int SaveFinFinance(fin_finances item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.fin_finances.Add(item);
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

        public bool DeleteFinFinance(int id)
        {
            bool res = false;
            try
            {
                var item = db.fin_finances.SingleOrDefault(x => x.id == id);
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

    }
}