
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
    
public partial class aspnet_Applications
{

    public aspnet_Applications()
    {

        this.aspnet_Membership = new HashSet<aspnet_Membership>();

        this.aspnet_Paths = new HashSet<aspnet_Paths>();

        this.aspnet_Roles = new HashSet<aspnet_Roles>();

        this.aspnet_Users = new HashSet<aspnet_Users>();

    }


    public string ApplicationName { get; set; }

    public string LoweredApplicationName { get; set; }

    public System.Guid ApplicationId { get; set; }

    public string Description { get; set; }



    public virtual ICollection<aspnet_Membership> aspnet_Membership { get; set; }

    public virtual ICollection<aspnet_Paths> aspnet_Paths { get; set; }

    public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }

    public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }

}

}