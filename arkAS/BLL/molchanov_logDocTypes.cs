
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
    
public partial class molchanov_logDocTypes
{

    public int id { get; set; }

    public System.DateTime date { get; set; }

    public string notice { get; set; }

    public string userName { get; set; }

    public int docTypeID { get; set; }



    public virtual molchanov_docTypes molchanov_docTypes { get; set; }

}

}
