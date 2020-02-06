using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace arkAS.BLL.SmallData
{
    public class SmallDataRepository
    {
        public LocalSqlServer db;

        public SmallDataRepository()
        {
            db = new LocalSqlServer();
        }

        public List<SmallDataTest> GetTable()
        {
            var res = new List<SmallDataTest>();
            res = db.SmallDataTest.ToList();
            return res;
        }

        public int GetTableItem(int productID, string UserName)
        {
            int res = 0;
            res = db.SmallDataTest.Where(x => x.productID == productID && x.UserName == UserName).Select(x => x.id).FirstOrDefault();
            return res;
        }

        public int SaveTable(SmallDataTest element)
        {
            try
            {
                int res = GetTableItem(element.productID, element.UserName);
                if (res == 0)
                {
                    db.SmallDataTest.Add(element);
                    db.SaveChanges();
                }
                else
                {
                    element.id = res;
                    try
                    {                
                        db.Entry(element).State = EntityState.Modified;
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
            return 1;
        }
    }
}