
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
    
public partial class Mikhailova_statuses_invoces
{

    public Mikhailova_statuses_invoces()
    {

        this.Mikhailova_invoices = new HashSet<Mikhailova_invoices>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }

    public string color { get; set; }



    public virtual ICollection<Mikhailova_invoices> Mikhailova_invoices { get; set; }

}

}
