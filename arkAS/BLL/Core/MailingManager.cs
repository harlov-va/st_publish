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
    public class MailingManager
    {
         #region System
        public CoreRepository db;
        private bool _disposed;

        public MailingManager()
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

        #region as_Emails

        public as_emails GetEmail(int id)
        {

            as_emails res = db.GetEmail(id);
            return res;
        }

        public List<as_emails> GetEmails()
        {
            List<as_emails> res = db.GetEmails();
            return res;
        }

        public int SaveEmail(as_emails element)
        {
            try
            {
                db.SaveEmail(element);

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool DeleteEmail(int id)
        {
            bool res = false;
            try
            {
                db.DeleteEmail(id);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }
        #endregion

        #region as_MailingLOG

        public as_mailingLog GetEmailLogItem(int id)
        {

            as_mailingLog res = db.GetEmailLogItem(id);
            return res;
        }

        public List<as_mailingLog> GetMailingLog()
        {
            List<as_mailingLog> res = db.GetMailingLog();
            return res;
        }

        public int SaveMailingLogItem(as_mailingLog element)
        {
            try
            {
                db.SaveMailingLogItem(element);

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return element.id;
        }
        public bool SaveMailingLog(List<as_mailingLog> elements)
        {
            bool res = false;

            try
            {
                db.SaveMailingLog(elements);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }
        public bool DeleteMailingLogItem(int id)
        {
            bool res = false;
            try
            {
                db.DeleteMailingLogItem(id);
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