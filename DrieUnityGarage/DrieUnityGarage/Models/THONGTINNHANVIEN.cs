using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class THONGTINNHANVIEN
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        private String maNV;
        private String hoTenNV;
        private String dienThoaiNV;
        private DateTime ngaySinh;
        private String gioiTinh;
        private String email;
        private String diaChi;
        private String chucVu;
        private String phongBan;
        private String tenDangNhap;
        private String matKhau;
        private DateTime ngayTaoTK;
        private String thongTin;

        public string MaNV { get => maNV; set => maNV = value; }
        public string HoTenNV { get => hoTenNV; set => hoTenNV = value; }
        public string DienThoaiNV { get => dienThoaiNV; set => dienThoaiNV = value; }
        public DateTime NgaySinh { get => ngaySinh; set => ngaySinh = value; }
        public string GioiTinh { get => gioiTinh; set => gioiTinh = value; }
        public string Email { get => email; set => email = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public string ChucVu { get => chucVu; set => chucVu = value; }
        public string PhongBan { get => phongBan; set => phongBan = value; }
        public string TenDangNhap { get => tenDangNhap; set => tenDangNhap = value; }
        public string MatKhau { get => matKhau; set => matKhau = value; }
        public DateTime NgayTaoTK { get => ngayTaoTK; set => ngayTaoTK = value; }
        public string ThongTin { get => thongTin; set => thongTin = value; }

        public THONGTINNHANVIEN(String id)
        {
            MaNV = id;
            var nv = db.NHANVIENs.Single(s => s.MaNV.Equals(id));
            HoTenNV = nv.HoTenNV;
            DienThoaiNV = nv.DienThoaiNV;
            NgaySinh = (DateTime)nv.NgaySinh;
            GioiTinh = nv.GioiTinh;
            Email = nv.Email;
            DiaChi = nv.DiaChi;
            ChucVu = nv.ChucVu;
            PhongBan = nv.PhongBan;
            TenDangNhap = nv.TenDangNhap;
            MatKhau = nv.MatKhau;
            if (nv.NgayTaoTK == null)
            {
                NgayTaoTK = DateTime.Now;
            }
            else
            {
                NgayTaoTK = (DateTime)nv.NgayTaoTK;
            }

            ThongTin = id + " - " + HoTenNV + " - " + ChucVu;
        }

    }
}