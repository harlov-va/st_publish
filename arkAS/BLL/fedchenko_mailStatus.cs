
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
    
public partial class fedchenko_mailStatus
{

    public fedchenko_mailStatus()
    {

        this.fedchenko_mail = new HashSet<fedchenko_mail>();

    }


    public int Id { get; set; }

    public string Name { get; set; }



    public virtual ICollection<fedchenko_mail> fedchenko_mail { get; set; }

}

}