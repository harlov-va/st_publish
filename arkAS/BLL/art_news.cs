
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
    
public partial class art_news
{

    public int id { get; set; }

    public string title { get; set; }

    public string text { get; set; }

    public Nullable<System.DateTime> created { get; set; }

    public Nullable<int> typeID { get; set; }

    public string imgPath { get; set; }

    public string anouns { get; set; }



    public virtual art_newsType art_newsType { get; set; }

}

}
