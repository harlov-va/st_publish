
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
    
public partial class motskin_mails
{

    public motskin_mails()
    {

        this.motskin_mailStatusLog = new HashSet<motskin_mailStatusLog>();

    }


    public int id { get; set; }

    public System.DateTime date { get; set; }

    public string fromAddress { get; set; }

    public string toAddress { get; set; }

    public string description { get; set; }

    public string trackNumber { get; set; }

    public string backTrackNumber { get; set; }

    public Nullable<System.DateTime> backDateReceived { get; set; }

    public bool isDeleted { get; set; }

    public int sendSystemID { get; set; }

    public int mailStatusID { get; set; }

    public System.Guid createdUnique { get; set; }



    public virtual ICollection<motskin_mailStatusLog> motskin_mailStatusLog { get; set; }

    public virtual motskin_mailStatuses motskin_mailStatuses { get; set; }

    public virtual motskin_sendSystems motskin_sendSystems { get; set; }

}

}