
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
    
public partial class motskin_documents
{

    public motskin_documents()
    {

        this.motskin_documentStatusLog = new HashSet<motskin_documentStatusLog>();

    }


    public int id { get; set; }

    public System.DateTime date { get; set; }

    public string number { get; set; }

    public decimal sum { get; set; }

    public string comment { get; set; }

    public string reference { get; set; }

    public bool isDeleted { get; set; }

    public int contractorID { get; set; }

    public int documentTypeID { get; set; }

    public int documentStatusID { get; set; }

    public System.Guid createdUnique { get; set; }



    public virtual motskin_contractors motskin_contractors { get; set; }

    public virtual ICollection<motskin_documentStatusLog> motskin_documentStatusLog { get; set; }

    public virtual motskin_documentStatuses motskin_documentStatuses { get; set; }

    public virtual motskin_documentTypes motskin_documentTypes { get; set; }

}

}