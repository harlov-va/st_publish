
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
    
public partial class h_deliverySystems
{

    public h_deliverySystems()
    {

        this.h_mails = new HashSet<h_mails>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }



    public virtual ICollection<h_mails> h_mails { get; set; }

}

}
