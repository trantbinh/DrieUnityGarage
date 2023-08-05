using DrieUnityGarage.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DrieUnityGarage.Controllers
{
    public class CT_BAOGIAController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: CT_BAOGIA
        public ActionResult Index()
        {
            var cT_BAOGIA = db.CT_BAOGIA.Include(c => c.HANGHOA).Include(c => c.BAOGIA);
            return View(cT_BAOGIA.ToList());
        }

        // GET: CT_BAOGIA/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_BAOGIA cT_BAOGIA = db.CT_BAOGIA.Find(id);
            if (cT_BAOGIA == null)
            {
                return HttpNotFound();
            }
            return View(cT_BAOGIA);
        }

        // GET: CT_HOADON/Create
        public ActionResult Create()
        {
            ViewBag.CTBG_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
            ViewBag.CTBG_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH");
            return View();
        }

        // POST: CT_HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CTBG_MaHH,CTBG_MaBG,SoLuong,ThanhTien")] CT_BAOGIA cT_BAOGIA)
        {
            if (ModelState.IsValid)
            {
                db.CT_BAOGIA.Add(cT_BAOGIA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CTBG_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_BAOGIA.CTBG_MaHH);
            ViewBag.CTBG_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH", cT_BAOGIA.CTBG_MaBG);
            return View(cT_BAOGIA);
        }


        // GET: CT_HOADON/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_BAOGIA cT_BAOGIA = db.CT_BAOGIA.Find(id);
            if (cT_BAOGIA == null)
            {
                return HttpNotFound();
            }
            ViewBag.CTBG_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_BAOGIA.CTBG_MaHH);
            ViewBag.CTBG_MaBG = new SelectList(db.HOADONs, "MaBG", "BG_MaKH", cT_BAOGIA.CTBG_MaBG);
            return View(cT_BAOGIA);
        }

        // POST: CT_HOADON/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CTBG_MaHH,CTBG_MaBG,SoLuong,ThanhTien")] CT_BAOGIA cT_BAOGIA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cT_BAOGIA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CTBG_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_BAOGIA.CTBG_MaHH);
            ViewBag.CTBG_MaBG = new SelectList(db.HOADONs, "MaBG", "BG_MaKH", cT_BAOGIA.CTBG_MaBG);
            return View(cT_BAOGIA);
        }


        // GET: CT_HOADON/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_BAOGIA cT_BAOGIA = db.CT_BAOGIA.Find(id);
            if (cT_BAOGIA == null)
            {
                return HttpNotFound();
            }
            return View(cT_BAOGIA);
        }

        // POST: CT_HOADON/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CT_BAOGIA cT_BAOGIA = db.CT_BAOGIA.Find(id);
            db.CT_BAOGIA.Remove(cT_BAOGIA);
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