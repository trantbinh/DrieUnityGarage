using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrieUnityGarage.Models
{
    public class TIEPNHAN
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        public String MaTN { get; set; }
        public String MaNV { get; set; }
        public String MaKH { get; set; }
        public String BienSoXe { get; set; }
        public DateTime ThoiGianTiepNhan { get; set; }
        public DateTime ThoiGianDuKien { get; set; }
        public String GhiChu{get;set; }
        public String SelectedTN { get; set; }

        public TIEPNHAN(String newMaTN)
        {
            this.MaTN = newMaTN;

            var tn = db.THONGTINTIEPNHANs.Single(s => s.MaTN.Equals(newMaTN));

            this.MaNV = tn.TN_MaNV;

            this.MaKH = tn.TN_MaKH;
            this.BienSoXe = tn.TN_BienSoXe;
            this.ThoiGianTiepNhan = (DateTime)tn.ThoiGianTiepNhan;
            this.ThoiGianDuKien = (DateTime)tn.ThoiGianGiaoDuKien;
            this.GhiChu = tn.GhiChuKH;

        }
    }
}