using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace arkAS.BLL.Core
{
    public class AsCtrlManager
    {
        #region System
        CoreRepository db;
         private bool _disposed;
         public AsCtrlManager()
        {
            db = new CoreRepository();
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

        #region projects

        public as_ctrl GetCompMonitoring(int id)
        {
            as_ctrl res = new as_ctrl();
            res = db.GetItemAsCtrl(id);
            return res;
        }

        public List<as_ctrl> GetCompMonitorings()
        {
            var res = new List<as_ctrl>();
            res = db.GetItemsAsCtrl();
            return res;
        }

        public int SaveCompMonitoring(as_ctrl item)
        {
            int error = -1;
            try
            {
                db.SaveItemAsCtrl(item, ref error);
                error = 0;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return error;
        }

        public void DeleteCompMonitoring(int id)
        {
            try
            {
                db.DeleteItemAsCtrl(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        #endregion
    }
}