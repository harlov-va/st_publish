
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
    
public partial class gurevskiy_invoices
{

    public int id { get; set; }

    public int statusID { get; set; }

    public int partnerID { get; set; }

    public System.DateTime date { get; set; }

    public string number { get; set; }

    public string comment { get; set; }



    public virtual gurevskiy_partner gurevskiy_partner { get; set; }

    public virtual gurevskiy_invoiceStatuses gurevskiy_invoiceStatuses { get; set; }

}

}
