
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
    
public partial class calc_calcs
{

    public calc_calcs()
    {

        this.calc_parameters = new HashSet<calc_parameters>();

    }


    public int id { get; set; }

    public string name { get; set; }

    public string description { get; set; }

    public string makeup { get; set; }

    public string resultFunction { get; set; }

    public string calcFunction { get; set; }

    public string code { get; set; }



    public virtual ICollection<calc_parameters> calc_parameters { get; set; }

}

}
