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
    public class PHIEUTHUController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
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

        // GET: PHIEUTHU
        public ActionResult LayDanhSachPhieuThu()
        {
            var pHIEUTHUs = db.PHIEUTHUs.Include(p => p.KHACHHANG).Include(p => p.NHANVIEN);
            return View(pHIEUTHUs.ToList());
        }

        // GET: PHIEUTHU/LayThongTinPhieuThu/5
        public ActionResult LayThongTinPhieuThu(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHIEUTHU pHIEUTHU = db.PHIEUTHUs.Find(id);
            if (pHIEUTHU == null)
            {
                return HttpNotFound();
            }
            String idNV = pHIEUTHU.PT_MaNV;
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            String tenKH = db.KHACHHANGs.Find(pHIEUTHU.PT_MaKH).HoTenKH; ;
            String kh = pHIEUTHU.PT_MaKH + " - " + tenKH;
            ViewBag.KhachHang = kh;

            return View(pHIEUTHU);
        }

        // GET: PHIEUTHU/TaoPhieuThu
        public ActionResult TaoPhieuThu()
        {
            ViewBag.MaPT = TaoMaPhieuThu();
            String idNV = Session["MaTaiKhoanNV"].ToString();
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            ViewBag.DiaChi = "DrieUnityGrage - 828 Sư Vạn Hạnh, phường 12, quận 10, TPHCM";

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");
                return View();
        }

        // POST: PHIEUTHU/TaoPhieuThu
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoPhieuThu([Bind(Include = "MaPT,NgayLap,DiaChi,LyDoNop,SoTien,ChungTuGoc,PT_MaKH,PT_MaNV")] PHIEUTHU pHIEUTHU, String lstMaKH)
        {
            if (ModelState.IsValid)
            {
                pHIEUTHU.MaPT = TaoMaPhieuThu();
                pHIEUTHU.NgayLap = DateTime.Now;
                pHIEUTHU.PT_MaNV = Session["MaTaiKhoanNV"].ToString();
                pHIEUTHU.DiaChi = "DrieUnityGrage - 828 Sư Vạn Hạnh, phường 12, quận 10, TPHCM";
                pHIEUTHU.PT_MaKH = lstMaKH;
                db.PHIEUTHUs.Add(pHIEUTHU);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhieuThu");
            }

            ViewBag.MaPT = TaoMaPhieuThu();
            String idNV = Session["MaTaiKhoanNV"].ToString();
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            ViewBag.DiaChi = "DrieUnityGrage - 828 Sư Vạn Hạnh, phường 12, quận 10, TPHCM";
            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin", pHIEUTHU.PT_MaKH);
            return View(pHIEUTHU);
        }

        // GET: PHIEUTHU/SuaPhieuThu/5
        public ActionResult SuaPhieuThu(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHIEUTHU pHIEUTHU = db.PHIEUTHUs.Find(id);
            if (pHIEUTHU == null)
            {
                return HttpNotFound();
            }
            String idNV = pHIEUTHU.PT_MaNV;
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");
            Session["MaPT"] = id;
            return View(pHIEUTHU);
        }

        // POST: PHIEUTHU/SuaPhieuThu/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaPhieuThu([Bind(Include = "MaPT,NgayLap,DiaChi,LyDoNop,SoTien,ChungTuGoc,PT_MaKH,PT_MaNV")] PHIEUTHU pHIEUTHU, String lstMaKH)
        {
            if (ModelState.IsValid)
            {
                pHIEUTHU.PT_MaKH = lstMaKH;
                db.Entry(pHIEUTHU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhieuThu");
            }
            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", pHIEUTHU.PT_MaKH);
            ViewBag.PT_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", pHIEUTHU.PT_MaNV);
            return View(pHIEUTHU);
        }

        // GET: PHIEUTHU/XoaPhieuThu/5
        public ActionResult XoaPhieuThu(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PHIEUTHU pHIEUTHU = db.PHIEUTHUs.Find(id);
            if (pHIEUTHU == null)
            {
                return HttpNotFound();
            }
            String idNV = pHIEUTHU.PT_MaNV;
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            String tenKH = db.KHACHHANGs.Find(pHIEUTHU.PT_MaKH).HoTenKH; ;
            String kh = pHIEUTHU.PT_MaKH+ " - " + tenKH;
            ViewBag.KhachHang = kh;

            return View(pHIEUTHU);
        }

        // POST: PHIEUTHU/XoaPhieuThu/5
        [HttpPost, ActionName("XoaPhieuThu")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PHIEUTHU pHIEUTHU = db.PHIEUTHUs.Find(id);
            db.PHIEUTHUs.Remove(pHIEUTHU);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachPhieuThu");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private String TaoMaPhieuThu()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<PHIEUTHU> lstHD = db.PHIEUTHUs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "PT001";
            }
            else
            {
                PHIEUTHU lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaPT;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "PT00" + newMaHD.ToString();
                }
                else { idHD = "PT0" + newMaHD.ToString(); }
            }
            return (idHD);
        }


    }
}
