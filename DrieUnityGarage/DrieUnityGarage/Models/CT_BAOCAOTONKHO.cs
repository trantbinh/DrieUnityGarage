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
    
    public partial class CT_BAOCAOTONKHO
    {
        public string CTBCTK_MaBCTK { get; set; }
        public string CTBCTK_MaHH { get; set; }
        public Nullable<int> CTBCTK_SoLuongTonKho { get; set; }
        public string CTBCTK_NhaCungCap { get; set; }
    
        public virtual HANGHOA HANGHOA { get; set; }
    }
}
