
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
    
public partial class Mikhailova_mails
{

    public int id { get; set; }

    public System.DateTime date { get; set; }

    public string from { get; set; }

    public string to { get; set; }

    public string desc { get; set; }

    public string systemMail { get; set; }

    public string treckNumber { get; set; }

    public Nullable<int> statusID { get; set; }

    public string treckNumberReplay { get; set; }

    public System.DateTime dateReplay { get; set; }

    public Nullable<bool> isDeleted { get; set; }



    public virtual Mikhailova_statuses_mails Mikhailova_statuses_mails { get; set; }

}

}