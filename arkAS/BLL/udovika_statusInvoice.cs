
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
    
public partial class udovika_statusInvoice
{

    public udovika_statusInvoice()
    {

        this.udovika_invoice = new HashSet<udovika_invoice>();

    }


    public int id { get; set; }

    public string name { get; set; }



    public virtual ICollection<udovika_invoice> udovika_invoice { get; set; }

}

}