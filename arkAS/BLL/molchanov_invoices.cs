
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
    
public partial class molchanov_invoices
{

    public molchanov_invoices()
    {

        this.molchanov_logInvoices = new HashSet<molchanov_logInvoices>();

    }


    public int id { get; set; }

    public System.Guid uniqueCode { get; set; }

    public System.DateTime date { get; set; }

    public string number { get; set; }

    public string description { get; set; }

    public Nullable<bool> isDeleted { get; set; }

    public int invStatusID { get; set; }

    public int contragentID { get; set; }



    public virtual molchanov_contragents molchanov_contragents { get; set; }

    public virtual molchanov_invoiceStatuses molchanov_invoiceStatuses { get; set; }

    public virtual ICollection<molchanov_logInvoices> molchanov_logInvoices { get; set; }

}

}