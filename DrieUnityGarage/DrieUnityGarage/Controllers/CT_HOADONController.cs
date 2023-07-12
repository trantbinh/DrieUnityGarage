using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGarage.Models;
using DrieUnityGarage.Models;

namespace DrieUnityGarage.Controllers
{
    public class CT_HOADONController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: CT_HOADON
        public ActionResult Index()
        {
            var cT_HOADON = db.CT_HOADON.Include(c => c.HANGHOA).Include(c => c.HOADON);
            return View(cT_HOADON.ToList());
        }

        // GET: CT_HOADON/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_HOADON cT_HOADON = db.CT_HOADON.Find(id);
            if (cT_HOADON == null)
            {
                return HttpNotFound();
            }
            return View(cT_HOADON);
        }

        // GET: CT_HOADON/Create
        public ActionResult Create()
        {
            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
            ViewBag.CTHD_MaHD = new SelectList(db.HOADONs, "MaHD", "HD_MaKH");
            return View();
        }

        // POST: CT_HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CTHD_MaHH,CTHD_MaHD,SoLuong,ThanhTien")] CT_HOADON cT_HOADON)
        {
            if (ModelState.IsValid)
            {
                db.CT_HOADON.Add(cT_HOADON);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_HOADON.CTHD_MaHH);
            ViewBag.CTHD_MaHD = new SelectList(db.HOADONs, "MaHD", "HD_MaKH", cT_HOADON.CTHD_MaHD);
            return View(cT_HOADON);
        }

        // GET: CT_HOADON/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_HOADON cT_HOADON = db.CT_HOADON.Find(id);
            if (cT_HOADON == null)
            {
                return HttpNotFound();
            }
            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_HOADON.CTHD_MaHH);
            ViewBag.CTHD_MaHD = new SelectList(db.HOADONs, "MaHD", "HD_MaKH", cT_HOADON.CTHD_MaHD);
            return View(cT_HOADON);
        }

        // POST: CT_HOADON/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CTHD_MaHH,CTHD_MaHD,SoLuong,ThanhTien")] CT_HOADON cT_HOADON)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cT_HOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_HOADON.CTHD_MaHH);
            ViewBag.CTHD_MaHD = new SelectList(db.HOADONs, "MaHD", "HD_MaKH", cT_HOADON.CTHD_MaHD);
            return View(cT_HOADON);
        }

        // GET: CT_HOADON/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_HOADON cT_HOADON = db.CT_HOADON.Find(id);
            if (cT_HOADON == null)
            {
                return HttpNotFound();
            }
            return View(cT_HOADON);
        }

        // POST: CT_HOADON/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CT_HOADON cT_HOADON = db.CT_HOADON.Find(id);
            db.CT_HOADON.Remove(cT_HOADON);
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
