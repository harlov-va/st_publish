//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace arkAS.BLL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompMonitoring
    {
        public int id { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public Nullable<System.DateTime> created { get; set; }
        public string username { get; set; }
        public string url { get; set; }
        public string referrer { get; set; }
        public Nullable<System.Guid> guid { get; set; }
    }
}