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
using System.Web.Mvc;

namespace arkAS.BLL.Core
{
    public class SqlQueries
    {
        #region System
        public LocalSqlServer db;
        private bool _disposed;

        public SqlQueries()
        {
            db = new LocalSqlServer();
            _disposed = false;
            //SERIALIZE WILL FAIL WITH PROXIED ENTITIES
            //dbContext.Configuration.ProxyCreationEnabled = false;
            //ENABLING COULD CAUSE ENDLESS LOOPS AND PERFORMANCE PROBLEMS
            //dbContext.Configuration.LazyLoadingEnabled = false;
        }

        private void _debug(Exception ex, Object parameters, string additions = "")
        {
            RDL.Debug.LogError(ex, additions, parameters);
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

        #region SqlQueries

        public List<string> GetUnregQueries(List<rosh_sqlQueriesTable> inpParameters)//ненормированные запросы
        {
            List<string> res = new List<string>();
            string msg = "";

            try
            {
                foreach (var x in inpParameters)
                {
                    var con = x.con;
                    var manager = x.manager;
                    var rowsLimit = x.rowsLimit;
                    var timeLimit = x.timeLimit;

                    try
                    {
                        using (SqlConnection conStr = new SqlConnection(ConfigurationManager.ConnectionStrings[con].ConnectionString))
                        {
                            string SqlQuery = "DECLARE @rowsLimit INT = 0, @timeLimit INT = 0  SELECT TOP 5 st.text, max_rows rows, qp.query_plan, qs.* FROM sys.dm_exec_query_stats qs CROSS APPLY sys.dm_exec_sql_text(qs.plan_handle) st CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp WHERE max_rows > @rowsLimit AND max_elapsed_time > @timeLimit ORDER BY max_rows DESC";

                            conStr.Open();

                            using (var cmd = conStr.CreateCommand())
                            {
                                cmd.CommandText = SqlQuery;
                                cmd.Parameters.AddWithValue("rowsLimit", rowsLimit);
                                cmd.Parameters.AddWithValue("timeLimit", timeLimit);                                

                                var items = conStr.Query(SqlQuery);

                                if (items != null && (int)items.Count() > 0)
                                {
                                    string str = "";

                                    foreach (var item in items)
                                    {
                                        var data = (IDictionary<string, object>)item;
                                        var max_rows = data["max_rows"].ToString();
                                        var max_elapsed_time = data["max_elapsed_time"].ToString();

                                        str = string.Format("{0} max_rows = {1}, max_elapsed_time = {2}", str, max_rows, max_elapsed_time);
                                    }

                                    msg = "Запрос для менеджера " + manager + " содержит превышения по параметрам: " + str;
                                    res.Add(msg);

                                    SendManagerNotification(msg);//вызов метода заглушки
                                }
                            }//-
                        }
                    }
                    catch(Exception ex1)
                    {
                        _debug(ex1, new { con }, "");
                        msg = "Сбой подключения к базе данных для менеджера: " + manager;
                        res.Add(msg);
                    }                    
 
                }
            }
            catch (Exception ex)
            {
                _debug(ex, new { res }, "");
                msg = "Сбой получения входных параметров";
                res.Add(msg);
            }
            return res;
        }


        //метод заглушка, выводящий "критичные" параметры запроса
        public bool SendManagerNotification(string str)
        {
            var res = false;
            string msg = "";

            try
            {
                if (str != null)
                {
                    msg = str;
                    res = true;
                }
            }
            catch(Exception ex)
            {
                _debug(ex, new { res }, "");
                msg = "Сбой запроса";
            }
            return res;
        }

        #endregion
    }
}