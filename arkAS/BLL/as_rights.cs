
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
    
public partial class as_rights
{

    public as_rights()
    {

        this.as_rightsRoles = new HashSet<as_rightsRoles>();

    }


    public int id { get; set; }

    public string code { get; set; }

    public string name { get; set; }



    public virtual ICollection<as_rightsRoles> as_rightsRoles { get; set; }

}

}