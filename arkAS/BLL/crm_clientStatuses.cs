
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
    
public partial class crm_clientStatuses
{

    public crm_clientStatuses()
    {

        this.crm_clients = new HashSet<crm_clients>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }

    public string color { get; set; }

    public string state { get; set; }



    public virtual ICollection<crm_clients> crm_clients { get; set; }

}

}
