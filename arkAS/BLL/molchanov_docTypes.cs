
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
    
public partial class molchanov_docTypes
{

    public molchanov_docTypes()
    {

        this.molchanov_documents = new HashSet<molchanov_documents>();

        this.molchanov_logDocTypes = new HashSet<molchanov_logDocTypes>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }



    public virtual ICollection<molchanov_documents> molchanov_documents { get; set; }

    public virtual ICollection<molchanov_logDocTypes> molchanov_logDocTypes { get; set; }

}

}
