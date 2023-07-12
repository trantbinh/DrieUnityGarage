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
    public class HANGHOAController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: HANGHOA
        public ActionResult Index()
        {
            var hANGHOAs = db.HANGHOAs.Include(h => h.NHACUNGCAP);
            return View(hANGHOAs.ToList());
        }

        // GET: HANGHOA/Details/5
        public ActionResult Details(string id)
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

        // GET: HANGHOA/Create
        public ActionResult Create()
        {
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC");
            return View();
        }

        // POST: HANGHOA/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHH,TenHH,DonGia,DonViTinh,LoaiHang,SoLuongTon,HH_MaNCC")] HANGHOA hANGHOA)
        {
            if (ModelState.IsValid)
            {
                db.HANGHOAs.Add(hANGHOA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // GET: HANGHOA/Edit/5
        public ActionResult Edit(string id)
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

        // POST: HANGHOA/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHH,TenHH,DonGia,DonViTinh,LoaiHang,SoLuongTon,HH_MaNCC")] HANGHOA hANGHOA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hANGHOA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // GET: HANGHOA/Delete/5
        public ActionResult Delete(string id)
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

        // POST: HANGHOA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            db.HANGHOAs.Remove(hANGHOA);
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
