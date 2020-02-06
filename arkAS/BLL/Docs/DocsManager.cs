using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Docs
{
    public class DocsManager
    {
        #region System
        public DocsRepository db;
        private bool _disposed;
        
        public DocsManager()
        {
            db = new DocsRepository();
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
        #endregion

        #region Docs

        public doc_docs GetDoc(int id)
        {
            var res = new doc_docs();
            res = db.GetDoc(id);
            return res;
        }

        public List<doc_docs> GetDocs()
        {
            var res = new List<doc_docs>();
            res = db.GetDocs();
            return res;
        }

        public void SaveDoc(doc_docs item)
        {
            try
            {
                db.SaveDocs(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDoc(int id)
        {
            try
            {
                db.DeleteDoc(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditDocField(int pk, string name, string value)
        {
            var item = GetDoc(pk);
            switch (name)
            {
                case "name": item.name = value; break;
                case "number": item.number = value; break;
                case "path": item.path = value; break;
                case "typeName": if (value != "") item.typeID = RDL.Convert.StrToInt(value, 0); break;
                case "statusName": if (value != "") item.statusID = RDL.Convert.StrToInt(value, 0); break;
                case "projectName": if (value != "") item.projectID = RDL.Convert.StrToInt(value, 0); break;
                case "contragentName": if (value != "") item.contragentID = RDL.Convert.StrToInt(value, 0); break;
            }
            SaveDoc(item);
        }
        
        #endregion

        #region DocTypes

        public doc_docTypes GetDocType(int id)
        {
            var res = new doc_docTypes();
            res = db.GetDocType(id);
            return res;
        }

        public List<doc_docTypes> GetDocTypes()
        {
            var res = new List<doc_docTypes>();
            res = db.GetDocTypes();
            return res;
        }

       
        
        public void SaveDocType(doc_docTypes item)
        {
            try
            {
                db.SaveDocType(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocType(int id)
        {
            try
            {
                db.DeleteDocType(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region DocStatuses

        public doc_docStatuses GetDocStatus(int id)
        {
            var res = new doc_docStatuses();
            res = db.GetDocStatus(id);
            return res;
        }

        public List<doc_docStatuses> GetDocStatuses()
        {
            var res = new List<doc_docStatuses>();
            res = db.GetDocStatuses();
            return res;
        }

        public void SaveDocStatus(doc_docStatuses item)
        {
            try
            {
                db.SaveDocStatus(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocStatus(int id)
        {
            try
            {
                db.DeleteDocStatus(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

        #region DocTypeTemplates

        public doc_docTypeTemplates GetDocTypeTemplate(int id)
        {
            var res = new doc_docTypeTemplates();
            res = db.GetDocTypeTemplate(id);
            return res;
        }

        public List<doc_docTypeTemplates> GetDocTypeTemplates()
        {
            var res = new List<doc_docTypeTemplates>();
            res = db.GetDocTypeTemplates();
            return res;
        }

        public List<doc_docTypeTemplates> GetListTemplatesByType(int typeId)
        {
            var res = new List<doc_docTypeTemplates>();
            res = db.GetListTemplatesByType(typeId);
            return res;
        }



        public void SaveDocTypeTemplate(doc_docTypeTemplates item)
        {
            try
            {
                db.SaveDocTypeTemplate(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocTypeTemplate(int id)
        {
            try
            {
                db.DeleteDocTypeTemplate(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion
        internal void EditTextField(int pk, string name, string value)
        {
            var item = db.GetDocLog(pk);
            switch (name)
            {
                case "name": item.docID= RDL.Convert.StrToInt(value, 0); break;
                case "isDownload": item.isDownload= RDL.Convert.StrToBoolean(value); break;
                case "createdBy": item.docID = RDL.Convert.StrToInt(value,0); break;
                case "created": item.created = RDL.Convert.StrToDateTime(value, DateTime.Now); break;
            }
            SaveDocLogs(item);
        }
        internal void EditTextField2(int pk, string name, string value)
        {
            var item = db.GetDocVersion(pk);
            switch (name)
            {
                case "name": item.docID = RDL.Convert.StrToInt(value, 0); break;
                case "decs": item.decs= value; break;
                case "createdBy": item.docID = RDL.Convert.StrToInt(value, 0); break;
                case "created": item.created = RDL.Convert.StrToDateTime(value, DateTime.Now); break;
            }
            SaveDocVersions(item);
        }
        #region DocLogs
        public doc_docLogs GetDocLog(int id)
        {
            var res = new doc_docLogs();
            res = db.GetDocLog(id);
            return res;
        }

        public List<doc_docLogs> GetDocLogs()
        {
            var res = new List<doc_docLogs>();
            res = db.GetDocLogs();
            return res;
        }

        public void SaveDocLogs(doc_docLogs item)
        {
            try
            {
                db.SaveDocLogs(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocLogs(int id)
        {
            try
            {
                db.DeleteDocLogs(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        
        #endregion

        #region DocVersion

        public doc_docVersions GetDocVersion(int id)
        {
            var res = new doc_docVersions();
            res = db.GetDocVersion(id);
            return res;
        }

        public List<doc_docVersions> GetDocVersions()
        {
            var res = new List<doc_docVersions>();
            res = db.GetDocVersions();
            return res;
        }

        public void SaveDocVersions(doc_docVersions item)
        {
            try
            {
                db.SaveDocVersion(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteDocVersions(int id)
        {
            try
            {
                db.DeleteDocVersion(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion

      
    }
}