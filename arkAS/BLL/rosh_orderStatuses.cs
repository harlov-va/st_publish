
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
    
public partial class rosh_orderStatuses
{

    public rosh_orderStatuses()
    {

        this.rosh_orders = new HashSet<rosh_orders>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }

    public string color { get; set; }



    public virtual ICollection<rosh_orders> rosh_orders { get; set; }

}

}