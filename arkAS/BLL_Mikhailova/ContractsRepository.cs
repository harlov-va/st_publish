using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL_Mikhailova
{
    public class ContractsRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public ContractsRepository()
        {
            db = new LocalSqlServer();
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
            var res = db.Mikhailova_contracts.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_contracts> GetContracts()
        {
            var res = db.Mikhailova_contracts.ToList();
            return res;
        }

        public int SaveContract(Mikhailova_contracts item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_contracts.Add(item);
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

        public bool DeleteContract(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_contracts.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region Invoices

        public Mikhailova_invoices GetInvoice(int id)
        {
            var res = db.Mikhailova_invoices.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_invoices> GetInvoices()
        {
            var res = db.Mikhailova_invoices.ToList();
            return res;
        }

        public int SaveInvoice(Mikhailova_invoices item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_invoices.Add(item);
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

        public bool DeleteInvoice(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_invoices.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region Mails

        public Mikhailova_mails GetMail(int id)
        {
            var res = db.Mikhailova_mails.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_mails> GetMails()
        {
            var res = db.Mikhailova_mails.ToList();
            return res;
        }

        public int SaveMail(Mikhailova_mails item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_mails.Add(item);
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

        public bool DeleteMail(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_mails.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region StatusesContract

        public Mikhailova_statuses_contracts GetStatusContract(int id)
        {
            var res = db.Mikhailova_statuses_contracts.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_statuses_contracts> GetStatusesContract()
        {
            var res = db.Mikhailova_statuses_contracts.ToList();
            return res;
        }

        public int SaveStatusContract(Mikhailova_statuses_contracts item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_statuses_contracts.Add(item);
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

        public bool DeleteStatusContract(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_statuses_contracts.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region StatusesInvoice

        public Mikhailova_statuses_invoces GetStatusInvoice(int id)
        {
            var res = db.Mikhailova_statuses_invoces.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_statuses_invoces> GetStatusesInvoice()
        {
            var res = db.Mikhailova_statuses_invoces.ToList();
            return res;
        }

        public int SaveStatusInvoice(Mikhailova_statuses_invoces item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_statuses_invoces.Add(item);
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

        public bool DeleteStatusInvoice(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_statuses_invoces.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region StatusesMail

        public Mikhailova_statuses_mails GetStatusMail(int id)
        {
            var res = db.Mikhailova_statuses_mails.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_statuses_mails> GetStatusesMail()
        {
            var res = db.Mikhailova_statuses_mails.ToList();
            return res;
        }

        public int SaveStatusMail(Mikhailova_statuses_mails item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_statuses_mails.Add(item);
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

        public bool DeleteStatusMail(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_statuses_mails.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region Contagents

        public Mikhailova_contagents GetContagent(int id)
        {
            var res = db.Mikhailova_contagents.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_contagents> GetContagents()
        {
            var res = db.Mikhailova_contagents.ToList();
            return res;
        }

        public int SaveContagent(Mikhailova_contagents item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_contagents.Add(item);
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

        public bool DeleteContagent(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_contagents.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region DocTypes

        public Mikhailova_docTypes GetDocType(int id)
        {
            var res = db.Mikhailova_docTypes.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_docTypes> GetDocTypes()
        {
            var res = db.Mikhailova_docTypes.ToList();
            return res;
        }

        public int SaveDocType(Mikhailova_docTypes item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_docTypes.Add(item);
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

        public bool DeleteDocType(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_docTypes.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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

        #region DocTypeTemplates

        public Mikhailova_docTypeTemplates GetDocTypeTemplate(int id)
        {
            var res = db.Mikhailova_docTypeTemplates.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<Mikhailova_docTypeTemplates> GetDocTypeTemplates()
        {
            var res = db.Mikhailova_docTypeTemplates.ToList();
            return res;
        }

        public List<Mikhailova_docTypeTemplates> GetListTemplatesByType(int typeId)
        {
            var res = db.Mikhailova_docTypeTemplates.Where(x => x.typeID == typeId).ToList();
            return res;
        }

        public int SaveDocTypeTemplate(Mikhailova_docTypeTemplates item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.Mikhailova_docTypeTemplates.Add(item);
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

        public bool DeleteDocTypeTemplate(int id)
        {
            var res = false;
            try
            {
                var item = db.Mikhailova_docTypeTemplates.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = EntityState.Deleted;
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