using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class THONGTINNHACUNGCAP
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        private String maNCC;
        private String tenNCC;
        private String diaChiNCC;
        private String dienThoaiNCC;
        private String maSoThue;
        private String email;
        private String loaiHinh;
        private String hoTenNguoiDaiDien;
        private String thongTin;

        public string MaNCC { get => maNCC; set => maNCC = value; }
        public string TenNCC { get => tenNCC; set => tenNCC = value; }
        public string DiaChiNCC { get => diaChiNCC; set => diaChiNCC = value; }
        public string DienThoaiNCC { get => dienThoaiNCC; set => dienThoaiNCC = value; }
        public string MaSoThue { get => maSoThue; set => maSoThue = value; }
        public string Email { get => email; set => email = value; }
        public string LoaiHinh { get => loaiHinh; set => loaiHinh = value; }
        public string HoTenNguoiDaiDien { get => hoTenNguoiDaiDien; set => hoTenNguoiDaiDien = value; }
        public string ThongTin { get => thongTin; set => thongTin = value; }

        public THONGTINNHACUNGCAP(String id)
        {
            MaNCC = id;
            var ncc = db.NHACUNGCAPs.Single(s => s.MaNCC.Equals(id));
            TenNCC = ncc.TenNCC;
            DiaChiNCC = ncc.DiaChiNCC;
            DienThoaiNCC= ncc.DienThoaiNCC;
            MaSoThue = ncc.MaSoThueNCC;
            Email=ncc.Email;
            LoaiHinh = ncc.LoaiHinh;
            HoTenNguoiDaiDien= ncc.HoTenNguoiDaiDien;
            ThongTin = id + " - " + TenNCC;
        }

    }
}