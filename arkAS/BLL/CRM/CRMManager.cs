using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Dapper;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.IO;

namespace arkAS.BLL.CRM
{
    public class CRMManager
    {
        #region System
        public CRMRepository db;
        private bool _disposed;

        public CRMManager()
        {
            db = new CRMRepository();
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

        #region clients

        public crm_clients GetClient(int id)
        {
            var res = new crm_clients();
            res = db.GetClient(id);
            return res;
        }

        public List<crm_clients> GetClients()
        {
            var res = new List<crm_clients>();
            res = db.GetClients();
            return res;
        }

        public void SaveClient(crm_clients item)
        {
            try
            {
                db.SaveClient(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteClient(int id)
        {
            try
            {
                db.DeleteClient(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditClientField(int pk, string name, string value)
        {
            var item = GetClient(pk);
            switch (name)
            {
                case "fio": item.fio = value; break;
                case "city": item.city = value; break;
                case "note": item.note = value; break;
                case "sourceName":
                    if (value != "")
                        item.sourceID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.sourceID = null;
                    break;
                case "statusName":
                    if (value != "")
                        item.statusID = RDL.Convert.StrToInt(value, 0);
                    else
                        item.statusID = null;
                    break;
                case "addedBy": item.addedBy = value; break;
                case "subchannel": item.subchannel = value; break;
                case "username": item.username = value; break;
                case "needActive": item.needActive = (value == "Да"); break;
                case "nextContact":
                    if (value != "")
                        item.nextContact = RDL.Convert.StrToDateTime(value, System.DateTime.Now);
                    else
                        item.nextContact = null;
                    break;
            }
            SaveClient(item);
        }

        /*public string GetClientStatusColor(int clientID)
        {
            string res = "";
            try
            {
                if (GetClient(clientID).statusID != null)
                {
                    var color = GetClient(clientID).crm_clientStatuses.color;
                    if (color != null)
                        res = color;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }*/

        #endregion

        #region clientStatus
        public crm_clientStatuses GetClientStatus(int id)
        {
            var res = new crm_clientStatuses();
            res = db.GetClientStatus(id);
            return res;
        }

        public List<crm_clientStatuses> GetClientStatuses()
        {
            var res = new List<crm_clientStatuses>();
            res = db.GetClientStatuses();
            return res;
        }

        public void SaveClientStatus(crm_clientStatuses item)
        {
            try
            {
                db.SaveClientStatus(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteClientStatus(int id)
        {
            try
            {
                db.DeleteClientStatus(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region clientSource

        public crm_sources GetClientSource(int id)
        {
            var res = new crm_sources();
            res = db.GetClientSource(id);
            return res;
        }

        public List<crm_sources> GetClientSources()
        {
            var res = new List<crm_sources>();
            res = db.GetClientSources();
            return res;
        }

        public void SaveClientSources(crm_sources item)
        {
            try
            {
                db.SaveClientSources(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteClientSources(int id)
        {
            try
            {
                db.DeleteClientSources(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region Orders

        public List<crm_orders> GetOrders(int clientID=0) {
            var res = new List<crm_orders>();
            res = db.GetOrders(clientID);
            return res;        
        }

        internal crm_orders GetOrder(int id)
        {
            var res = db.GetOrder(id);
            return res;
        }
        internal crm_orders GetOrder(string orderNum)
        {
            var res = db.GetOrder(orderNum);
            return res;
        }

        internal int SaveOrder(crm_orders item)
        {
            var res = db.SaveOrder(item);
            return res;
        }

        public List<crm_orderStatuses> GetOrderStatuses()
        {
            var res = new List<crm_orderStatuses>();
            res = db.GetOrderStatuses();
            return res;
        }

        internal void EditOrderField(int pk, string name, string value)
        {
            var order = db.GetOrder(pk);
            if (order != null) { 
                switch(name){
                    case "orderNum": order.orderNum = value; break;
                    case "statusName":
                        int? stID = RDL.Convert.StrToInt(value, 0);
                        if (stID == 0) stID = null;
                        order.statusID = stID; break; 
                } 
            }
            db.SaveOrder(order);           
        }

        internal int ReplaceOrderField(string code, string from, string to)
        {
            var res = 0;
            try
            {
                var items = db.GetOrders();
                var statusTo = db.GetOrderStatuses().FirstOrDefault(x => String.Compare( x.name, to, true)==0);
                var clientTo = db.GetClients().FirstOrDefault(x => String.Compare(x.fio, to, true) == 0);
                switch (code)
                {
                    case "statusName":
                        foreach (var item in items)
                        {
                            if (item.crm_orderStatuses != null && String.Compare(item.crm_orderStatuses.name, from, true) == 0)
                            {
                                item.statusID = statusTo != null ? (int?)statusTo.id : null;
                                res++;
                            }
                        }
                        db.db.SaveChanges();
                        break;
                    case "clientName":
                        foreach (var item in items)
                        {
                            if (item.crm_clients != null && String.Compare(item.crm_clients.fio, from, true) == 0)
                            {
                                item.clientID = clientTo != null ? (int?)clientTo.id : null;
                                res++;
                            }
                        }
                        db.db.SaveChanges();
                        break;
                }
            }
            catch (Exception ex) { }
            return res;
        }
        #endregion

        #region projects

        public List<tt_projects> GetProjects()
        {
            var res = new List<tt_projects>();
            res = db.GetProjects();
            return res;
        }

        internal tt_projects GetProject(int id)
        {
            var res = db.GetProject(id);
            return res;
        }

        internal int SaveProject(tt_projects item)
        {
            var res = db.SaveProject(item);
            return res;
        }

        #endregion

        #region reclamation

        public List<recl_items> GetReclamations()
        {
            var res = new List<recl_items>();
            res = db.GetReclamations();
            return res;
        }

        internal recl_items GetReclamation(int id)
        {
            var res = db.GetReclamation(id);
            return res;
        }

        internal int SaveReclamation(recl_items item)
        {
            var res = db.SaveReclamation(item);
            return res;
        }

        public void DeleteReclamation(int id)
        {
            try
            {
                db.DeleteReclamation(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditReclamationField(int pk, string name, string value)
        {
            var recl = db.GetReclamation(pk);
            if (recl != null)
            {
                switch (name)
                {
                    case "addedBy": recl.addedBy = value; break;
                    case "name":    recl.name = value; break;
                    case "statusName": 
                        int? stID = RDL.Convert.StrToInt(value, 0);
                        if (stID == 0) stID = null;
                        recl.statusID = stID; break;
                    case "projectName": 
                        int? prID = RDL.Convert.StrToInt(value, 0);
                        if (prID == 0) prID = null;
                        recl.projectID = prID; break;
                    case "haveWOWname":
                        int? haveWOWvalue = RDL.Convert.StrToInt(value, 0);
                        if (haveWOWvalue == 2) { recl.haveWOW = true; }
                        else { recl.haveWOW = false; }
                        break;
                    case "customerText": 
                        recl.customerText = value; 
                        recl.created = DateTime.Now;
                        break;
                    case "whatToDo": recl.whatToDo = value; break;
                    case "report": 
                        recl.report = value;
                        recl.reportDate = DateTime.Now;
                        break;
                    case "reportDate":
                        if (value.Length == 10)
                        {
                            int y = Convert.ToInt32(value.Substring(6));
                            int m = Convert.ToInt32(value.Substring(3, 2));
                            int d = Convert.ToInt32(value.Substring(0, 2));
                            recl.reportDate = new DateTime(y, m, d);
                        }
                        break;
                    case "created":
                        if (value.Length == 10)
                        {
                            int y = Convert.ToInt32(value.Substring(6));
                            int m = Convert.ToInt32(value.Substring(3, 2));
                            int d = Convert.ToInt32(value.Substring(0, 2));
                            recl.created = new DateTime(y, m, d);
                        }
                        break;
                }
            }
            db.SaveReclamation(recl);
        }

        #endregion
        
        #region reclamationStatuses

        public List<recl_itemStatuses> GetReclamationStatuses()
        {
            var res = new List<recl_itemStatuses>();
            res = db.GetReclamationStatuses();
            return res;
        }

        internal recl_itemStatuses GetReclamationStatus(int id)
        {
            var res = db.GetReclamationStatus(id);
            return res;
        }

        internal int SaveReclamationStatus(recl_itemStatuses item)
        {
            var res = db.SaveReclamationStatus(item);
            return res;
        }

        #endregion

        #region Dapper

        #region Client

        /*
         * Возвращает список клиентов
         * В sql указывается хранимая процедура
         * */
        public IEnumerable<crm_clients> GetSQLClients(string sql)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<crm_clients>(sql, new { id = 0}, commandType: CommandType.StoredProcedure);

                    List<crm_clients> list = new List<crm_clients>();
                    foreach (var n in els)
                    {
                        var listSources = conn.Query<crm_sources>(sql, new { sourceID = n.sourceID }, commandType: CommandType.StoredProcedure);
                        n.crm_sources = (listSources.Count<crm_sources>() > 0) ? listSources.First() : null;
                        var listClientStatuses = conn.Query<crm_clientStatuses>(sql, new { statusID = n.statusID }, commandType: CommandType.StoredProcedure);
                        n.crm_clientStatuses = (listClientStatuses.Count<crm_clientStatuses>() > 0) ? listClientStatuses.First() : null;
                        var listOrders = conn.Query<crm_orders>(sql, new { orders = n.id }, commandType: CommandType.StoredProcedure);
                        n.crm_orders = listOrders.ToList();

                        list.Add(n);
                    }

                    return (IEnumerable<crm_clients>)list;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(IEnumerable<crm_clients>);
            }
        }

        #endregion

        /*
         * Функция общего назначения для выборки через Dapper
         * По умолчанию в sql указывается хранимая процедура
         * */
        public IEnumerable<T> GetSQLData<T>(string sql, object parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    conn.Open();
                    var els = conn.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure);
                    return (IEnumerable<T>)els;
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
                return default(IEnumerable<T>);
            }
        }

        #endregion

        #region cl ListItems

        public Dictionary<int, string> GetLists()
        {
            var res = db.GetLists();
            return res;
        }

        public cl_listItems clGetListItem(int id)
        {
            var res = new cl_listItems();
            res = db.clGetListItem(id);
            return res;
        }

        internal int clSaveListItem(cl_listItems item)
        {
            var res = db.clSaveListItem(item);
            return res;
        }

        public void clDeleteListItem(int id)
        {
            try
            {
                db.clDeleteListItem(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void clEditListItemField(int pk, string name, string value)
        {
            var li = db.clGetListItem(pk);
            if (li != null)
            {
                switch (name)
                {
                    case "name": li.name = value; break;
                    case "listName":
                        int lID = RDL.Convert.StrToInt(value, 0);
                        li.listID = lID; break;
                    case "ord":
                        int ord = RDL.Convert.StrToInt(value, 0);
                        li.ord = ord; break;
                }
            }
            db.clSaveListItem(li);
        }

        
        #endregion

        #region cl Lists

        public List<cl_lists> clGetLists()
        {
            var res = new List<cl_lists>();
            res = db.clGetLists();
            return res;
        }

        public cl_lists clGetList(int id)
        {
            var res = new cl_lists();
            res = db.clGetList(id);
            return res;
        }

        public List<string> GetUserNames()
        {
            var res = db.GetUserNames();
            return res;
        }

        internal int clSaveList(cl_lists item)
        {
            var res = db.clSaveList(item);
            return res;
        }

        internal void clEditListField(int pk, string name, string value)
        {
            var list = db.clGetList(pk);
            if (list != null)
            {
                switch (name)
                {
                    case "name": list.name = value; break;
                    case "code": list.code = value; break;
                    case "users": list.users = value; break;
                    case "roles": list.roles = value; break;
                }
            }
            db.clSaveList(list);
        }

        internal void ChangeRoleForList(int pk, string roles)
        {
            var list = db.clGetList(pk);
            if (list != null)
            {
                list.roles = roles;
            }
            db.clSaveList(list);
        }

        public void clDeleteList(int id)
        {
            try
            {
                db.clDeleteList(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion
    }
}