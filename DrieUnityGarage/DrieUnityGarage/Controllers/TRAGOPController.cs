using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGarage.Models;
using Newtonsoft.Json.Linq;

namespace DrieUnityGarage.Controllers
{
    public class TRAGOPController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        public ActionResult CTTG_LayDSThongTin(String id)
        {
            Session["MaTG"] = id;

            return View(db.CT_TRAGOP.Where(m=>m.CTTG_MaTG.Equals(id)).ToList());
        }





        // GET: TRAGOP/Create
        public ActionResult CTTG_TaoCT(String id)
        {
            var tg = db.TRAGOPs.Find(id);
            ViewBag.NgayTra = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            var phieuThu = db.PHIEUTHUs.ToList();
            var lstPhieuThu = new List<PHIEUTHU>();
            foreach (var item in phieuThu)
            {
                var tRAGOP = db.CT_TRAGOP.FirstOrDefault(m => m.CTTG_MaPT.Equals(item.MaPT));
                if (tRAGOP != null)
                    continue;
                else
                    lstPhieuThu.Add(item);
            }
            Session["MaTG"] = id;
            ViewBag.lstMaPT = new SelectList(lstPhieuThu, "MaPT", "MaPT");
            ViewBag.MaTG = id;
            return View();
        }

        // POST: TRAGOP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CTTG_TaoCT(String lstMaPT, DateTime NgayTra)
        {
            CT_TRAGOP cT_TRAGOP = new CT_TRAGOP();
            String maTG = Session["MaTG"].ToString();
            var phieuThu = db.PHIEUTHUs.Find(lstMaPT);
            var traGop = db.TRAGOPs.Find(maTG);
            decimal soTienTra = (decimal) phieuThu.SoTien;
            decimal soTienConLai = System.Math.Round((decimal)(traGop.SoTienTraGop - soTienTra), 2);
            if (ModelState.IsValid)
            {
                cT_TRAGOP.CTTG_NgayTra = NgayTra;
                cT_TRAGOP.CTTG_MaTG= maTG;
                cT_TRAGOP.CTTG_SoTienTra= soTienTra;
                cT_TRAGOP.CTTG_SoTienConLai= soTienConLai;
                cT_TRAGOP.CTTG_MaPT = lstMaPT;
                db.CT_TRAGOP.Add(cT_TRAGOP);
                db.SaveChanges();
                return RedirectToAction("CTTG_LayDSThongTin", new { id = Session["MaTG"].ToString() });
            }
            return View(cT_TRAGOP);
        }
        public ActionResult CTTG_XoaCT(String id, int t)
        {
            CT_TRAGOP tRAGOP = db.CT_TRAGOP.Find(id,t);
            db.CT_TRAGOP.Remove(tRAGOP);
            db.SaveChanges();
            return RedirectToAction("CTTG_LayDSThongTin", new { id = Session["MaTG"].ToString() });
        }

        // GET: TRAGOP
        public ActionResult Index()
        {
            return View(db.TRAGOPs.ToList());
        }

        // GET: TRAGOP/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAGOP tRAGOP = db.TRAGOPs.Find(id);
            if (tRAGOP == null)
            {
                return HttpNotFound();
            }
            String idNV = tRAGOP.TG_MaNV;
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            DateTime date = (DateTime)tRAGOP.NgayTraDinhKi;
            String dateFormat = date.ToString("yyyy/MM/dd");
            ViewBag.NgayTraDinhKi = dateFormat;


