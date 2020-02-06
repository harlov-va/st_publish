using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arkAS.Models.Reclamations
{
    public class ReclamationItem
    {
        public int id { get; set; }
        public string created { get; set; }
        public string addedBy { get; set; }
        public string name { get; set; }
        //public string customerText { get; set; }
        //public string whatToDo { get; set; }
        public string reportDate { get; set; }
        public string statusName { get; set; }
        public string projectName { get; set; }
        public bool haveWOW { get; set; }
        //public string report { get; set; }
        public int number { get; set; }
    }
}