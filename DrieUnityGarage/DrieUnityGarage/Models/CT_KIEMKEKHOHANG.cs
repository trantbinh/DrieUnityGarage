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
    
    public partial class CT_KIEMKEKHOHANG
    {
        public string CTKK_MaKK { get; set; }
        public string CTKK_MaHH { get; set; }
        public Nullable<int> CTKK_SoLuongSoSach { get; set; }
        public Nullable<decimal> CTKK_ThanhTienSoSach { get; set; }
        public Nullable<int> CTKK_SoLuongKiemKe { get; set; }
        public Nullable<decimal> CTKK_ThanhTienKiemKe { get; set; }
    
        public virtual HANGHOA HANGHOA { get; set; }
        public virtual KIEMKEKHOHANG KIEMKEKHOHANG { get; set; }
    }
}