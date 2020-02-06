using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.BLL
{
    public class ContragentsManager:IContragentsManager
    {
        #region System
        private IRepository _db;
        private bool _disposed;
        public ContragentsManager(IRepository db)
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
                if (disposing)
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
        private bool _canAccessToItem(aspnet_Users user)
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
        #region Contragents
        public List<h_contragents> GetContragents(aspnet_Users user, out string msg)
        {
            msg = "";
            List<h_contragents> res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение списка элемента!";
                    res = null;
                }
                else
                {
                    res = _db.GetContragents().ToList();
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении списка элемента");
                res = null;
            }
            return res;
        }
        public h_contragents GetContragent(int id, aspnet_Users user ,out string msg)
        {
            msg = "";
            h_contragents res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение элемента по id";
                    res = null;
                }
                else
                {
                    res = _db.GetContragent(id);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении элемента по id");
                res = null;
            }
            return res;
        }
        public h_contragents CreateContragent(Dictionary<string , string> parameters, aspnet_Users user, out string msg)
        {
            msg = "";
            h_contragents res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = msg = "Нет прав создавать элемент";
                    res = null;
                }
                else
                {
                    res = new h_contragents();
                    foreach (var key in parameters.Keys)
                        switch (key)
                        {
                            case "name": res.name = parameters[key];
                                break;
                            case "email": res.email = parameters[key];
                                break;
                        }
                    _db.SaveContragent(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при создании элемента");
                res = null; 
            }
            return res;
        }
        public h_contragents EditContragent(Dictionary<string , string > parameters, int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_contragents res;
            try
            {
                if (!_canManageItem(user))
                {
                    msg = "Нет прав редактировать элемент";
                    res = null;
                }
                else
                {
                    res = _db.GetContragent(id);
                    foreach (var key in parameters.Keys)
                        switch (key)
                        {
                            case "name":
                                res.name = parameters[key];
                                break;
                            case "email":
                                res.email = parameters[key];
                                break;
                        }
                    _db.SaveContragent(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = null;
            }
            return res;
        }
        public bool RemoveContragent(int id, aspnet_Users user, out string msg)
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
                    _db.DeleteContragent(id);
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
        //"Нет прав на получение элемента по id"
        //"Ошибка возникла при получении элемента по id"
        //msg = "Нет прав создавать элемент";
        //"Ошибка возникла при создании элемента"
        //msg = "Нет прав редактировать элемент";
        //"Ошибка возникла при изменении элемента"
        //msg = "Нет прав на удаление элемента";
        //"Ошибка возникла при удалении элемента"
    }
}