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

        // GET: PHIEUTHU
        public ActionResult Index()
        {
            var pHIEUTHUs = db.PHIEUTHUs.Include(p => p.KHACHHANG).Include(p => p.NHANVIEN);
            return View(pHIEUTHUs.ToList());
        }

        // GET: PHIEUTHU/Details/5
        public ActionResult Details(string id)
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
            return View(pHIEUTHU);
        }

        // GET: PHIEUTHU/Create
        public ActionResult Create()
        {
            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH");
            ViewBag.PT_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV");
            return View();
        }

        // POST: PHIEUTHU/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaPT,NgayLap,DiaChi,LyDoNop,SoTien,ChungTuGoc,PT_MaKH,PT_MaNV")] PHIEUTHU pHIEUTHU)
        {
            if (ModelState.IsValid)
            {
                db.PHIEUTHUs.Add(pHIEUTHU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", pHIEUTHU.PT_MaKH);
            ViewBag.PT_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", pHIEUTHU.PT_MaNV);
            return View(pHIEUTHU);
        }

        // GET: PHIEUTHU/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", pHIEUTHU.PT_MaKH);
            ViewBag.PT_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", pHIEUTHU.PT_MaNV);
            return View(pHIEUTHU);
        }

        // POST: PHIEUTHU/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaPT,NgayLap,DiaChi,LyDoNop,SoTien,ChungTuGoc,PT_MaKH,PT_MaNV")] PHIEUTHU pHIEUTHU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pHIEUTHU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PT_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", pHIEUTHU.PT_MaKH);
            ViewBag.PT_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", pHIEUTHU.PT_MaNV);
            return View(pHIEUTHU);
        }

        // GET: PHIEUTHU/Delete/5
        public ActionResult Delete(string id)
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
            return View(pHIEUTHU);
        }

        // POST: PHIEUTHU/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PHIEUTHU pHIEUTHU = db.PHIEUTHUs.Find(id);
            db.PHIEUTHUs.Remove(pHIEUTHU);
            db.SaveChanges();
            return RedirectToAction("Index");
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
