
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
    
public partial class molchanov_mails
{

    public molchanov_mails()
    {

        this.molchanov_logMails = new HashSet<molchanov_logMails>();

    }


    public int id { get; set; }

    public System.Guid uniqueCode { get; set; }

    public System.DateTime date { get; set; }

    public string fromSender { get; set; }

    public string toRecipient { get; set; }

    public string description { get; set; }

    public string trackNumber { get; set; }

    public string backTrackNumber { get; set; }

    public Nullable<System.DateTime> backDateReceipt { get; set; }

    public int deliverySystemID { get; set; }

    public int mailStatusID { get; set; }



    public virtual molchanov_deliverySystems molchanov_deliverySystems { get; set; }

    public virtual molchanov_mailStatuses molchanov_mailStatuses { get; set; }

    public virtual ICollection<molchanov_logMails> molchanov_logMails { get; set; }

}

}
