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
    
    public partial class KIEMKEKHOHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KIEMKEKHOHANG()
        {
            this.CT_KIEMKEKHOHANG = new HashSet<CT_KIEMKEKHOHANG>();
        }
    
        public string MaKK { get; set; }
        public Nullable<System.DateTime> ThoiDiemKiemKe { get; set; }
        public string KK_MaNV { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_KIEMKEKHOHANG> CT_KIEMKEKHOHANG { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
    }
}
