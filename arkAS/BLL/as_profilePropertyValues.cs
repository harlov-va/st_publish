
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
    
public partial class as_profilePropertyValues
{

    public int id { get; set; }

    public Nullable<int> propertyID { get; set; }

    public string value { get; set; }

    public Nullable<System.Guid> userGuid { get; set; }



    public virtual as_profileProperties as_profileProperties { get; set; }

}

}
