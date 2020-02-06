using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Calendar
{
    public class CalendarManager
    {
        #region System
        
        public CalendarRepository db;
        private bool _disposed;

        public CalendarManager()
        {
            db = new CalendarRepository();
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

        private void _debug(Exception ex, Object parameters, string additions = "")
        {
            RDL.Debug.LogError(ex, additions, parameters);
        }
        
        #endregion

        #region Events

        public List<rosh_CalendarEvents> GetEvents()
        {
            var res = new List<rosh_CalendarEvents>();
            try
            {
                res = db.GetEvents().ToList();
            }
            catch (Exception ex)
            {
                _debug(ex, new { }, "");
            }
            return res;
        }

        public rosh_CalendarEvents GetEvent(int id)
        {
            var res = new rosh_CalendarEvents();
            try
            {
                res = db.GetEvent(id);
            }
            catch (Exception ex)
            {
                _debug(ex, new { id }, "");
            }
            return res;
        }

        #endregion
    }
}