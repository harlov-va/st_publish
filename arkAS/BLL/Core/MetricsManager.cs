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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace arkAS.BLL.Core
{
    public class MetricsManager
    {
          #region System
        public CoreRepository db;
        private bool _disposed;

        public MetricsManager()
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

        public List<as_mt_metricTypes> GetTypes(string code)
        {
            var res = new List<as_mt_metricTypes>();
            var roles = Roles.GetRolesForUser();
            var username = HttpContext.Current.User.Identity.Name;
            try
            {
                var mT = db.db.as_mt_metrics.ToList()
                    .Where(x => HaveCurrentUserAccess(x, roles, username))
                    .Select(x => x.typeID)
                    .Distinct();
                res = db.db.as_mt_metricTypes
                    .Where(x => mT.Contains(x.id))
                    .OrderBy(x => x.ord).ToList();
            }
            catch (Exception ex) {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        public as_mt_metricTypes GetType(int? typeId)
        {
            var res = new as_mt_metricTypes();
            try
            {
                res = db.db.as_mt_metricTypes.ToList().FirstOrDefault(x => x.id == typeId);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        public List<as_mt_metrics> GetAllMetrics(bool all)
        {
            if (all)
                return db.db.as_mt_metrics.OrderBy(x => x.ord).ToList();
            else
                return db.db.as_mt_metrics.OrderBy(x => x.ord).ToList().Where(x => HaveCurrentUserAccess(x)).ToList();
        }

        public List<as_mt_metricTypes> GetAllMetricTypes()
        {
            var res = new List<as_mt_metricTypes>();
            res = db.db.as_mt_metricTypes.OrderBy(x => x.ord).ToList().ToList();
            return res;
        }

        public List<as_mt_metrics> GetMetrics(int typeID)
        {
            var res = new List<as_mt_metrics>();
            res = db.db.as_mt_metrics.Where(x=>x.typeID== typeID && !x.parentID.HasValue).OrderBy(x=>x.ord).ToList().Where(x => HaveCurrentUserAccess(x)).ToList();
            return res;
        }

        public void SaveMetric(as_mt_metrics item)
        {
            try
            {
                db.SaveMt_metric(item);
                RDL.CacheManager.PurgeCacheItems("as_mt_metrics");
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }


        public as_mt_metrics GetMetric(int metricID, ArrayList row, out DataTable dt)
        {
            dt = new DataTable();           
            var res = new as_mt_metrics();
            res = db.db.as_mt_metrics.FirstOrDefault(x => x.id == metricID);
            if (!HaveCurrentUserAccess(res)) return res;

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServerSimple"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(res.sql, cn);
                    cmd.CommandType = res.isSP==true ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.Parameters.AddWithValue("@username", User.CurrentUser.Identity.Name);

                 //   cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = "dd";
                 //   cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = "ddds";

                    if (row != null)
                    {
                        foreach (var par in row)
                        {
                            var item = par as Dictionary<string, object>;
                            cmd.Parameters.AddWithValue("@"+item["colname"].ToString(), item["value"] != null ? item["value"].ToString() : "");
                        }
                    }
                    
                    cn.Open();
                   SqlDataAdapter da = new SqlDataAdapter(cmd);

                   // var reader = cmd.ExecuteReader();
                   da.Fill(dt);
                            
                }
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
            return res;
        }

        /*public string[] getRolesForMetric(int metricID)
        {
            List<String> res = new List<String>();
            List<as_mt_metrics> list = db.db.as_mt_metrics.Where(x => x.id == metricID).ToList();
            foreach (as_mt_metrics item in list)
            {
                var r = item.roles;
                res.AddRange(r.Split(','));
            }
            return res.ToArray();
        }*/

        public string[] getRolesForMetric(int metricID) 
        { 
            List<String> res = new List<String>(); 
            as_mt_metrics m = db.db.as_mt_metrics.FirstOrDefault(x => x.id == metricID); 
            if (m != null) 
            { 
                res.AddRange(m.roles.Split(',')); 
            } 
            return res.ToArray(); 
        }
        private bool HaveCurrentUserAccess(as_mt_metrics m)
        {
            var res = false;
            var roles = Roles.GetRolesForUser();
            var username = HttpContext.Current.User.Identity.Name;

            var mRoles = (m.roles ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var mUsers = (m.users ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            res = mUsers.Contains(username) || mRoles.Any(x => roles.Contains(x));
            return res;
        }

        private bool HaveCurrentUserAccess(as_mt_metrics m, string[] roles, string username) {
            var res = false;

            var mRoles = (m.roles ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var mUsers = (m.users ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            res = mUsers.Contains(username) || mRoles.Any(x => roles.Contains(x));
            return res;        
        }

        public void DeleteMetric(int id)
        {
            try
            {
                db.DeleteMt_metrics(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void SaveMetricType(as_mt_metricTypes item)
        {
            try
            {
                db.SaveMt_metricType(item);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }
        public void DeleteMetricType(int id)
        {
            try
            {
                db.DeleteMt_metricType(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

    }
}