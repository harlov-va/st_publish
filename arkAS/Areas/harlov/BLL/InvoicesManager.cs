using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.BLL
{
    public class InvoicesManager:IInvoicesManager
    {
        #region System
        private IRepository _db;
        private bool _disposed;
        public InvoicesManager (IRepository db)
        {
            _db = db;
            _disposed = false;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            { 
                if(disposing)
                {
                    if (_db != null)
                        _db.Dispose();
                }
                _db = null;
                _disposed = true;
            }
        }
        private void _debug(Exception ex, Object parameters = null, string additions = "")
        {
            RDL.Debug.LogError(ex, additions, parameters);
        }
        private bool _canAccesseToItem(aspnet_Users user)
        {
            var res = false;
            var isAdmin = user.aspnet_Roles.Any(x => x.RoleName == "admin");
            var isManager = user.aspnet_Roles.Any(x => x.RoleName == "guest");
            if(user !=null && isAdmin || isManager)
            {
                res = true;
            }
            return res;
        }
        private bool _canManageItem(aspnet_Users user)
        {
            var res = false;
            var isAdmin = user.aspnet_Roles.Any(x => x.RoleName == "admin");
            if(user!=null && isAdmin)
            {
                res = true;
            }
            return res;
        }
        #endregion
        #region Inovices
        public List<h_invoices> GetInvoices(aspnet_Users user, out string msg)
        {
            msg = "";
            List<h_invoices> res;
            try
            {
                if (!_canAccesseToItem(user))
                {
                    msg = "Нет прав на получение списка элемента!";
                    res = null;
                }
                else
                {
                    res = _db.GetInvoices().Where(x => x.isDeleted !=true).ToList();
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении списка элемента");
                res = null;
            }
            return res;
        }
        public h_invoices GetInvoice(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_invoices res;
            try
            {
                if(!_canAccesseToItem(user))
                {
                    msg = "Нет прав на получение элемента по id";
                    res = null;
                }
                else
                {
                    res = _db.GetInvoice(id);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении элемента по id");
                res = null;
            }
            return res;
        }
        public h_invoices CreateInvoice(Dictionary <string , string> parameters,aspnet_Users user, out string msg)
        {
            msg = "";
            h_invoices res;
            try
            {
                if(!_canAccesseToItem(user))
                {
                    msg = "Нет прав создавать элемент";
                    res = null;
                }
                else
                {
                    res = new h_invoices();
                    res.uniqueCode = Guid.NewGuid();
                    res.isDeleted = false;
                    foreach(var key in parameters.Keys)
                        switch(key)
                        {
                            case "number":res.number = parameters[key];
                                break;
                            case "date": res.date = RDL.Convert.StrToDateTime(parameters[key],DateTime.Now);
                                break;
                            case "description": res.description = parameters[key];
                                break;
                            case "invoiceStatus": res.invStatusID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "contragentName": res.contragentID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                        }
                    _db.SaveInvoice(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при создании элемента");
                res = null;
            }
            return res;
        }
        public h_invoices EditInvoice(Dictionary <string,string> parameters, int id,aspnet_Users user, out string msg)
        {
            msg = "";
            h_invoices res;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав редактировать элемент";
                    res = null;
                }
                else
                {
                    res = _db.GetInvoice(id);
                    foreach(var key in parameters.Keys)
                    switch(key)
                        {
                            case "number":
                                res.number = parameters[key];
                                break;
                            case "date":
                                res.date = RDL.Convert.StrToDateTime(parameters[key], DateTime.Now);
                                break;
                            case "description":
                                res.description = parameters[key];
                                break;
                            case "invoiceStatus":
                                res.invStatusID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "contragentName":
                                res.contragentID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                        }
                    _db.SaveInvoice(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = null;
            }
            return res;
        }
        public bool ChangeInvoicesStatus(int id, string name, string value, out string msg, aspnet_Users user)
        {
            bool res;
            msg = "";
            try
            {
                if (!_canManageItem(user))
                {
                    msg = "Нет прав редактировать элемента";
                    res = false;
                }
                else
                {
                    var item = _db.GetInvoice(id);
                    switch (name)
                    {
                        case "invoiceStatus":
                            int stID = RDL.Convert.StrToInt(value, 0);
                            item.invStatusID = stID;
                            _db.SaveInvoice(item);
                            //_db.SaveInvoiceLog(_logInvoicesChanges(user, item, "Документ  изменен"));
                            res = true;
                            break;
                        default:
                            res = false;
                            break;

                    }
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = false;
            }
            return res;
        }
        public bool RemoveInvoice(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            bool res = false;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав на удаление элемента";
                    res = false;
                }
                else
                {
                    var item = _db.GetInvoice(id);
                    item.isDeleted = true;
                    _db.SaveInvoice(item);
                    res = true;
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при удалении элемента");
                res = false;
            }
            return res;
        }
        #endregion
        #region InvoiceStatuses
        public List<h_invoiceStatuses> GetInvoiceStatuses(aspnet_Users user, out string msg)
        {
            msg = "";
            List<h_invoiceStatuses> res;
            try
            {
                if (!_canAccesseToItem(user))
                {
                    msg = "Нет прав на получение списка элемента!";
                    res = null;
                }
                else
                {
                    res = _db.GetInvoiceStatuses().ToList();
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении списка элемента");
                res = null;
            }
            return res;
        }
        public h_invoiceStatuses GetInvoiceStatus(int id, aspnet_Users user , out string msg)
        {
            msg = "";
            h_invoiceStatuses res;
            try
            {
                if (!_canAccesseToItem(user))
                {
                    msg = "Нет прав на получение элемента по id";
                    res = null;
                }
                else
                {
                    res = _db.GetInvoiceStatus(id);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении списка элемента");
                res = null;
            }
            return res;
            
        }
        public h_invoiceStatuses CreateInvoiceStatus(Dictionary <string , string> parameters, aspnet_Users user, out string msg)
        {
            msg = "";
            h_invoiceStatuses res;
            try
            {
                if(!_canAccesseToItem(user))
                {
                    msg = "Нет прав создавать элемент";
                    res = null;
                }
                else
                {
                    res = new h_invoiceStatuses();
                    foreach (var key in parameters.Keys)
                        switch(key)
                        {
                            case "name":res.name = parameters[key];
                                break;
                            case "code": res.code = parameters[key];
                                break;
                        }
                    _db.SaveInvoiceStatus(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при создании элемента");
                res = null;
            }
            return res;
        }
        public h_invoiceStatuses EditInvoiceStatus(Dictionary <string, string> parameters, int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_invoiceStatuses res;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав редактировать элемент";
                    res = null;
                }
                else
                {
                    res = _db.GetInvoiceStatus(id);
                    foreach(var key in parameters.Keys)
                        switch(key)
                        {
                            case "name":res.name = parameters[key];
                                break;
                            case "code":res.code = parameters[key];
                                break;
                        }
                    _db.SaveInvoiceStatus(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = null;
            }
            return res;
        }
        public bool RemoveInvoiceStatus(int id,aspnet_Users user, out string msg)
        {
            msg = "";
            bool res = false;
            try
            {
                if (!_canManageItem(user))
                {
                    msg = "Нет прав на удаление элемента";
                    res = false;
                }
                else
                {
                    _db.DeleteInvoiceStatus(id);
                    res = true;
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при удалении элемента");
                res = false;
            }
            return res;
        }
        #endregion
        //msg = "Нет прав на получение списка элемента!";
        //"Ошибка возникла при получении списка элемента"
        //msg="Нет прав на получение элемента по id"
        //"Ошибка возникла при получении элемента по id"
        //msg = "Нет прав создавать элемент";
        //"Ошибка возникла при создании элемента"
        //msg = "Нет прав редактировать элемент";
        //"Ошибка возникла при изменении элемента"
        //msg = "Нет прав на удаление элемента";
        //"Ошибка возникла при удалении элемента"
    }
}