
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
    
public partial class imp_itemLog
{

    public int id { get; set; }

    public Nullable<int> itemID { get; set; }

    public Nullable<System.DateTime> created { get; set; }

    public string createdBy { get; set; }

    public string errors { get; set; }

    public string info { get; set; }

    public Nullable<int> durationSec { get; set; }

    public Nullable<bool> withBackup { get; set; }

    public Nullable<bool> isImport { get; set; }



    public virtual imp_items imp_items { get; set; }

}

}
