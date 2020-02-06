using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Gurevskiy
{
    public class GurevskiyRepository : IDisposable
    {

        #region System
        public LocalSqlServer db = null;

        public GurevskiyRepository(LocalSqlServer context = null)
        {
            if (context == null)
                this.db = new LocalSqlServer();
            else
                this.db = context;
        }

        public bool Save()
        {
            bool res = false;

            try
            {
                db.SaveChanges();
                res = true;
            }
            catch (Exception e)
            {
                RDL.Debug.LogError(e);
            }

            return res;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion



        #region Contracts
        public gurevskiy_contracts GetContract(int id)
        {
            var res = db.gurevskiy_contracts.SingleOrDefault(x => x.id == id);
            return res;
        }

        public bool SaveContract(gurevskiy_contracts item)
        {
            bool res = false;

            if (item.id == 0)
            {
                db.gurevskiy_contracts.Add(item);
                res = Save();
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;
                res = Save();
            }

            return res;
        }

        public bool DeleteContract(int id)
        {
            bool res = false;

            var item = GetContract(id);
            if (item != null)
            {
                db.gurevskiy_contracts.Remove(item);
                res = Save();
            }

            return res;
        }
        #endregion

        #region ContractTypes
        public gurevskiy_contractTypes GetContractType(int id)
        {
            var res = db.gurevskiy_contractTypes.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<gurevskiy_contractTypes> GetContractTypes()
        {
            var res = db.gurevskiy_contractTypes.ToList();
            return res;
        }
        #endregion

        #region ContractStatus
        public gurevskiy_contractStatuses GetContractStatus(int id)
        {
            var res = db.gurevskiy_contractStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<gurevskiy_contractStatuses> GetContractStatuses()
        {
            var res = db.gurevskiy_contractStatuses.OrderBy(x => x.id).ToList();
            return res;
        }
        #endregion


        
        #region Invoices
        public gurevskiy_invoices GetInvoice(int id)
        {
            var res = db.gurevskiy_invoices.SingleOrDefault(x => x.id == id);
            return res;
        }

        public bool SaveInvoice(gurevskiy_invoices item)
        {
            bool res = false;

            if (item.id == 0)
            {
                db.gurevskiy_invoices.Add(item);
                res = Save();
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;
                res = Save();
            }

            return res;
        }

        public bool DeleteInvoice(int id)
        {
            bool res = false;

            var item = GetInvoice(id);
            if (item != null)
            {
                db.gurevskiy_invoices.Remove(item);
                res = Save();
            }

            return res;
        }
        #endregion

        #region InvoiceStatus
        public gurevskiy_invoiceStatuses GetInvoiceStatus(int id)
        {
            var res = db.gurevskiy_invoiceStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<gurevskiy_invoiceStatuses> GetInvoiceStatuses()
        {
            var res = db.gurevskiy_invoiceStatuses.OrderBy(x => x.id).ToList();
            return res;
        }
        #endregion



        #region Mails
        public gurevskiy_mails GetMail(int id)
        {
            var res = db.gurevskiy_mails.SingleOrDefault(x => x.id == id);
            return res;
        }

        public bool SaveMail(gurevskiy_mails item)
        {
            bool res = false;

            if (item.dateBack == (DateTime?)System.Data.SqlTypes.SqlDateTime.Null)
                item.dateBack = null;

            if (item.id == 0)
            {
                db.gurevskiy_mails.Add(item);
                res = Save();
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;
                res = Save();
            }

            return res;
        }

        public bool DeleteMail(int id)
        {
            bool res = false;

            var item = GetMail(id);
            if (item != null)
            {
                db.gurevskiy_mails.Remove(item);
                res = Save();
            }

            return res;
        }
        #endregion

        #region MailStatus
        public gurevskiy_mailStatuses GetMailStatus(int id)
        {
            var res = db.gurevskiy_mailStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<gurevskiy_mailStatuses> GetMailStatuses()
        {
            var res = db.gurevskiy_mailStatuses.OrderBy(x => x.id).ToList();
            return res;
        }
        #endregion


        
        #region Partners
        public gurevskiy_partner GetPartner(int id)
        {
            var res = db.gurevskiy_partner.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<gurevskiy_partner> GetPartners()
        {
            var res = db.gurevskiy_partner.OrderBy(x => x.name).ToList();
            return res;
        }
        #endregion
    }
}