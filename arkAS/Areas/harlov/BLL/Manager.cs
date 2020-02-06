using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.BLL
{
    public class Manager : IManager
    {
        #region System
        private IRepository _db;
        private bool _disposed;
        public Manager(IRepository db)
        {
            _db = db;
            _disposed = true;
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
                    if (_db != null) _db.Dispose();
                }
                _db = null;
                _disposed = true;
            }
        }
        #endregion
        IContragentsManager _contragents;
        IDocumentManager _documents;
        IInvoicesManager _invoices;
        IMailsManager _mails;
        public IContragentsManager Contragents
        {
            get
            {
                if (_contragents == null)
                    _contragents = new ContragentsManager(_db);
                return _contragents;
            }
            set
            {
                _contragents = value;
            }
        }
        public IDocumentManager Documents
        {
            get
            {
                if (_documents == null)
                    _documents = new DocumentsManager(_db);
                return _documents;
            }
            set
            {
                _documents = value;
            }
        }
        public IInvoicesManager Invoices
        {
            get
            {
                if (_invoices == null)
                    _invoices = new InvoicesManager(_db);
                return _invoices;
            }
            set
            {
                _invoices = value;
            }
        }
        public IMailsManager Mails
        {
            get
            {
                if (_mails == null)
                    _mails = new MailsManager(_db);
                return _mails;
            }
            set
            {
                _mails = value;
            }
        }
        public aspnet_Users GetUser()
        {
            string userName = "";
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                userName = HttpContext.Current.User.Identity.Name;
                return _db.GetUsers().FirstOrDefault(x => x.UserName == userName);
            }
            return null;
        }
    }
}