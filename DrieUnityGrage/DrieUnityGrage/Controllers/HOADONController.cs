using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGrage.Models;

namespace DrieUnityGrage.Controllers
{
    public class HOADONController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: HOADON
        public ActionResult LayDanhSachHoaDon()
        {
            var hOADONs = db.HOADONs.Include(h => h.KHACHHANG).Include(h => h.PHUONGTIEN).Include(h => h.THONGTINTHANHTOAN).Include(h => h.THONGTINTIEPNHAN);
            return View(hOADONs.ToList());
        }

        // GET: HOADON/Details/5
        public ActionResult LayThongTinHoaDon(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADONs.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            return View(hOADON);
        }

        // GET: HOADON/Create
        public ActionResult ThemHoaDon()
        {
            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH");
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "BienSoXe");
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH");
            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN");
            return View();
        }
        public ActionResult TaoMaHD()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<HOADON> lstHD = db.HOADONs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "HD01";
            }
            else
            {
                HOADON lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaHD;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(3));
                int newMaHD = lastMaHDNum + 1;
                idHD = "HD0" + newMaHD.ToString();
            }
            return PartialView(idHD);
        }




        //Autofill fields
        public ActionResult LayThongTinTiepNhan(string id)
        {
            THONGTINTIEPNHAN thongtinTN = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(id));
            KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(thongtinTN.TN_MaKH));
            PHUONGTIEN pt = db.PHUONGTIENs.FirstOrDefault(m => m.BienSoXe == thongtinTN.TN_BienSoXe);
            NHANVIEN nv = db.NHANVIENs.FirstOrDefault(m => m.MaNV == thongtinTN.TN_MaNV);
            List<string> lstThongTin = new List<string>();
            lstThongTin.Add(kh.MaKH);
            lstThongTin.Add(kh.HoTenKH);
            // lstThongTin.Add(kh.Email);
           // lstThongTin.Add(kh.DienThoaiKH);
            lstThongTin.Add(nv.HoTenNV);
            lstThongTin.Add(pt.BienSoXe);
            /*lstThongTin.Add(pt.SoMay);
            lstThongTin.Add(pt.SoKhung);
            lstThongTin.Add(pt.SoKM.ToString());
            lstThongTin.Add(pt.MauXe);
            lstThongTin.Add(pt.LoaiXe);
            lstThongTin.Add(pt.Model);*/

            return PartialView(lstThongTin);
        }









        // POST: HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                db.HOADONs.Add(hOADON);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", hOADON.HD_MaKH);
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "BienSoXe", hOADON.HD_BienSoXe);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH", hOADON.HD_MaTT);
            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", hOADON.HD_MaTN);
            return View(hOADON);
        }

        // GET: HOADON/Edit/5
        public ActionResult SuaHoaDon(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADONs.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", hOADON.HD_MaKH);
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "SoMay", hOADON.HD_BienSoXe);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH", hOADON.HD_MaTT);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", hOADON.HD_MaTT);
            return View(hOADON);
        }

        // POST: HOADON/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHoaDon");
            }
            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", hOADON.HD_MaKH);
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "SoMay", hOADON.HD_BienSoXe);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH", hOADON.HD_MaTT);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", hOADON.HD_MaTT);
            return View(hOADON);
        }

        // GET: HOADON/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADONs.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            return View(hOADON);
        }

        // POST: HOADON/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HOADON hOADON = db.HOADONs.Find(id);
            db.HOADONs.Remove(hOADON);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachHoaDon");
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
