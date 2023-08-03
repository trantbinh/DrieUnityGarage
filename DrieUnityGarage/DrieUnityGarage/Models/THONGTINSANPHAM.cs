using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class THONGTINSANPHAM
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        private String maSP;
        private String tenSP;
        private int soLuong;
        private decimal donGia;
        private String donViTinh;
        private String hinhAnh;
        private int tonKho;
        public string MaSP { get => maSP; set => maSP = value; }
        public string TenSP { get => tenSP; set => tenSP = value; }
        public int SoLuong { get => soLuong; set => soLuong = value; }
        public decimal DonGia { get => donGia; set => donGia = value; }
        public string DonViTinh { get => donViTinh; set => donViTinh = value; }
        public string HinhAnh { get => hinhAnh; set => hinhAnh = value; }
        public int TonKho { get => tonKho; set => tonKho = value; }

        public decimal FinalPrice()
        {
            return SoLuong * DonGia;
        }
        public THONGTINSANPHAM(String newMaSP)
        {
            MaSP = newMaSP;
            var sp = db.HANGHOAs.Single(s => s.MaHH.Equals(newMaSP));
            TenSP = sp.TenHH;
            DonViTinh = sp.DonViTinh;
            DonGia = sp.DonGia;
            SoLuong = 1;
            HinhAnh = sp.HinhAnh;
            TonKho =(int) sp.SoLuongTon;
        }
        public THONGTINSANPHAM(String newMaSP, int? sl)
        {
            MaSP = newMaSP;
            var sp = db.HANGHOAs.Single(s => s.MaHH.Equals(newMaSP));
            TenSP = sp.TenHH;
            DonViTinh = sp.DonViTinh;
            DonGia = sp.DonGia;
            SoLuong =(int) sl;
            HinhAnh = sp.HinhAnh;
            TonKho = (int)sp.SoLuongTon;
        }


    }
}