            return View(tRAGOP);
        }

        // GET: TRAGOP/Create
        public ActionResult Create()
        {
            var hoaDon = db.HOADONs.ToList();
            var lstHoaDon = new List<HOADON>();
            foreach (var item in hoaDon)
            {
                var tRAGOP = db.TRAGOPs.FirstOrDefault(m => m.TG_MaHD.Equals(item.MaHD));
                if (tRAGOP != null)
                    continue;
                else
                    lstHoaDon.Add(item);
            }

            ViewBag.lstMaHD = new SelectList(lstHoaDon, "MaHD", "MaHD");
            ViewBag.MaTG = TaoMaPhieuTraGop();
            String idNV = "NV001";
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");

            return View();
        }

        // POST: TRAGOP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaTG,NgayLap,TG_MaHD,TG_MaKH,SoTienTraGop,NgayTraDinhKi,TyLeTraGop,ThoiHanTra,SoTienDaTra, TG_MaNV")] TRAGOP tRAGOP, String lstMaHD)
        {
            var hoaDon = db.HOADONs.Find(lstMaHD);
            decimal soTienTra = (decimal) hoaDon.TongThanhToan * (decimal) tRAGOP.TyLeTraGop;
            decimal tienHangThang = (decimal)soTienTra / (decimal)tRAGOP.ThoiHanTra;
            decimal value = (decimal)System.Math.Round(tienHangThang, 2);
            if (ModelState.IsValid)
            {
                tRAGOP.NgayLap = DateTime.Now;
                tRAGOP.MaTG = TaoMaPhieuTraGop();
                tRAGOP.TG_MaHD = lstMaHD;
                tRAGOP.TG_MaKH = hoaDon.HD_MaKH;
                tRAGOP.TG_MaNV = "NV001";
                tRAGOP.SoTienTraGop= soTienTra;
                tRAGOP.SoTienTraHangThang= value;
                tRAGOP.SoTienDaTra = 0;
                db.TRAGOPs.Add(tRAGOP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tRAGOP);
        }

        // GET: TRAGOP/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAGOP tRAGOP = db.TRAGOPs.Find(id);
            if (tRAGOP == null)
            {
                return HttpNotFound();
            }
            var hoaDon = db.HOADONs.ToList();
            var lstHoaDon = new List<HOADON>();
            foreach (var item in hoaDon)
            {
                var traGopNew = db.TRAGOPs.FirstOrDefault(m => m.TG_MaHD.Equals(item.MaHD));
                if (traGopNew != null && traGopNew.TG_MaHD!= tRAGOP.TG_MaHD)
                    continue;
                else
                    lstHoaDon.Add(item);
            }

            ViewBag.lstMaHD = new SelectList(lstHoaDon, "MaHD", "MaHD", tRAGOP.TG_MaHD);
            String idNV = tRAGOP.TG_MaNV;
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            DateTime date = (DateTime)tRAGOP.NgayTraDinhKi;
            String dateFormat = date.ToString("yyyy/MM/dd");
            ViewBag.NgayTraDinhKi = dateFormat;

            Session["MaNV"] = idNV;
            return View(tRAGOP);
        }

        // POST: TRAGOP/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTG,NgayLap,TG_MaHD,TG_MaKH,SoTienTraGop,NgayTraDinhKi,TyLeTraGop,ThoiHanTra,SoTienDaTra")] TRAGOP tRAGOP, String lstMaHD)
        {
            var hoaDon = db.HOADONs.Find(lstMaHD);
            decimal soTienTra = (decimal)hoaDon.TongThanhToan * (decimal)tRAGOP.TyLeTraGop;
            decimal tienHangThang = (decimal)soTienTra / (decimal)tRAGOP.ThoiHanTra;
            decimal value = (decimal)System.Math.Round(tienHangThang, 2);

            if (ModelState.IsValid)
            {
                tRAGOP.TG_MaHD = lstMaHD;
                tRAGOP.TG_MaKH = hoaDon.HD_MaKH;
                tRAGOP.SoTienTraGop = soTienTra;
                tRAGOP.SoTienTraHangThang = value;
                tRAGOP.SoTienDaTra = 0;
                tRAGOP.TG_MaNV = Session["MaNV"].ToString();
                db.Entry(tRAGOP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tRAGOP);
        }

        // GET: TRAGOP/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAGOP tRAGOP = db.TRAGOPs.Find(id);
            if (tRAGOP == null)
            {
                return HttpNotFound();
            }
            String idNV = tRAGOP.TG_MaNV;
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            DateTime date = (DateTime)tRAGOP.NgayTraDinhKi;
            String dateFormat = date.ToString("yyyy/MM/dd");
            ViewBag.NgayTraDinhKi = dateFormat;

            return View(tRAGOP);
        }

        // POST: TRAGOP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check;
            List<CT_TRAGOP> cthd = db.CT_TRAGOP.Where(m => m.CTTG_MaTG.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietTG(cthd[i].CTTG_MaTG);
                }
            }
            TRAGOP tRAGOP = db.TRAGOPs.Find(id);
            db.TRAGOPs.Remove(tRAGOP);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public bool XoaChiTietTG(string id)
        {
            CT_TRAGOP cT_TRAGOP = db.CT_TRAGOP.FirstOrDefault(m => m.CTTG_MaTG.Equals(id));
            db.CT_TRAGOP.Remove(cT_TRAGOP);
            db.SaveChanges();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private String TaoMaPhieuTraGop()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<TRAGOP> lstHD = db.TRAGOPs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "TG001";
            }
            else
            {
                TRAGOP lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaTG;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "TG00" + newMaHD.ToString();
                }
                else { idHD = "TG0" + newMaHD.ToString(); }
            }
            return (idHD);
        }



    }
}
