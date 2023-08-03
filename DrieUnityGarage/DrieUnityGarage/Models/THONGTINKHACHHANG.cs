using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class THONGTINKHACHHANG
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        private String maKH;
        private String hoTenKH;
        private String dienThoaiKH;
        private DateTime ngaySinh;
        private String gioiTinh;
        private String email;
        private int diemThanhVien;
        private String diaChi;
        private String thongTin;

        public string MaKH { get => maKH; set => maKH = value; }
        public string HoTenKH { get => hoTenKH; set => hoTenKH = value; }
        public string DienThoaiKH { get => dienThoaiKH; set => dienThoaiKH = value; }
        public DateTime NgaySinh { get => ngaySinh; set => ngaySinh = value; }
        public string GioiTinh { get => gioiTinh; set => gioiTinh = value; }
        public string Email { get => email; set => email = value; }
        public int DiemThanhVien { get => diemThanhVien; set => diemThanhVien = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public string ThongTin { get => thongTin; set => thongTin = value; }

        public THONGTINKHACHHANG(String id)
        {
            MaKH = id;
            var kh = db.KHACHHANGs.Single(s=>s.MaKH.Equals(id));
            HoTenKH = kh.HoTenKH;
            DienThoaiKH = kh.DienThoaiKH;
            NgaySinh = (DateTime)kh.NgaySinh;
            GioiTinh = kh.GioiTinh;
            Email = kh.Email;
            if (kh.DiemThanhVien == null)
                DiemThanhVien = 0;
            else
            DiemThanhVien = (int)kh.DiemThanhVien;
            DiaChi = kh.DiaChi;
            ThongTin = id + " - " + DienThoaiKH +" - "+HoTenKH;
        }
    }
}