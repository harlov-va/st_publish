
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
    
public partial class tsilurik_types
{

    public tsilurik_types()
    {

        this.tsilurik_documents = new HashSet<tsilurik_documents>();

    }


    public int id { get; set; }

    public string name { get; set; }



    public virtual ICollection<tsilurik_documents> tsilurik_documents { get; set; }

}

}