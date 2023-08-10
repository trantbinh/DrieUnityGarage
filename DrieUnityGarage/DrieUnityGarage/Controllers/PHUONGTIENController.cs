using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGarage.Models;

namespace DrieUnityGarage.Controllers
{
    public class PHUONGTIENController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: PHUONGTIEN
        public ActionResult LayDanhSachPhuongTien()
        {
            var pHUONGTIENs = db.PHUONGTIENs.Include(p => p.KHACHHANG);
            return View(pHUONGTIENs.ToList());
        }

        // GET: PHUONGTIEN/LayThongTinPhuongTien/5
        public ActionResult LayThongTinPhuongTien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHUONGTIEN pHUONGTIEN = db.PHUONGTIENs.Find(id);
            if (pHUONGTIEN == null)
            {
                return HttpNotFound();
            }
            return View(pHUONGTIEN);
        }
        public List<THONGTINKHACHHANG> LayDanhSachKhachHangDB()
        {
            var newlstKH = new List<THONGTINKHACHHANG>();
            var khDB = db.KHACHHANGs.ToList();
            int c = khDB.Count();
            for (int i = 0; i < c; i++)
            {

                newlstKH.Add(new THONGTINKHACHHANG(khDB[i].MaKH));
            }
            Session.Remove("lstKH");
            Session["lstKH"] = newlstKH;
            return newlstKH;
        }

        // GET: PHUONGTIEN/TaoThongTinPhuongTien
        public ActionResult TaoThongTinPhuongTien()
        {

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");
            return View();
        }

        // POST: PHUONGTIEN/TaoThongTinPhuongTien
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoThongTinPhuongTien([Bind(Include = "BienSoXe,SoMay,SoKhung,SoKM,LoaiXe,Model,MauXe,PT_MaKH")] PHUONGTIEN pHUONGTIEN, String lstMaKH)
        {
            if (ModelState.IsValid)
            {
                pHUONGTIEN.PT_MaKH = lstMaKH;
                db.PHUONGTIENs.Add(pHUONGTIEN);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhuongTien");
            }

            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", pHUONGTIEN.PT_MaKH);
            return View(pHUONGTIEN);
        }

        // GET: PHUONGTIEN/SuaThongTinPhuongTien/5
        public ActionResult SuaThongTinPhuongTien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHUONGTIEN pHUONGTIEN = db.PHUONGTIENs.Find(id);
            if (pHUONGTIEN == null)
            {
                return HttpNotFound();
            }
            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin", pHUONGTIEN.PT_MaKH);

            return View(pHUONGTIEN);
        }

        // POST: PHUONGTIEN/SuaThongTinPhuongTien/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinPhuongTien([Bind(Include = "BienSoXe,SoMay,SoKhung,SoKM,LoaiXe,Model,MauXe,PT_MaKH")] PHUONGTIEN pHUONGTIEN, String lstMaKH)
        {
            if (ModelState.IsValid)
            {
                pHUONGTIEN.PT_MaKH = lstMaKH;
                db.Entry(pHUONGTIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhuongTien");
            }
            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", pHUONGTIEN.PT_MaKH);
            return View(pHUONGTIEN);
        }

        // GET: PHUONGTIEN/XoaThongTinPhuongTien/5
        public ActionResult XoaThongTinPhuongTien(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHUONGTIEN pHUONGTIEN = db.PHUONGTIENs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá phương tiện này";
            }
            else
            {

                if (pHUONGTIEN == null)
                {
                    return HttpNotFound();
                }
            }
            return View(pHUONGTIEN);
        }

        // POST: PHUONGTIEN/XoaThongTinPhuongTien/5
        [HttpPost, ActionName("XoaThongTinPhuongTien")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check = KiemTraKhoaNgoaiHangHoa(id);
            if (check == true)
            {
                return XoaThongTinPhuongTien(id, 0);
            }
            else
            {

                PHUONGTIEN pHUONGTIEN = db.PHUONGTIENs.Find(id);
                db.PHUONGTIENs.Remove(pHUONGTIEN);
                db.SaveChanges();
            }
            return RedirectToAction("LayDanhSachPhuongTien");
        }
        public bool KiemTraKhoaNgoaiHangHoa(string id)
        {
            List<THONGTINTIEPNHAN> tn = db.THONGTINTIEPNHANs.Where(m => m.TN_BienSoXe.Equals(id)).ToList();
            List<BAOHANH> bg = db.BAOHANHs.Where(m => m.BH_BienSoXe.Equals(id)).ToList();
            List<BAOGIA> xk = db.BAOGIAs.Where(m => m.BG_BienSoXe.Equals(id)).ToList();
            List<HOADON> nk = db.HOADONs.Where(m => m.HD_BienSoXe.Equals(id)).ToList();
            List<DATLICH> dl = db.DATLICHes.Where(m => m.DL_BienSoXe.Equals(id)).ToList();

            if (tn.Count() == 0 && bg.Count() == 0 && xk.Count() == 0 && nk.Count() == 0 && dl.Count() == 0)
            {
                return false;
            }
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
    }
}
