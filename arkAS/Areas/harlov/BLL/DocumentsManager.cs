using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.BLL
{
    public class DocumentsManager : IDocumentManager
    {
        #region System
        private IRepository _db;
        private bool _disposed;
        public DocumentsManager(IRepository db)
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
            if (!_disposed)
            {
                if (disposing)
                    if (_db != null)
                        _db.Dispose();
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
            if (user != null && isAdmin || isManager)
            {
                res = true;
            }
            return res;
        }
        private bool _canManageItem(aspnet_Users user)
        {
            var res = false;
            var isAdmin = user.aspnet_Roles.Any(x => x.RoleName == "admin");
            if (user!=null && isAdmin)
            {
                res = true;
            }
            return res;
        }
        #endregion
        #region Documents
        public List<h_documents> GetDocuments(aspnet_Users user, out string msg)
        {
            msg = "";
            List<h_documents> res;
            try
            {
                if (!_canAccessToItem(user))
                { 
                    msg = "Нет прав на получение списка элементов!";
                    res = null;
                }
                else
                {
                    res = _db.GetDocuments().Where(x => x.isDeleted != true).ToList();
                }
            }
            catch (Exception e)
            { 
            _debug(e, new { }, "Ошибка возникла при получении списка документов");
            res = null;
            }
            return res;
        }
        public h_documents GetDocument(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_documents res;
            try
            {
                if (!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение документа";
                    res = null;
                }
                else
                {
                    res = _db.GetDocument(id);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении одного документа по id");
                res = null;
            }
            return res;
        }
        public h_documents CreateDocument(Dictionary <string,string> parameters, aspnet_Users user, out string msg)
        {
            msg = "";
            h_documents res;
            try
            {
                if (!_canAccessToItem(user))
                {
                    msg = "Нет прав на создание документа";
                    res = null;
                }
                else
                {
                    res = new h_documents();
                    res.uniqueCode = Guid.NewGuid();
                    res.isDeleted = false;
                    foreach (var key in parameters.Keys)
                    {
                        switch (key)
                        {
                            case "number": res.number = parameters[key];
                                break;
                            case "date": res.date = RDL.Convert.StrToDateTime(parameters[key], DateTime.Now);
                                break;
                            case "sum": res.sum = RDL.Convert.StrToDecimal(parameters[key], 0);
                                break;
                            case "description": res.description = parameters[key];
                                break;
                            case "link": res.link = parameters[key];
                                break;
                            case "contragentName": res.contragentID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "docStatus": res.docStatusID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "docTypes": res.docTypeID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "ParentDocs": res.docParentID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                        }
                    }
                _db.SaveDocument(res);
            }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при создании нового документа");
                res = null;
            }
            return res;
        }
        public h_documents EditDocument(Dictionary<string ,string> parameters, int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_documents res;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав на редактирование элемента";
                    res = null;
                }
                else
                {
                    res = _db.GetDocument(id);
                    foreach(var key in parameters.Keys)
                    {
                        switch (key)
                        {
                            case "number":res.number = parameters[key];
                                break;
                            case "date":
                                res.date = RDL.Convert.StrToDateTime(parameters[key], DateTime.Now);
                                break;
                            case "sum":
                                res.sum = RDL.Convert.StrToDecimal(parameters[key], 0);
                                break;
                            case "description":
                                res.description = parameters[key];
                                break;
                            case "link":
                                res.link = parameters[key];
                                break;
                            case "contragentName":
                                res.contragentID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "docStatus":
                                res.docStatusID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "docTypes":
                                res.docTypeID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                            case "ParentDocs":
                                res.docParentID = RDL.Convert.StrToInt(parameters[key], 0);
                                break;
                        }
                    }
                    _db.SaveDocument(res);
                }
            }
            catch(Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = null;
            }
            return res;
        }
        public bool ChangeDocumentInLine(int id, string name, string value, aspnet_Users user, out string msg)
        {
            bool res = false;
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
                    var item = _db.GetDocument(id);
                    switch (name)
                    {
                        case "date": item.date = RDL.Convert.StrToDateTime(value, DateTime.Now);
                            res = true;
                            break;
                        case "sum":item.sum = RDL.Convert.StrToDecimal(value, 0);
                            res = true;
                            break;
                        case "link": item.link = value;
                            res = true;
                            break;
                        case "docStatus": item.docStatusID = RDL.Convert.StrToInt(value, 0);
                            res = true;
                            break;
                    }
                    if (res)
                    {
                        _db.SaveDocument(item);
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

        public bool RemoveDocument(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            bool res;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав на удаление элемента";
                    res = false;
                }
                else
                {
                    var item = _db.GetDocument(id);
                    item.isDeleted = true;
                    _db.SaveDocument(item);
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
        #region DocTypes
        public List<h_docTypes> GetDocTypes(aspnet_Users user, out string msg)
        {
            msg = "";
            List<h_docTypes> res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение списка элемента!";
                    res = null;
                }
                else
                {
                    res = _db.GetDocTypes().ToList();
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении списка элементов");
                res = null;
            }
            return res;
        }
        public h_docTypes GetDocType(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_docTypes res;
            try
            {
                if (!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение элемента по id";
                    res = null;
                }
                else
                {
                    res = _db.GetDocType(id);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении элемента по id");
                res = null;
            }
            return res;
        }
        public h_docTypes CreateDocType(Dictionary<string ,string> parameters, aspnet_Users user, out string msg)
        {
            msg = "";
            h_docTypes res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = "Нет прав на создание элемента";
                    res = null;
                }
                else
                {
                    res = new h_docTypes();
                    foreach(var key in parameters.Keys)
                        switch(key)
                        { 
                            case "name": res.name = parameters[key];
                                break;
                            case "code": res.code = parameters[key];
                                break;
                                
                        }
                    _db.SaveDocType(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при создании элемента");
                res = null;
            }
            return res;
        }
        public h_docTypes EditDocType(Dictionary<string ,string> parameters, int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_docTypes res;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав на редактирование элемента";
                    res = null;
                }
                else
                {
                    res = _db.GetDocType(id);
                    foreach(var key in parameters.Keys)
                        switch (key)
                        {
                            case "name": res.name = parameters[key];
                                break;
                            case "code": res.code = parameters[key];
                                break;
                        }
                    _db.SaveDocType(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = null;
            }
            return res;
        }
        public bool RemoveDocType(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            bool res = false;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав на удаление пользователя";
                    res = false;
                }
                else
                {
                    _db.DeleteDocType(id);
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
        #region DocStatuses
        public List<h_docStatuses> GetDocStatuses(aspnet_Users user, out string msg)
        {
            msg = "";
            List<h_docStatuses> res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение списка элементов!";
                    res = null;
                }
                else
                {
                    res = _db.GetDocStatuses().ToList();
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении списка элементов");
                res = null;
            }
            return res;
        }
        public h_docStatuses GetDocStatus(int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_docStatuses res;
            try
            {
                if(!_canAccessToItem(user))
                {
                    msg = "Нет прав на получение элемента по id";
                    res = null;
                }
                else
                {
                    res = _db.GetDocStatus(id);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при получении элемента по id");
                res = null;
            }
            return res;
        }
        public h_docStatuses CreateDocStatus(Dictionary<string , string> parameters, aspnet_Users user, out string msg)
        {
            msg = "";
            h_docStatuses res;
            try
            {
               if(!_canAccessToItem(user))
                {
                    msg = "Нет прав создавать элемента";
                    res = null;
                }
               else
                {
                    res = new h_docStatuses();
                    foreach (var key in parameters.Keys)
                        switch (key)
                        {
                            case "name": res.name = parameters[key];
                                break;
                            case "code": res.code = parameters[key];
                                break;
                        }
                    _db.SaveDocStatus(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при создании элемента");
                res = null;
            }
            return res;
        }
        public h_docStatuses EditDocStatus(Dictionary <string,string> parameters, int id, aspnet_Users user, out string msg)
        {
            msg = "";
            h_docStatuses res;
            try
            {
                if(!_canManageItem(user))
                {
                    msg = "Нет прав редактировать элемента";
                    res = null;
                }
                else
                {
                    res = _db.GetDocStatus(id);
                    foreach(var key in parameters.Keys)
                    {
                        switch(key)
                        {
                            case "name":res.name = parameters[key];
                                break;
                            case "code":res.code = parameters[key];
                                break;
                        }
                    }
                    _db.SaveDocStatus(res);
                }
            }
            catch (Exception e)
            {
                _debug(e, new { }, "Ошибка возникла при изменении элемента");
                res = null;
            }
            return res;
        }
        public bool RemoveDocStatus(int id, aspnet_Users user, out string msg)
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
                    _db.DeleteDocStatus(id);
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
        //"Нет прав на получение элемента по id"
        //"Ошибка возникла при получении элемента по id"
        //msg = "Нет прав создавать элемента";
        //msg = "Нет прав редактировать элемента";
        //"Ошибка возникла при изменении элемента"
        //msg = "Нет прав на удаление элемента";
        //"Ошибка возникла при удалении элемента"
    }
}