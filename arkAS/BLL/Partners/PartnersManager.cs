using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arkAS.BLL;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace arkAS.BLL.Partners
{
    public class PartnersManager
    {
        #region System
        public PartnersRepository db;
        private bool _disposed;

        public PartnersManager()
        {
            db = new PartnersRepository();
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
        public ps_partners GetPartner(int id)
        {
            var res = new ps_partners();
            res = db.GetPartner(id);
            return res;
        }

        public List<ps_partners> GetPartners()
        {
            var res = new List<ps_partners>();
            res = db.GetPartners();
            return res;
        }

        public void SavePartner(ps_partners item)
        {
            try
            {
                db.SavePartner(item);
            }
            catch(Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeletePartner(int id)
        {
            try
            {
                db.DeletePartner(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
             
        }

        internal void EditPartnerField(int pk, string name, string value)
        {
            var item = GetPartner(pk);
            switch(name)
            {
                case "fio": item.fio = value; break;
                case "url": item.url = value; break;
                case "desc": item.desc = value; break;
                case "experience": item.experience = value; break;
                case "technologies": item.technologies = value; break;
                case "conditions": item.conditions = value; break;
                case "specID":
                    if (value != "")
                        item.specID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.specID = null;
                    break;

                case "statusName":
                case "statusID":
                    if (value != "")
                        item.statusID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.statusID = null;
                    break;
            }
            SavePartner(item);
        }

        #endregion

        #region PartnerStatus
        public ps_statuses GetPartnersStatus(int id)
        {
            var res = new ps_statuses();
            res = db.GetPartnersStatus(id);
            return res;
        }

        public List<ps_statuses> GetPartnerStatuses()
        {
            var res = new List<ps_statuses>();
            res = db.GetPartnersStatuses();
            return res;

        }

        public void SavePartnerStatus(ps_statuses item)
        {
            try
            {
                db.SavePartnerStatus(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
           
        }

        public void DeletePartnerStatus(int id)
        {
            try
            {
                db.DeletePartnerStatus(id);
            }
            catch(Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
    
        #endregion 
     
        #region PartnersSpec
        public ps_specs GetPartnersSpec(int id)
        {
            var res = new ps_specs();
            res = db.GetPartnersSpec(id);
            return res;
        }

        public List<ps_specs> GetPartnersSpec()
        {
            var res = new List<ps_specs>();
            res = db.GetPartnersSpecs();
            return res;

        }

        public void SavePartnersSpec(ps_specs item)
        {
            try
            {
                db.SavePartnersSpec(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
           
        }

        public void DeletePartnersSpec(int id)
        {
            try
            {
                db.DeletePartnerSpec(id);
            }
            catch(Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
    
        #endregion

        #region PartnersSpecList
        public ps_specsPartners GetPartnersSpecListItem(int id)
        {
            var res = new ps_specsPartners();
            res = db.GetPartnersSpecListItem(id);
            return res;
        }

        public List<ps_specsPartners> GetPartnersSpecList()
        {
            var res = new List<ps_specsPartners>();
            res = db.GetPartnersSpecList();
            return res;

        }

        public void SavePartnersSpecList(ps_specsPartners item)
        {
            try
            {
                db.SavePartnersSpecList(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }

        }

        public void DeletePartnersSpecListItem(int id)
        {
            try
            {
                db.DeletePartnerSpec(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion


        public List<ps_specs> GetSpecs(int partnerID)
        {
            return db.GetPartnersSpecList().Where(x => x.partnerID == partnerID).Select(x=> x.ps_specs).ToList();
        }

        //
        public void addPartnersSpecs(int specID, int partnerID)
        {
            var item = new ps_specsPartners
            {
                id = 0,
                specID = specID,
                partnerID = partnerID
            };

                db.SavePartnersSpecList(item);
        }

        public void removePartnersSpecs(int specID, int partnerID)
        {
            var item = db.GetPartnersSpecListItemCh(specID, partnerID);
            if (item != null)
            {
                db.DeletePartnerSpecListItem(item.id);
            }
 
        }

   }
}
        