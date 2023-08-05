using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class THONGTINTIEPNHANXE
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        private String maTN;
        private String maNV;
        private String maKH;
        private String bienSoXe;
        private DateTime thoiGianTiepNhan;
        private DateTime thoiGianDuKien;
        private String ghiChuKH;
        private String trangThai;
        private String fullThongTin;

        public string MaTN { get => maTN; set => maTN = value; }
        public string MaNV { get => maNV; set => maNV = value; }
        public string MaKH { get => maKH; set => maKH = value; }
        public string BienSoXe { get => bienSoXe; set => bienSoXe = value; }
        public DateTime ThoiGianTiepNhan { get => thoiGianTiepNhan; set => thoiGianTiepNhan = value; }
        public DateTime ThoiGianDuKien { get => thoiGianDuKien; set => thoiGianDuKien = value; }
        public string GhiChuKH { get => ghiChuKH; set => ghiChuKH = value; }
        public string TrangThai { get => trangThai; set => trangThai = value; }
        public string FullThongTin { get => fullThongTin; set => fullThongTin = value; }

        public THONGTINTIEPNHANXE(String newMaTN)
        {
            this.MaTN = newMaTN;

            var tn = db.THONGTINTIEPNHANs.Single(s => s.MaTN.Equals(newMaTN));

            this.MaNV = tn.TN_MaNV;

            this.MaKH = tn.TN_MaKH;
            this.BienSoXe = tn.TN_BienSoXe;
            this.ThoiGianTiepNhan = (DateTime)tn.ThoiGianTiepNhan;
            if(tn.ThoiGianGiaoDuKien== null)
            {
                ThoiGianDuKien = DateTime.Now;
            }
            else { this.ThoiGianDuKien = (DateTime)tn.ThoiGianGiaoDuKien; }
            this.GhiChuKH = tn.GhiChuKH;
            this.TrangThai= tn.TrangThai;
            this.FullThongTin = tn.MaTN + " - " + tn.TN_MaKH + " - " + tn.TN_BienSoXe;
        }
    }
}