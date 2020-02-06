using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.ProcessItem
{
    public class ProcessItemManager
    {
        #region System

        public ProcessItemRepository db;
        private bool _disposed;

        public ProcessItemManager()
        {
            db = new ProcessItemRepository();
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

        #region ProcessItem

        public Dictionary<int, string> GetProcessList()
        {
            var res = db.GetProcessList();
            return res;
        }

        public List<proc_processItems> GetProcessItems()
        {
            var res = new List<proc_processItems>();
            res = db.GetProcessItems();
            return res;
        }

        public proc_processItems GetProcessItem(int id)
        {
            var res = new proc_processItems();
            res = db.GetProcessItem(id);
            return res;
        }

        public void SaveProcessItem(proc_processItems item)
        {
            try
            {
                db.SaveProcessItem(item);
            }
            catch(Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
        }

        public void DeleteProcessItem(int id)
        {
            try
            {
                db.DeleteProcessItem(id);
            }
            catch (Exception ex)
            {
                RDL.Debug.LogError(ex);
            }
             
        }

        public List<string> GetUserNames()
        {
            var res = db.GetUserNames();
            return res;
        }
        
        internal void processEditListField(int pk, string name, List<string> value)
        {
            var list = db.GetProcessItem(pk);
            if (list != null)
            {
                switch (name)
                {
                    case "name": list.name = value[0]; break;
                    case "desc": list.desc = value[0]; break;
                    case "color": list.color=value[0]; break;
                    case "ord": list.ord = Convert.ToInt32(value[0]); break;

                    case "processName":
                        int itemid = RDL.Convert.StrToInt(value[0], 0);
                        list.processID = itemid; break;
                  
                    case "users":
                    {
                        List<string> listUsers = GetUserNames();
                        string users = string.Empty;
                        foreach (var item in value)
                        {
                            users += listUsers[Convert.ToInt32(item)] + ",";
                        }
                        list.users = users.Substring(0, users.Length - 1);
                      break;
                    };

                    case "roles": list.roles = value[0].Replace("<br>", ","); break;
                    case "isFinish":
                    {
                       var res = value[0];

                       if (res == "1")
                       {
                           list.isFinish = true;
                       }
                       else
                       {
                           list.isFinish = false;
                       }

                       break;
                    }
                }
            }
            db.SaveProcessItem(list);
        }

        internal void ChangeRoleForProcessItem(int pk, string roles)
        {
            var list = db.GetProcessItem(pk);
            if (list != null)
            {
                list.roles = roles;
            }
            db.SaveProcessItem(list);
        }
        #endregion

    }
}