
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
    
public partial class gurevskiy_invoiceStatuses
{

    public gurevskiy_invoiceStatuses()
    {

        this.gurevskiy_invoices = new HashSet<gurevskiy_invoices>();

    }


    public int id { get; set; }

    public string name { get; set; }



    public virtual ICollection<gurevskiy_invoices> gurevskiy_invoices { get; set; }

}

}