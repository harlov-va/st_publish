using System;
using System.Collections.Generic;
using System.Linq;
using arkAS.BLL;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using RDL;
using System.Web.Security;

namespace arkAS.BLL.Core
{
    public class RightsManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public RightsManager()
        {
            db = new CoreRepository();
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

        public as_rights GetRight(int id)
        {
            var res = new as_rights();
            var key = "as_rights_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false) {
                res = (as_rights)CacheManager.Cache[key];
            }else{
                try {
                    res = db.GetRight(id);
                    CacheManager.CacheData(key, res);
                }catch(Exception ex){
                    Debug.LogError(ex);
                }           
            }
            return res;        
        }


        public as_rights GetRight(string code)
        {
            var res = new as_rights();
            var key = "as_rights_code_" + code;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false)
            {
                res = (as_rights)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetRight(code);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public List<as_rights> GetRightsByRole(string role)
        {
            var res = new List<as_rights>();
            try
            {
                res = GetRights().Where(x => x.as_rightsRoles.Any(y => y.role == role)).ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
           
            return res;
        }
       
        public bool CheckRightForRole(string role,string rightCode)
        {
            bool res = false;
            try
            {
                res = db.db.as_rightsRoles.Any(x=>x.role==role && x.as_rights.code == rightCode);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return res;
        }

        public bool CheckRightForUser(string username, string rightCode)
        {
            bool res = false;
            try
            {
                var roles = Roles.GetRolesForUser(username);

                res = db.db.as_rightsRoles.Any(x => roles.Contains(x.role) && x.as_rights.code == rightCode);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return res;
        }
        public List<as_rights> GetRights()
        {
            var res = new List<as_rights>();
            var key = "as_rights";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false) {
                res = (List<as_rights>)CacheManager.Cache[key];
            }else{
                try {
                    res = db.GetRights();
                    CacheManager.CacheData(key, res);
                }catch(Exception ex){
                    Debug.LogError(ex);
                }           
            }
            return res;        
        }
        public List<as_rightsRoles> GetRolesByRight(int rightID)
        {
            var res = new List<as_rightsRoles>();
            try
            {
                res = db.db.as_rightsRoles.Where(x => x.rightID == rightID).ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return res;
        }
        public bool DeleteRolesByRightsID(int rightID)
        {
            var res = false;
            var list=new List<as_rightsRoles>();
           
            try
            {               
                list = GetRolesByRight(rightID);
                foreach (as_rightsRoles item in list)
                {
                    db.DeleteRightRole(item.id);
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
               
            }         
            return res;
        }
        public void SaveRight(as_rights item)
        {
            try {

                db.SaveRight(item);
            }catch(Exception ex){
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteRight(as_rights item)
        {
            try
            {

              
                db.DeleteRight(item.id);
                

            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void SaveRightRole(as_rightsRoles item)
        {
            try
            {
                db.SaveRightRole(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteRightRole(int rightID, string role)
        {
            try
            {
                var item = db.db.as_rightsRoles.FirstOrDefault(x=>x.role==role && x.rightID==rightID);
                if (item != null) {
                    db.DeleteRightRole(item.id);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        internal void EditRightField(int pk, string name, string value)
        {
            var right = GetRight(pk);
            switch (name) {
                case "name": right.name = value; break;
                case "code": right.code = value; break;
            }
            SaveRight(right);
        }
    }
}