using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Calendar
{
    public class CalendarRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public CalendarRepository()
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

        #region Events

        public IQueryable<rosh_CalendarEvents> GetEvents()
        {
            var events = db.rosh_CalendarEvents;
            return events;
        }

        public rosh_CalendarEvents GetEvent(int id)
        {
            var res = new rosh_CalendarEvents();
            res = db.rosh_CalendarEvents.FirstOrDefault(c => c.id == id);
            return res;
        }
        #endregion
    }
}