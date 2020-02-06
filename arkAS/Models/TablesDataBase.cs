using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Models
{
    public class TableDataBase
    {
        public int code { get; set; }
        public string view_name { get; set; }
        public string db_name { get; set; }
    }

    public class TablesDataBase
    {
        public List<TableDataBase> listTablesDataBase = new List<TableDataBase>();

        public TablesDataBase()
        {
            listTablesDataBase.Add(new TableDataBase
            {
                code = 1,
                db_name = "fin_finances",
                view_name = "Финансы"
            });

            listTablesDataBase.Add(new TableDataBase
            {
                code = 2,
                db_name = "fin_contragents",
                view_name = "Контрагенты"
            });

            listTablesDataBase.Add(new TableDataBase
            {
                code = 3,
                db_name = "imp_itemLog",
                view_name = "Логи"
            });
        }

        public void SetTablesDataBase(List<TableDataBase> list)
        {
            listTablesDataBase = list;
        }

        public List<TableDataBase> GetTablesDataBaseList()
        {
            return listTablesDataBase;
        }

        public TableDataBase GetTableDataBaseByCode(int code)
        {
            return listTablesDataBase.First(i => i.code == code);
        }
    }
}