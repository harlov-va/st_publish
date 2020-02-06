using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.Models
{
    public class DocumentViewModel
    {
        public List<h_contragents> Contragents { get; set; }
        public List<h_docStatuses> DocStatuses { get; set; }
        public List<h_docTypes> DocTypes { get; set; }
        public List<h_documents> Documents { get; set; }
    }
}