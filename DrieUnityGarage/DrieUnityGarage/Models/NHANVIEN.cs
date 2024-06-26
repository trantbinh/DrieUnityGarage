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
    
    public partial class NHANVIEN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NHANVIEN()
        {
            this.BAOCAOTONKHOes = new HashSet<BAOCAOTONKHO>();
            this.KIEMKEKHOHANGs = new HashSet<KIEMKEKHOHANG>();
            this.NHAPKHOes = new HashSet<NHAPKHO>();
            this.PHIEUTHUs = new HashSet<PHIEUTHU>();
            this.THONGTINTIEPNHANs = new HashSet<THONGTINTIEPNHAN>();
            this.XUATKHOes = new HashSet<XUATKHO>();
            this.TRAGOPs = new HashSet<TRAGOP>();
        }
    
        public string MaNV { get; set; }
        public string HoTenNV { get; set; }
        public string DienThoaiNV { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string ChucVu { get; set; }
        public string PhongBan { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public Nullable<System.DateTime> NgayTaoTK { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BAOCAOTONKHO> BAOCAOTONKHOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KIEMKEKHOHANG> KIEMKEKHOHANGs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NHAPKHO> NHAPKHOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PHIEUTHU> PHIEUTHUs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<THONGTINTIEPNHAN> THONGTINTIEPNHANs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<XUATKHO> XUATKHOes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRAGOP> TRAGOPs { get; set; }
    }
}
