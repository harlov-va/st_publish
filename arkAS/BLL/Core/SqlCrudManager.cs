using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using RDL;

namespace arkAS.BLL.Core
{
    public class SqlCrudManager
    {
        public CoreRepository db;
        private bool _disposed;
        public SqlCrudManager()
        {
            db = new CoreRepository();
            _disposed = false;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public List<as_sqlGet> GetSqlByRole(string role)
        {
            var res = new List<as_sqlGet>();
            try
            {
                res = GetSqlCruds().Where(x => x.as_sqlRole.Any(y => y.role == role)).ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return res;
        }
        public string GetSqlByID(int id)
        {
            var res = "";
            try
            {
                res = GetSqlCruds().Where(x=>x.id==id).Select(x=>x.sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return res;
        }
        public void DeleteSqlRole(int sqlID, string role)
        {
            try
            {
                var item = db.db.as_sqlRole.FirstOrDefault(x => x.role == role && x.sqlID == sqlID);
                if (item != null)
                {
                    db.DeleteSqlRole(item.id);
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void SaveSqlRole(as_sqlRole item)
        {
            try
            {
                db.SaveSqlRole(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void addMenuToRole(int menuID, string role)
        {
            var item = new as_menuRoles { id = 0, itemID = menuID, role = role, ord = null };
            db.SaveMenuRole(item);
            RDL.CacheManager.PurgeCacheItems("as_menu");
        }

        public void removeMenuToRole(int menuID, string role)
        {
            var item = db.GetMenuRole(menuID, role);
            if (item != null)
            {
                db.DeleteMenuRole(item.id);
                RDL.CacheManager.PurgeCacheItems("as_menu");
            }
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
        
        public List<as_sqlGet> GetSqlCruds()
        {
            var res = new List<as_sqlGet>();
            var key = "as_sqlGet";
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false)
            {
                res = (List<as_sqlGet>)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSqlCruds();
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public List<as_sqlRole> GetRolesBySql(int sqlID)
        {
            var res = new List<as_sqlRole>();
            try
            {
                res = db.db.as_sqlRole.Where(x => x.sqlID== sqlID).ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return res;
        }
        public bool DeleteRolesBySqlID(int sqlID)
        {
            var res = false;
            var list = new List<as_sqlRole>();

            try
            {
                list = GetRolesBySql(sqlID);
                foreach (as_sqlRole item in list)
                {
                    db.DeleteSqlRole(item.id);
                }
                res = true;
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);

            }
            return res;
        }
        public void DeleteSqlCrud(as_sqlGet item)
        {
            try
            {
                RDL.CacheManager.PurgeCacheItems("as_sqlGet_id__" + item.code);
                db.DeleteSqlCrud(item.id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public as_sqlGet GetSql(int id)
        {
            var res = new as_sqlGet();
            var key = "as_sqlGet_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false)
            {
                res = (as_sqlGet)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSqlGet(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res; 
        }
        public as_sqlGet GetSqlCrud(int id)
        {
            var res = new as_sqlGet();
            var key = "as_sqlCrud_id_" + id;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false)
            {
                res = (as_sqlGet)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSqlGet(id);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public DataTable GetSqlCrud(string command, ArrayList row)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    
                    
                        try
                        {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(command, cn);
                            cmd.CommandType = CommandType.Text;
                           
                           
                            dt.Load(cmd.ExecuteReader());
                        }
                        catch (Exception)
                        {

                            Debug.WriteToTraceFile("C:/123.txt");
                        }


                    

                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return dt;
        }
        public void SaveDocLogs(as_sqlGet item)
        {
            try
            {
                db.SaveSqlCrud(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public as_sqlGet GetSqlGet(string code)
        {
            var res = new as_sqlGet();
            var key = "as_sqlGet_code_" + code;
            if (CacheManager.EnableCaching && CacheManager.Cache[key] != null && false)
            {
                res = (as_sqlGet)CacheManager.Cache[key];
            }
            else
            {
                try
                {
                    res = db.GetSql(code);
                    CacheManager.CacheData(key, res);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return res;
        }
        public void SaveSql(as_sqlGet item)
        {
            try
            {

                db.SaveSql(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        internal void EditRightField(int pk, string name, string value)
        {
            var sql = GetSql(pk);
            switch (name)
            {
                case "sql": sql.sql = value; break;
                case "code": sql.code = value; break;
            }
            SaveSql(sql);
        }
    }
}