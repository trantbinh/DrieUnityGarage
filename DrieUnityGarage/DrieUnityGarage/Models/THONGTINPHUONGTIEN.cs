using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class THONGTINPHUONGTIEN
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        private String bienSoXe;
        private String soMay;
        private String soKhung;
        private int soKM;
        private String loaiXe;
        private String model;
        private String mauXe;
        private String tTPT_MaKH;

        public string BienSoXe { get => bienSoXe; set => bienSoXe = value; }
        public string SoMay { get => soMay; set => soMay = value; }
        public string SoKhung { get => soKhung; set => soKhung = value; }
        public int SoKM { get => soKM; set => soKM = value; }
        public string LoaiXe { get => loaiXe; set => loaiXe = value; }
        public string Model { get => model; set => model = value; }
        public string MauXe { get => mauXe; set => mauXe = value; }
        public string TTPT_MaKH { get => tTPT_MaKH; set => tTPT_MaKH = value; }
        public THONGTINPHUONGTIEN(String id)
        {
            this.BienSoXe = id;
            var xe= db.PHUONGTIENs.Single(s=>s.BienSoXe.Equals(id));
            this.SoMay = xe.SoMay;
            this.SoKhung = xe.SoKhung;
            this.SoKM = (int) xe.SoKM;
            this.LoaiXe = xe.LoaiXe;
            this.Model = xe.Model;
            this.MauXe = xe.MauXe;
            this.TTPT_MaKH = xe.PT_MaKH;
        }
    }
}