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
    
    public partial class DANHGIATINHTRANG
    {
        public string MaDG { get; set; }
        public Nullable<System.DateTime> NgayLap { get; set; }
        public string DG_MaTN { get; set; }
        public Nullable<int> NgoaiThat { get; set; }
        public Nullable<int> KinhChanGio { get; set; }
        public Nullable<int> GatMua { get; set; }
        public Nullable<int> HeThongDen { get; set; }
        public Nullable<int> HeThongDieuHoa { get; set; }
        public Nullable<int> DauDongCo { get; set; }
        public Nullable<int> DauPhanh { get; set; }
        public Nullable<int> DauTroLucLai { get; set; }
        public Nullable<int> NuocRuaKinh { get; set; }
        public Nullable<int> NuocLamMat { get; set; }
        public Nullable<int> LocGioDongCo { get; set; }
        public Nullable<int> LocGioDieuHoa { get; set; }
        public Nullable<int> Acquy { get; set; }
        public Nullable<int> HeThongSac { get; set; }
        public Nullable<int> DauCap { get; set; }
        public Nullable<int> MaPhanh { get; set; }
        public Nullable<int> DiaPhanh { get; set; }
        public Nullable<int> HeThongLai { get; set; }
        public Nullable<int> HeThongTreo { get; set; }
        public Nullable<int> BanhTraiTruoc { get; set; }
        public Nullable<int> BanhTraiSau { get; set; }
        public Nullable<int> BanhPhaiTruoc { get; set; }
        public Nullable<int> BanhPhaiSau { get; set; }
        public Nullable<int> CanChinhThuocLai { get; set; }
        public Nullable<int> CanBangDong { get; set; }
        public Nullable<int> DaoLop { get; set; }
        public Nullable<int> ThayThe { get; set; }
        public string GhiChu { get; set; }
    
        public virtual THONGTINTIEPNHAN THONGTINTIEPNHAN { get; set; }
    }
}