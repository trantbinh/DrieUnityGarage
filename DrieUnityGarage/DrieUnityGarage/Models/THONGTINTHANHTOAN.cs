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
    
    public partial class THONGTINTHANHTOAN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public THONGTINTHANHTOAN()
        {
            this.HOADONs = new HashSet<HOADON>();
        }
    
        public string MaTT { get; set; }
        public Nullable<System.DateTime> NgayTT { get; set; }
        public string TT_MaKH { get; set; }
        public string TT_MaHD { get; set; }
        public Nullable<decimal> TongThanhToan { get; set; }
        public string HinhThuc { get; set; }
        public Nullable<double> TyLeThanhToan { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOADON> HOADONs { get; set; }
    }
}