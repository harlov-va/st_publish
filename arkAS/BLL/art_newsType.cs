
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
    
public partial class art_newsType
{

    public art_newsType()
    {

        this.art_news = new HashSet<art_news>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string code { get; set; }



    public virtual ICollection<art_news> art_news { get; set; }

}

}
