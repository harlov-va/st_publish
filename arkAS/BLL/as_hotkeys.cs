
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
    
public partial class as_hotkeys
{

    public int Id { get; set; }

    public Nullable<int> keyCode { get; set; }

    public Nullable<bool> isShift { get; set; }

    public Nullable<bool> isAlt { get; set; }

    public Nullable<bool> isCtrl { get; set; }

    public string url { get; set; }

    public string js { get; set; }

    public string roles { get; set; }

}

}