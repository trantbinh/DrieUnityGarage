//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrieUnityGarage.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CT_NHAPKHO
    {
        public string CTNK_MaHH { get; set; }
        public string CTNK_MaNK { get; set; }
        public Nullable<int> SoLuongYeuCau { get; set; }
        public Nullable<int> SoLuongThucNhap { get; set; }
    
        public virtual HANGHOA HANGHOA { get; set; }
        public virtual NHAPKHO NHAPKHO { get; set; }
    }
}