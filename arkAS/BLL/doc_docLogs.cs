
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
    
public partial class doc_docLogs
{

    public int id { get; set; }

    public Nullable<int> docID { get; set; }

    public bool isDownload { get; set; }

    public Nullable<System.DateTime> created { get; set; }

    public string createdBy { get; set; }



    public virtual doc_docs doc_docs { get; set; }

}

}
