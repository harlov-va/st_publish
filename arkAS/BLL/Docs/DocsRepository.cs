using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace arkAS.BLL.Docs
{
    public class DocsRepository
    {
        #region System

        public LocalSqlServer db;
        private bool _disposed;

        public DocsRepository()
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
        #endregion

        #region Docs

        public doc_docs GetDoc(int id)
        {
            var res = db.doc_docs.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<doc_docs> GetDocs()
        {
            var res = db.doc_docs.ToList();
            return res;
        }

        public int SaveDocs(doc_docs item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.doc_docs.Add(item);
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

        public bool DeleteDoc(int id)
        {
            var res = false;
            try
            {
                var item = db.doc_docs.SingleOrDefault(x => x.id == id);
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

        #region DocTypes

        public doc_docTypes GetDocType(int id)
        {
            var res = db.doc_docTypes.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<doc_docTypes> GetDocTypes()
        {
            var res = db.doc_docTypes.ToList();
            return res;
        }

        public int SaveDocType(doc_docTypes item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.doc_docTypes.Add(item);
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

        public bool DeleteDocType(int id)
        {
            var res = false;
            try
            {
                var item = db.doc_docTypes.SingleOrDefault(x => x.id == id);
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

        #region DocStatuses

        public doc_docStatuses GetDocStatus(int id)
        {
            var res = db.doc_docStatuses.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<doc_docStatuses> GetDocStatuses()
        {
            var res = db.doc_docStatuses.ToList();
            return res;
        }

        public int SaveDocStatus(doc_docStatuses item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.doc_docStatuses.Add(item);
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

        public bool DeleteDocStatus(int id)
        {
            var res = false;
            try
            {
                var item = db.doc_docStatuses.SingleOrDefault(x => x.id == id);
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

        #region DocTypeTemplates

        public doc_docTypeTemplates GetDocTypeTemplate(int id)
        {
            var res = db.doc_docTypeTemplates.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<doc_docTypeTemplates> GetDocTypeTemplates()
        {
            var res = db.doc_docTypeTemplates.ToList();
            return res;
        }

        public List<doc_docTypeTemplates> GetListTemplatesByType(int typeId)
        {
            var res = db.doc_docTypeTemplates.Where(x=> x.typeID == typeId).ToList();
            return res;
        }

        public int SaveDocTypeTemplate(doc_docTypeTemplates item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.doc_docTypeTemplates.Add(item);
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

        public bool DeleteDocTypeTemplate(int id)
        {
            var res = false;
            try
            {
                var item = db.doc_docTypeTemplates.SingleOrDefault(x => x.id == id);
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

        #region DocLogs

        public doc_docLogs GetDocLog(int id)
        {
            var res = db.doc_docLogs.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<doc_docLogs> GetDocLogs()
        {
            var res = db.doc_docLogs.ToList();
            return res;
        }

        public int SaveDocLogs(doc_docLogs item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.doc_docLogs.Add(item);
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

        public bool DeleteDocLogs(int id)
        {
            var res = false;
            try
            {
                var item = db.doc_docLogs.SingleOrDefault(x => x.id == id);
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
    
        #region DocVersions

        public doc_docVersions GetDocVersion(int id)
        {
            doc_docVersions res = db.doc_docVersions.FirstOrDefault(x => x.id == id);
            return res;
        }

        public List<doc_docVersions> GetDocVersions()
        {
            var res = db.doc_docVersions.ToList();
            return res;
        }

        public int SaveDocVersion(doc_docVersions item)
        {
            try
            {
                if (item.id == 0)
                {
                    db.doc_docVersions.Add(item);
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

        public bool DeleteDocVersion(int id)
        {
            var res = false;
            try
            {
                var item = db.doc_docVersions.SingleOrDefault(x => x.id == id);
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

      

    }
}