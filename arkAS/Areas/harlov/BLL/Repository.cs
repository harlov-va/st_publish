using arkAS.Areas.harlov.BLL;
using arkAS.BLL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov
{
    public class Repository : IDisposable, IRepository
    {
        #region System
        private LocalSqlServer _db;
        public LocalSqlServer db
        {
            get
            {
                if (_db == null)
                    _db = new LocalSqlServer();
                return _db;
            }
            set
            {
                _db = value;
            }
        }
        private bool _disposed = false;
        public Repository(LocalSqlServer db)
        {
            if (db == null) this.db = new LocalSqlServer();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (db != null) Dispose(true);
                }
                db = null;
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Save()
        {
            db.SaveChanges();
        }
        public List<T> GetListSQLData<T>(string sql, object parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<T>(sql, parameters, commandType: type);
                    return els as List<T>;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(List<T>);
            }
        }
        public T GetSQLData<T>(string sql, object parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<T>(sql, parameters, commandType: type);
                    return (T)els;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(T);
            }
        }
        #endregion
        #region contragents
        public IQueryable<h_contragents> GetContragents()
        {
            var res = db.h_contragents;
            return res;
        }
        public h_contragents GetContragent(int ID)
        {
            var res = db.h_contragents.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveContragent(h_contragents element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_contragents.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = System.Data.Entity.EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteContragent(int ID)
        {
            bool res = false;
            var item = db.h_contragents.SingleOrDefault(x => x.id == ID);
            if (item != null)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region invoiceStatuses
        public IQueryable<h_invoiceStatuses> GetInvoiceStatuses()
        {
            var res = db.h_invoiceStatuses;
            return res;
        }
        public h_invoiceStatuses GetInvoiceStatus(int ID)
        {
            var res = db.h_invoiceStatuses.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveInvoiceStatus(h_invoiceStatuses element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_invoiceStatuses.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = System.Data.Entity.EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteInvoiceStatus(int ID)
        {
            bool res = false;
            var item = db.h_invoiceStatuses.SingleOrDefault(x => x.id == ID);
            if (item != null)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region invoices
        public IQueryable<h_invoices> GetInvoices()
        {
            var res = db.h_invoices.Include(s => s.h_invoiceStatuses);
            return res;
        }
        public h_invoices GetInvoice(int ID)
        {
            var res = db.h_invoices.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveInvoice(h_invoices element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_invoices.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteInvoice(int ID)
        {
            bool res = false;
            var item = db.h_invoices.SingleOrDefault(x => x.id == ID);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region deliverySystems
        public IQueryable<h_deliverySystems> GetDeliverySystems()
        {
            var res = db.h_deliverySystems;
            return res;
        }
        public h_deliverySystems GetDeliverySystem(int ID)
        {
            var res = db.h_deliverySystems.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveDeliverySystem(h_deliverySystems element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_deliverySystems.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteDeliverySystem(int ID)
        {
            bool res = false;
            var item = db.h_deliverySystems.SingleOrDefault(x => x.id == ID);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region mailStatuses
        public IQueryable<h_mailStatuses> GetMailStatuses()
        {
            var res = db.h_mailStatuses;
            return res;
        }
        public h_mailStatuses GetMailStatus(int ID)
        {
            var res = db.h_mailStatuses.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveMailStatus(h_mailStatuses element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_mailStatuses.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteMailStatus(int ID)
        {
            bool res = false;
            var item = db.h_mailStatuses.SingleOrDefault(x => x.id == ID);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region h_mails
        public IQueryable<h_mails> GetMails()
        {
            var res = db.h_mails
                .Include(s => s.h_mailStatuses)
                .Include(d => d.h_deliverySystems)
                ;
            return res;
        }
        public h_mails GetMail(int ID)
        {
            var res = db.h_mails.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveMail(h_mails element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_mails.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteMail(int ID)
        {
            bool res = false;
            var item = db.h_mails.SingleOrDefault(x => x.id == ID);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region h_docTypes
        public IQueryable<h_docTypes> GetDocTypes()
        {
            var res = db.h_docTypes;
            return res;
        }
        public h_docTypes GetDocType(int ID)
        {
            var res = db.h_docTypes.FirstOrDefault(x => x.id == ID);
            return res;
        }
        public int SaveDocType(h_docTypes element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_docTypes.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteDocType(int id)
        {
            bool res = false;
            var item = db.h_docTypes.SingleOrDefault(x => x.id == id);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region h_docStatuses
        public IQueryable<h_docStatuses> GetDocStatuses()
        {
            var res = db.h_docStatuses;
            return res;
        }
        public h_docStatuses GetDocStatus(int id)
        {
            var res = db.h_docStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }
        public int SaveDocStatus(h_docStatuses element, bool withSave = true)
        {
            if (element.id == 0)
            {
                db.h_docStatuses.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteDocStatus(int id)
        {
            bool res = false;
            var item = db.h_docStatuses.SingleOrDefault(x => x.id == id);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region h_documents
        public IQueryable<h_documents> GetDocuments()
        {
            var res = db.h_documents;
            return res;
        }
        public h_documents GetDocument(int id)
        {
            var res = db.h_documents.FirstOrDefault(x => x.id == id);
            return res;
        }
        public int SaveDocument(h_documents element, bool withSave = true)
        {
            if(element.id == 0)
            {
                db.h_documents.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteDocument(int id)
        {
            bool res = false;
            var item = db.h_documents.SingleOrDefault(x => x.id == id);
            if (item != null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region logMails
        public List<h_logMails> GetMailLogs()
        {
            //var res = db.h_logMails.Include(s => s.h_mails);
            //h_logMails res2 = GetSQLData<h_logMails>("h_logMails",CommandType.TableDirect) as List<h_logMails> ?? new List<h_logMails>();
            //SELECT * FROM h_logMails
            //dynamic res = GetSQLData<dynamic>("h_logMails", CommandType.TableDirect) as List<dynamic> ?? new List<dynamic>();
            var res = GetListSQLData<h_logMails>("SELECT * FROM h_logMails",null,CommandType.Text);
            return res;
        }
        public dynamic GetMailLogs(int mailID)
        {
            dynamic res = GetSQLData<dynamic>("SELECT * FROM h_logMails WHERE mailID="+mailID, null, CommandType.Text) as List<dynamic> ?? new List<dynamic>();
            return res;
        }
        public h_logMails GetLogMail(int id)
        {
            var res = db.h_logMails.FirstOrDefault(x => x.id == id);
            return res;
        }
        public int SaveMailLog(h_logMails element, bool withSave = true)
        {
            if(element.id == 0)
            {
                db.h_logMails.Add(element);
                if (withSave) Save();
            }
            else
            {
                db.Entry(element).State = EntityState.Modified;
                if (withSave) Save();
            }
            return element.id;
        }
        public bool DeleteMailLog(int id)
        {
            bool res = false;
            var item = db.h_logMails.SingleOrDefault(x => x.id == id);
            if(item !=null)
            {
                db.Entry(item).State = EntityState.Deleted;
                Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region Users
        public IQueryable<aspnet_Users> GetUsers()
        {
            var res = db.aspnet_Users;
            return res;
        }
        #endregion
    }
}