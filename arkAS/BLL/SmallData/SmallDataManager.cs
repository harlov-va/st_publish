using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.BLL.SmallData
{
    public class SmallDataManager
    {
        public SmallDataRepository db;

        public SmallDataManager()
        {
            db = new SmallDataRepository();
        }

        public List<SmallDataTest> GetTable()
        {
            var res = new List<SmallDataTest>();
            res = db.GetTable();
            return res;
        }

        public int GetTableItem(int productID, string UserName)
        {
            var res = 0;
            res = db.GetTableItem(productID, UserName);
            return res;
        }

        internal int SaveTable(SmallDataTest item)
        {
            var res = db.SaveTable(item);
            return res;
        }
    }
}