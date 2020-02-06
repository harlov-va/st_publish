using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL_Mikhailova
{
    public class ContractsManager
    {
        #region System

        public ContractsRepository db;
        private bool _disposed;

        public ContractsManager()
        {
            db = new ContractsRepository();
            _disposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
            }
            db = null;
            _disposed = true;
        }
        #endregion

        #region Contracts

        public Mikhailova_contracts GetContract(int id)
        {
            var res = new Mikhailova_contracts();
            res = db.GetContract(id);
            return res;
        }

        public List<Mikhailova_contracts> GetContracts()
        {
            var res = new List<Mikhailova_contracts>();
            res = db.GetContracts();
            return res;
        }

        public void SaveContract(Mikhailova_contracts item)
        {
            try
            {
                db.SaveContract(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteContract(int id)
        {
            try
            {
                db.DeleteContract(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditDocField(int pk, string name, string value)
        {
            var item = GetContract(pk);
            switch (name)
            {
                case "desc": item.desc = value; break;
                case "number": item.number = value; break;
                case "statusName": if (value != "") item.statusID = RDL.Convert.StrToInt(value, 0); break;
                case "contagentName": if (value != "") item.contagentID = RDL.Convert.StrToInt(value, 0); break;
            }
            SaveContract(item);
        }

        #endregion

        #region Invoices

        public Mikhailova_invoices GetInvoice(int id)
        {
            var res = new Mikhailova_invoices();
            res = db.GetInvoice(id);
            return res;
        }

        public List<Mikhailova_invoices> GetInvoices()
        {
            var res = new List<Mikhailova_invoices>();
            res = db.GetInvoices();
            return res;
        }

        public void SaveInvoice(Mikhailova_invoices item)
        {
            try
            {
                db.SaveInvoice(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteInvoice(int id)
        {
            try
            {
                db.DeleteInvoice(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditInvoiceField(int pk, string name, string value)
        {
            var item = GetInvoice(pk);
            switch (name)
            {
                case "desc": item.desc = value; break;
                case "statusName": if (value != "") item.statusID = RDL.Convert.StrToInt(value, 0); break;
                case "contagentName": if (value != "") item.contagentID = RDL.Convert.StrToInt(value, 0); break;
            }
            SaveInvoice(item);
        }

        #endregion

        #region Mail

        public Mikhailova_mails GetMail(int id)
        {
            var res = new Mikhailova_mails();
            res = db.GetMail(id);
            return res;
        }

        public List<Mikhailova_mails> GetMails()
        {
            var res = new List<Mikhailova_mails>();
            res = db.GetMails();
            return res;
        }

        public void SaveMail(Mikhailova_mails item)
        {
            try
            {
                db.SaveMail(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteMail(int id)
        {
            try
            {
                db.DeleteContract(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditMailField(int pk, string name, string value)
        {
            var item = GetMail(pk);
            switch (name)
            {
                case "desc": item.desc = value; break;
                case "systemMail": item.systemMail = value; break;
                case "treckNumber": item.treckNumber = value; break;
                case "treckNumberReplay": item.treckNumberReplay = value; break;
                case "statusName": if (value != "") item.statusID = RDL.Convert.StrToInt(value, 0); break;
            }
            SaveMail(item);
        }

        #endregion

        #region StatusesContract

        public Mikhailova_statuses_contracts GetStatusContract(int id)
        {
            var res = new Mikhailova_statuses_contracts();
            res = db.GetStatusContract(id);
            return res;
        }

        public List<Mikhailova_statuses_contracts> GetStatusesContract()
        {
            var res = new List<Mikhailova_statuses_contracts>();
            res = db.GetStatusesContract();
            return res;
        }

        public void SaveStatusContract(Mikhailova_statuses_contracts item)
        {
            try
            {
                db.SaveStatusContract(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteStatusContract(int id)
        {
            try
            {
                db.DeleteStatusContract(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region StatusInvoice

        public Mikhailova_statuses_invoces GetStatusInvoice(int id)
        {
            var res = new Mikhailova_statuses_invoces();
            res = db.GetStatusInvoice(id);
            return res;
        }

        public List<Mikhailova_statuses_invoces> GetStatusesInvoice()
        {
            var res = new List<Mikhailova_statuses_invoces>();
            res = db.GetStatusesInvoice();
            return res;
        }

        public void SaveStatusInvoice(Mikhailova_statuses_invoces item)
        {
            try
            {
                db.SaveStatusInvoice(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteStatusInvoice(int id)
        {
            try
            {
                db.DeleteStatusInvoice(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        
        #endregion

        #region StatusesMail

        public Mikhailova_statuses_mails GetStatusMail(int id)
        {
            var res = new Mikhailova_statuses_mails();
            res = db.GetStatusMail(id);
            return res;
        }

        public List<Mikhailova_statuses_mails> GetStatusesMail()
        {
            var res = new List<Mikhailova_statuses_mails>();
            res = db.GetStatusesMail();
            return res;
        }

        public void SaveStatusMail(Mikhailova_statuses_mails item)
        {
            try
            {
                db.SaveStatusMail(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteStatusMail(int id)
        {
            try
            {
                db.DeleteStatusContract(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        
        #endregion

        #region Contagents

        public Mikhailova_contagents GetContagent(int id)
        {
            var res = new Mikhailova_contagents();
            res = db.GetContagent(id);
            return res;
        }

        public List<Mikhailova_contagents> GetContagents()
        {
            var res = new List<Mikhailova_contagents>();
            res = db.GetContagents();
            return res;
        }

        public void SaveContagent(Mikhailova_contagents item)
        {
            try
            {
                db.SaveContagent(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteContagent(int id)
        {
            try
            {
                db.DeleteContagent(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region DocTypes

        public Mikhailova_docTypes GetDocType(int id)
        {
            var res = new Mikhailova_docTypes();
            res = db.GetDocType(id);
            return res;
        }

        public List<Mikhailova_docTypes> GetDocTypes()
        {
            var res = new List<Mikhailova_docTypes>();
            res = db.GetDocTypes();
            return res;
        }

        public void SaveDocType(Mikhailova_docTypes item)
        {
            try
            {
                db.SaveDocType(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocType(int id)
        {
            try
            {
                db.DeleteDocType(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region DocTypeTemplates

        public Mikhailova_docTypeTemplates GetDocTypeTemplate(int id)
        {
            var res = new Mikhailova_docTypeTemplates();
            res = db.GetDocTypeTemplate(id);
            return res;
        }

        public List<Mikhailova_docTypeTemplates> GetDocTypeTemplates()
        {
            var res = new List<Mikhailova_docTypeTemplates>();
            res = db.GetDocTypeTemplates();
            return res;
        }

        public List<Mikhailova_docTypeTemplates> GetListTemplatesByType(int typeId)
        {
            var res = new List<Mikhailova_docTypeTemplates>();
            res = db.GetListTemplatesByType(typeId);
            return res;
        }

        public void SaveDocTypeTemplate(Mikhailova_docTypeTemplates item)
        {
            try
            {
                db.SaveDocTypeTemplate(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocTypeTemplate(int id)
        {
            try
            {
                db.DeleteDocTypeTemplate(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditTemplateField(int pk, string name, string value)
        {
            var item = GetDocTypeTemplate(pk);
            switch (name)
            {
                case "name": item.name= value; break;
            }
            SaveDocTypeTemplate(item);
        }

        #endregion
    }
}