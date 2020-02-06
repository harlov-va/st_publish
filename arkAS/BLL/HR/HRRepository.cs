using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace arkAS.BLL.HR
{
    public class HRRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public HRRepository()
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
      
        #region Humans

        public List<hr_humans> GetHumans()
        {
            var res = new List<hr_humans>();
            res = db.hr_humans.ToList();
            return res;
        }

        public hr_humans GetHuman(int id)
        {
            var res = new hr_humans();
            res = db.hr_humans.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveHuman(hr_humans item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.hr_humans.Add(item);
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

        public bool DeleteHuman(int id)
        {
            bool res = false;
            try
            {
                var item = db.hr_humans.SingleOrDefault(x => x.id == id);
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
        public List<hr_humans> GetHumanSpecs()
        {
            return db.hr_humans.Include(x => x.hr_humanSpecializations).OrderBy(x => x.fio).ToList();
        }
        #endregion

        #region humanStatuses

        public List<hr_statuses> GetHumanStatuses()
        {
            var res = new List<hr_statuses>();
            res = db.hr_statuses.ToList();
            return res;
        }

        public hr_statuses GetHumanStatus(int id)
        {
            var res = new hr_statuses();
            res = db.hr_statuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveHumanStatus(hr_statuses item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.hr_statuses.Add(item);
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

        public bool DeleteHumanStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.hr_statuses.SingleOrDefault(x => x.id == id);
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

        #region Sources

        public List<hr_sources> GetHumanSources()
        {
            var res = new List<hr_sources>();
            res = db.hr_sources.ToList();
            return res;
        }

        public hr_sources GetHumanSource(int id)
        {
            var res = new hr_sources();
            res = db.hr_sources.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveHumanSource(hr_sources item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.hr_sources.Add(item);
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

        public bool DeleteHumanSource(int id)
        {
            bool res = false;
            try
            {
                var item = db.hr_sources.SingleOrDefault(x => x.id == id);
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

        #region HumanSpec
        public int SaveHumanSpec(int idHuman, int idSpec)
        {
            try
            {
                var item = db.hr_humanSpecializations.FirstOrDefault(x => x.humanID == idHuman && x.specializationID == idSpec);
                if ( item == null)
                {
                    item = new hr_humanSpecializations { humanID = idHuman, specializationID = idSpec };
                    db.hr_humanSpecializations.Add(item);
                    db.SaveChanges();
                }
                return item.id;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return 0;
        }
        public bool DeleteHumanSpec(int idHuman, int idSpec)
        {
            bool res = false;
            try
            {
                var item = db.hr_humanSpecializations.SingleOrDefault(x => x.humanID == idHuman && x.specializationID == idSpec);
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

        #region Specialization
        public List<hr_specializations> GetSpecs()
        {
            var res = new List<hr_specializations>();
            res = db.hr_specializations.ToList();
            return res;
        }
        public int SaveSpec(hr_specializations item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.hr_specializations.Add(item);
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
        public bool DeleteSpec(int id)
        {
            bool res = false;
            try
            {
                var item = db.hr_specializations.SingleOrDefault(x => x.id == id);
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
        public hr_specializations GetSpec(int id)
        {
            var res = new hr_specializations();
            res = db.hr_specializations.FirstOrDefault(x => x.id == id);
            return res;
        }
        public bool IsSpecInUse(int idSpec)
        {
            return db.hr_humanSpecializations.Count(x => x.specializationID == idSpec) > 0;
        }
 
        #endregion
    }
}