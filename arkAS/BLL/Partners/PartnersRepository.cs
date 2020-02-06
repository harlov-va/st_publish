using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.SignalR.Messaging;
using System.Data;

namespace arkAS.BLL.Partners
{
    public class PartnersRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public PartnersRepository()
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

        #region Partners
        public List<ps_partners> GetPartners()
        {
            var res = new List<ps_partners>();
            res = db.ps_partners.ToList();
            return res;
        }

        public ps_partners GetPartner(int id)
        {
            var res = new ps_partners();
            res = db.ps_partners.FirstOrDefault(partner => partner.id == id);
            return res;
        }

        public int SavePartner(ps_partners element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.ps_partners.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
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
            return element.id;
        }

        public bool DeletePartner(int id)
        {
            bool res = false;
            try
            {
                var item = db.ps_partners.SingleOrDefault(x => x.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
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

        #region Statuses

        public List<ps_statuses> GetPartnersStatuses()
        {
            var res = new List<ps_statuses>();
            res = db.ps_statuses.ToList();
            return res;
        }

        public ps_statuses GetPartnersStatus(int id)
        {
            var res = new ps_statuses();
            res = db.ps_statuses.FirstOrDefault(partner => partner.id == id);
            return res;
        }

        public int SavePartnerStatus(ps_statuses element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.ps_statuses.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
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
            return element.id;
        }

        public bool DeletePartnerStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.ps_statuses.SingleOrDefault(partner => partner.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
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

        #region Specs

        public List<ps_specs> GetPartnersSpecs()
        {
            var res = new List<ps_specs>();
            res = db.ps_specs.ToList();
            return res;
        }
        //
        public ps_specs GetPartnersSpec(int id, string name)
        {
            ps_specs res = db.ps_specs.FirstOrDefault(x => x.id == id && x.name == name);
            return res;
        }
        //
        public ps_specs GetPartnersSpec(int id)
        {
            var res = new ps_specs();
            res = db.ps_specs.FirstOrDefault(partner => partner.id == id);
            return res;
        }

        public int SavePartnersSpec(ps_specs element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.ps_specs.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
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
            return element.id;
        }

        public bool DeletePartnerSpec(int id)
        {
            bool res = false;
            try
            {
                var item = db.ps_specs.SingleOrDefault(partner => partner.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
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

        #region PartnersList

        public List<ps_specsPartners> GetPartnersSpecList()
        {
            var res = new List<ps_specsPartners>();
            res = db.ps_specsPartners.ToList();
            return res;
        }

        public ps_specsPartners GetPartnersSpecListItem(int id)
        {
            var res = new ps_specsPartners();
            res = db.ps_specsPartners.FirstOrDefault(partner => partner.id == id);
            return res;
        }
        //
        public ps_specsPartners GetPartnersSpecListItemCh(int specID, int partnerID)
        {
            ps_specsPartners res = db.ps_specsPartners.FirstOrDefault(x => x.specID == specID && x.partnerID == partnerID);
            return res;
        }
        //

        public int SavePartnersSpecList(ps_specsPartners element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.ps_specsPartners.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    try
                    {
                        db.Entry(element).State = EntityState.Modified;
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
            return element.id;
        }

        public bool DeletePartnerSpecListItem(int id)
        {
            bool res = false;
            try
            {
                var item = db.ps_specsPartners.SingleOrDefault(partner => partner.id == id);
                if (item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
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