
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
    
public partial class as_categories
{

    public as_categories()
    {

        this.as_categories1 = new HashSet<as_categories>();

    }


    public int id { get; set; }

    public string typeCode { get; set; }

    public string name { get; set; }

    public Nullable<int> parentID { get; set; }

    public string desc { get; set; }



    public virtual ICollection<as_categories> as_categories1 { get; set; }

    public virtual as_categories as_categories2 { get; set; }

}

}