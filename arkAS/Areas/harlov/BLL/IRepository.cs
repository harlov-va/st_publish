using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkAS.Areas.harlov.BLL
{
    public interface IRepository:IDisposable
    {
        #region System
        void Save();
        #endregion
        #region Invoices
        IQueryable<h_invoices> GetInvoices();
        h_invoices GetInvoice(int ID);
        int SaveInvoice(h_invoices element, bool withSave = true);
        bool DeleteInvoice(int ID);
        #endregion
        #region InvoiceStatuses
        IQueryable<h_invoiceStatuses> GetInvoiceStatuses();
        h_invoiceStatuses GetInvoiceStatus(int ID);
        int SaveInvoiceStatus(h_invoiceStatuses element, bool withSave = true);
        bool DeleteInvoiceStatus(int ID);
        #endregion
        #region Contragents
        IQueryable<h_contragents> GetContragents();
        h_contragents GetContragent(int ID);
        int SaveContragent(h_contragents element, bool withSave = true);
        bool DeleteContragent(int ID);
        #endregion
        #region DeliverySystems
        IQueryable<h_deliverySystems> GetDeliverySystems();
        h_deliverySystems GetDeliverySystem(int ID);
        int SaveDeliverySystem(h_deliverySystems element, bool withSave = true);
        bool DeleteDeliverySystem(int ID);
        #endregion
        #region mailStatuses
        IQueryable<h_mailStatuses> GetMailStatuses();
        h_mailStatuses GetMailStatus(int ID);
        int SaveMailStatus(h_mailStatuses element, bool withSave = true);
        bool DeleteMailStatus(int ID);
        #endregion
        #region Mails
        IQueryable<h_mails> GetMails();
        h_mails GetMail(int ID);
        int SaveMail(h_mails element, bool withSave = true);
        bool DeleteMail(int ID);
        #endregion
        #region docTypes
        IQueryable<h_docTypes> GetDocTypes();
        h_docTypes GetDocType(int ID);
        int SaveDocType(h_docTypes element, bool withSave = true);
        bool DeleteDocType(int ID);
        #endregion
        #region docStatuses
        IQueryable<h_docStatuses> GetDocStatuses();
        h_docStatuses GetDocStatus(int ID);
        int SaveDocStatus(h_docStatuses element, bool withSave = true);
        bool DeleteDocStatus(int ID);
        #endregion
        #region documents
        IQueryable<h_documents> GetDocuments();
        h_documents GetDocument(int ID);
        int SaveDocument(h_documents element, bool withSave = true);
        bool DeleteDocument(int ID);
        #endregion
        #region logMails
        List<h_logMails> GetMailLogs();
        dynamic GetMailLogs(int mailID);
        h_logMails GetLogMail(int id);
        int SaveMailLog(h_logMails element, bool withSave = true);
        bool DeleteMailLog(int id);
        #endregion
        #region Users
        IQueryable<aspnet_Users> GetUsers();
        #endregion
    }
}
