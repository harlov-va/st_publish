
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
    
public partial class rosh_docLogs
{

    public int id { get; set; }

    public System.DateTime date { get; set; }

    public Nullable<int> documentID { get; set; }

    public string changes { get; set; }



    public virtual rosh_documents rosh_documents { get; set; }

}

}
