using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Models
{
    public class Users
    {
        public string id { get; set; }
        public string name { get; set; }
        public string notifyEmails { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime lastActivityDate { get; set; }
    }
}