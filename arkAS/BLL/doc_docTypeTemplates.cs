
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
    
public partial class doc_docTypeTemplates
{

    public int id { get; set; }

    public string name { get; set; }

    public int typeID { get; set; }

    public string path { get; set; }

    public Nullable<int> ord { get; set; }



    public virtual doc_docTypes doc_docTypes { get; set; }

}

}
