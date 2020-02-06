using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace arkAS.BLL.CRM
{
    public class CRMRepository
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public CRMRepository()
        {
            db = new LocalSqlServer();
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
      
        #region Orders

        public List<crm_orders> GetOrders(int clientID=0)
        {
            var res = new List<crm_orders>();
            res = db.crm_orders
                .Include(x => x.crm_clients)    
                .Include(x => x.crm_orderStatuses)
                .Where(x => clientID==0 || x.clientID==clientID).ToList();
            return res;
        }

        public crm_orders GetOrder(int id)
        {
            var res = new crm_orders();
            res = db.crm_orders.FirstOrDefault(x => x.id == id);
            return res;
        }
        public crm_orders GetOrder(string orderNum)
        {
            var res = new crm_orders();
            res = db.crm_orders.FirstOrDefault(x => x.orderNum == orderNum);
            return res;
        }
       
        public int SaveOrder(crm_orders element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.crm_orders.Add(element);
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
        public bool DeleteOrder(int id)
        {
            bool res = false;
            try
            {
                var item = db.crm_orders.SingleOrDefault(x => x.id == id);
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
       
        #region OrderStatuses

        public List<crm_orderStatuses> GetOrderStatuses()
        {
            var res = new List<crm_orderStatuses>();
            res = db.crm_orderStatuses.ToList();
            return res;
        }

        public crm_orderStatuses GetOrderStatus(string code)
        {
            var res = new crm_orderStatuses();
            res = db.crm_orderStatuses.FirstOrDefault(x => x.code == code);
            return res;
        }

        public int SaveOrderStatus(crm_orderStatuses element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.crm_orderStatuses.Add(element);
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
        public bool DeleteOrderStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.crm_orderStatuses.SingleOrDefault(x => x.id == id);
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

        #region Clients

        public List<crm_clients> GetClients()
        {
            var res = new List<crm_clients>();
            res = db.crm_clients.ToList();
            return res;
        }

        public crm_clients GetClient(int id)
        {
            var res = new crm_clients();
            res = db.crm_clients.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveClient(crm_clients element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.crm_clients.Add(element);
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
        public bool DeleteClient(int id)
        {
            bool res = false;
            try
            {
                var item = db.crm_clients.SingleOrDefault(x => x.id == id);
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

        #region clientStatuses

        public List<crm_clientStatuses> GetClientStatuses()
        {
            var res = new List<crm_clientStatuses>();
            res = db.crm_clientStatuses.ToList();
            return res;
        }

        public crm_clientStatuses GetClientStatus(int id)
        {
            var res = new crm_clientStatuses();
            res = db.crm_clientStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveClientStatus(crm_clientStatuses element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.crm_clientStatuses.Add(element);
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
        public bool DeleteClientStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.crm_clientStatuses.SingleOrDefault(x => x.id == id);
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

        #region clientSources

        public List<crm_sources> GetClientSources()
        {
            var res = new List<crm_sources>();
            res = db.crm_sources.ToList();
            return res;
        }

        public crm_sources GetClientSource(int id)
        {
            var res = new crm_sources();
            res = db.crm_sources.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveClientSources(crm_sources element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.crm_sources.Add(element);
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
        public bool DeleteClientSources(int id)
        {
            bool res = false;
            try
            {
                var item = db.crm_sources.SingleOrDefault(x => x.id == id);
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

        #region projects

        public List<tt_projects> GetProjects()
        {
            var res = new List<tt_projects>();
            res = db.tt_projects.ToList();
            return res;
        }

        public tt_projects GetProject(int id)
        {
            var res = new tt_projects();
            res = db.tt_projects.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveProject(tt_projects element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.tt_projects.Add(element);
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
        
        public bool DeleteProject(int id)
        {
            bool res = false;
            try
            {
                var item = db.tt_projects.SingleOrDefault(x => x.id == id);
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

        #region reclamation

        public List<recl_items> GetReclamations()
        {
            var res = new List<recl_items>();
            res = db.recl_items.ToList();
            return res;
        }

        public recl_items GetReclamation(int id)
        {
            var res = new recl_items();
            res = db.recl_items.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveReclamation(recl_items element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.recl_items.Add(element);
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

        public bool DeleteReclamation(int id)
        {
            bool res = false;
            try
            {
                var item = db.recl_items.SingleOrDefault(x => x.id == id);
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
        
        #region reclamationStatuses

        public List<recl_itemStatuses> GetReclamationStatuses()
        {
            var res = new List<recl_itemStatuses>();
            res = db.recl_itemStatuses.ToList();
            return res;
        }

        public recl_itemStatuses GetReclamationStatus(int id)
        {
            var res = new recl_itemStatuses();
            res = db.recl_itemStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int SaveReclamationStatus(recl_itemStatuses element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.recl_itemStatuses.Add(element);
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

        public bool DeleteReclamationStatus(int id)
        {
            bool res = false;
            try
            {
                var item = db.recl_itemStatuses.SingleOrDefault(x => x.id == id);
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

        #region cl ListItems

        public Dictionary<int, string> GetLists()
        {        
            Dictionary<int, string> res = db.cl_lists.Select(x => new { id = x.id, name = x.name }).Distinct().ToDictionary(a => a.id, a => a.name);
            return res;
        }

        public cl_listItems clGetListItem(int id)
        {
            var res = new cl_listItems();
            res = db.cl_listItems.FirstOrDefault(x => x.id == id);
            return res;
        }

        public int clSaveListItem(cl_listItems element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.cl_listItems.Add(element);
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

        public bool clDeleteListItem(int id)
        {
            bool res = false;
            try
            {
                var item = db.cl_listItems.SingleOrDefault(x => x.id == id);
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

        #region cl Lists

        public cl_lists clGetList(int id)
        {
            var res = new cl_lists();
            res = db.cl_lists.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<cl_lists> clGetLists()
        {
            var res = new List<cl_lists>();
            res = db.cl_lists.ToList();
            return res;
        }

        public List<string> GetUserNames()
        {
            List<string> res = db.aspnet_Users.Select(x => x.UserName).ToList();
            return res;
        }

        public int clSaveList(cl_lists element)
        {
            try
            {
                if (element.id == 0)
                {
                    db.cl_lists.Add(element);
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

        public bool clDeleteList(int id)
        {
            bool res = false;
            try
            {
                var item = db.cl_lists.SingleOrDefault(x => x.id == id);
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