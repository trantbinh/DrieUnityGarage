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
    
    public partial class CT_TRAGOP
    {
        public string CTTG_MaTG { get; set; }
        public int CTTG_Thang { get; set; }
        public Nullable<System.DateTime> CTTG_NgayTra { get; set; }
        public Nullable<decimal> CTTG_SoTienTra { get; set; }
        public Nullable<decimal> CTTG_SoTienConLai { get; set; }
        public string CTTG_MaPT { get; set; }
    
        public virtual TRAGOP TRAGOP { get; set; }
        public virtual PHIEUTHU PHIEUTHU { get; set; }
    }
}
