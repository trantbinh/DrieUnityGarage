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
    
    public partial class NHAPKHO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NHAPKHO()
        {
            this.CT_NHAPKHO = new HashSet<CT_NHAPKHO>();
        }
    
        public string MaNK { get; set; }
        public Nullable<System.DateTime> NgayLap { get; set; }
        public string NK_MaNCC { get; set; }
        public string HoTenNguoiGiao { get; set; }
        public string SoChungTu { get; set; }
        public string NK_MaNV { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_NHAPKHO> CT_NHAPKHO { get; set; }
        public virtual NHACUNGCAP NHACUNGCAP { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
    }
}
