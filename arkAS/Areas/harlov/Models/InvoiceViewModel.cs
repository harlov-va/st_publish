using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.Models
{
    public class InvoiceViewModel
    {
        public List<h_contragents> Contragents { get; set; }
        public List<h_invoiceStatuses> InvStatuses { get; set; }
    }
}