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
    public class HANGHOAController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: HANGHOA
        public ActionResult LayDanhSachHangHoa()
        {
            var hANGHOAs = db.HANGHOAs.Include(h => h.NHACUNGCAP);
            return View(hANGHOAs.ToList());
        }

        // GET: HANGHOA/LayThongTinHangHoa/5
        public ActionResult LayThongTinHangHoa(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            if (hANGHOA == null)
            {
                return HttpNotFound();
            }
            return View(hANGHOA);
        }

        // GET: HANGHOA/ThemHangHoa
        public ActionResult ThemHangHoa()
        {
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC");
            return View();
        }

        // POST: HANGHOA/ThemHangHoa
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more LayThongTinHangHoa see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemHangHoa([Bind(Include = "MaHH,TenHH,DonGia,DonViTinh,LoaiHang,SoLuongTon,HH_MaNCC")] HANGHOA hANGHOA)
        {
            if (ModelState.IsValid)
            {
                db.HANGHOAs.Add(hANGHOA);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHangHoa");
            }

            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // GET: HANGHOA/SuaThongTinHangHoa/5
        public ActionResult SuaThongTinHangHoa(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            if (hANGHOA == null)
            {
                return HttpNotFound();
            }
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // POST: HANGHOA/SuaThongTinHangHoa/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more LayThongTinHangHoa see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinHangHoa([Bind(Include = "MaHH,TenHH,DonGia,DonViTinh,LoaiHang,SoLuongTon,HH_MaNCC")] HANGHOA hANGHOA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hANGHOA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHangHoa");
            }
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // GET: HANGHOA/XoaHangHoa/5
        public ActionResult XoaHangHoa(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            if (hANGHOA == null)
            {
                return HttpNotFound();
            }
            return View(hANGHOA);
        }

        // POST: HANGHOA/XoaHangHoa/5
        [HttpPost, ActionName("XoaHangHoa")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaHangHoaConfirmed(string id)
        {
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            db.HANGHOAs.Remove(hANGHOA);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachHangHoa");
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
