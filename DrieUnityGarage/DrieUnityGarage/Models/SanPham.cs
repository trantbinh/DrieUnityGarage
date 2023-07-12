using DrieUnityGarage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class SanPham
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        public String MaSP { get; set; }
        public String TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public String DonViTinh { get; set; }

        public decimal FinalPrice()
        {
            return SoLuong * DonGia;
        }
        public SanPham(String newMaSP)
        {
            this.MaSP = newMaSP;
            var sp = db.HANGHOAs.Single(s => s.MaHH.Equals(newMaSP));
            this.TenSP = sp.TenHH;
            this.DonViTinh = sp.DonViTinh;
            this.DonGia = sp.DonGia;
            this.SoLuong = 1;

        }

    }
}