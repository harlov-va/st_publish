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
    public class MailManager
    {
         #region System
        public CoreRepository db;
        private bool _disposed;

        public MailManager()
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

        #region as_Mail

        public as_mail GetMail(int id)
        {

            as_mail res = db.GetMail(id);
            return res;
        }

        public List<as_mail> GetMailList()
        {
            List<as_mail> res = db.GetMailList();
            return res;
        }

        public int SaveMailItem(as_mail element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.SaveMailItem(element);
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
                db.DeleteMail(id);
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

            as_mailLog res = db.GetMailLog(id);
            return res;
        }

        public List<as_mailLog> GetMailLog()
        {
            List<as_mailLog> res = db.GetMailLog();
            return res;
        }

        public int SaveMailLogItem(as_mailLog element)
        {
            try
            {
                db.SaveMailLogItem(element);

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
                db.SaveMailLogList(elements);
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

            return res;
        }
        public bool DeleteMailLogItem(int id)
        {
            bool res = false;
            try
            {
                db.DeleteMailLog(id);
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

            as_mailAttachment res = db.GetMailAttachment(id);
            return res;
        }

        public List<as_mailAttachment> GetMailAttachments()
        {
            List<as_mailAttachment> res = db.GetMailAttachments();
            return res;
        }

        public int SaveMailAttachmentItem(as_mailAttachment element)
        {
            try
            {
                db.SaveMailAttachmentItem(element);

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
                db.SaveMailAttachmentList(elements);
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
                 db.DeleteAttachment(id);
                
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