
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
    
public partial class pdv_invoiceStatuses
{

    public pdv_invoiceStatuses()
    {

        this.pdv_invoices = new HashSet<pdv_invoices>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }



    public virtual ICollection<pdv_invoices> pdv_invoices { get; set; }

}

}