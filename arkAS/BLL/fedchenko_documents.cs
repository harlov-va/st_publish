
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
    
public partial class fedchenko_documents
{

    public int Id { get; set; }

    public string Number { get; set; }

    public Nullable<int> CounterpartyId { get; set; }

    public Nullable<decimal> ContractSum { get; set; }

    public string Comment { get; set; }

    public Nullable<int> StatusId { get; set; }

    public string Url { get; set; }



    public virtual fedchenko_counterparty fedchenko_counterparty { get; set; }

    public virtual fedchenko_docStatus fedchenko_docStatus { get; set; }

}

}