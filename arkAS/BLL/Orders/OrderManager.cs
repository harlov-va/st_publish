using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Orders
{
    public class OrderManager
    {
        public OrderRepository db;
        private bool _disposed;


        public OrderManager()
        {
            db = new OrderRepository();
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
            var res = new crm_orders();
            res = db.GetOrder(id);
            return res;
        }

        public List<crm_orders> GetOrders()
        {
            var res = new List<crm_orders>();
            res = db.GetOrders();
            return res;
        
        }

        public void SaveOrder(crm_orders item)
        {
            try
            {
                db.SaveOrders(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteOrder(int id)
        {
            try
            {
                db.DeleteOrder(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditOrderField(int pk, string name, string value)
        {
            var item = GetOrder(pk);
            switch (name)
            {
                case "addedBy": item.addedBy = value; break;
                case "orderNum": item.orderNum = value; break;
                case "statusName": if (value != "") item.statusID = RDL.Convert.StrToInt(value, 0); break;
                case "clientName": if (value != "") item.clientID = RDL.Convert.StrToInt(value, 0); break;
               
            }
            SaveOrder(item);
        }
        #endregion

        #region crm_clients
        public List<crm_clients> GetClients()
        {
            var res = new List<crm_clients>();
            res = db.GetClients();
            return res;

        }
        #endregion
        #region crm_orderStatuses
        public List<crm_orderStatuses> GetStatuses()
        {
            var res = new List<crm_orderStatuses>();
            res = db.GetStatuses();
            return res;

        }
        #endregion

    }
}