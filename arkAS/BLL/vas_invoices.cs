
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
    
public partial class vas_invoices
{

    public int id { get; set; }

    public System.DateTime date { get; set; }

    public string number { get; set; }

    public int contractorID { get; set; }

    public int statusID { get; set; }

    public string comment { get; set; }

    public string code { get; set; }



    public virtual vas_contractors vas_contractors { get; set; }

    public virtual vas_invoiceStatuses vas_invoiceStatuses { get; set; }

}

}
