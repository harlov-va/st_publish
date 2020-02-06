using arkAS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Areas.harlov.Models
{
    public class MailViewModel
    {
        public List<h_deliverySystems> DeliverySystems { get; set; }
        public List<h_mailStatuses> MailStatuses { get; set; }
    }
}