using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Orders
{
    public class OrderRepository
    {
        public LocalSqlServer db;
        private bool _disposed;

        public OrderRepository()
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

        #region crm_orders
        public crm_orders GetOrder(int id)
        {
            var res = db.crm_orders.FirstOrDefault(x=> x.id == id);
            return res;
 
        }
        public List<crm_orders> GetOrders()
        {
            var res = db.crm_orders.ToList();
            return res;
        
        }
        public int SaveOrders(crm_orders item)
        {
             try
            {
                if (item.id == 0)
                {
                    db.crm_orders.Add(item);
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


        public bool DeleteOrder(int id)
        {
            var res = false;

            try
            {
                var item = db.crm_orders.SingleOrDefault(x => x.id == id);
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
        #region crm_clients
       
        public List<crm_clients> GetClients()
        {
            var res = db.crm_clients.ToList();
            return res;

        }
        #endregion

        #region crm_orderStatuses
        public List<crm_orderStatuses> GetStatuses()
        {
            var res = db.crm_orderStatuses.ToList();
            return res;

        }
        #endregion
    }



    }
       