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
    public class THONGTINTIEPNHANController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();


        public TIEPNHAN LayThongTinTiepNhan()
        {
            TIEPNHAN lstTN = Session["TiepNhan"] as TIEPNHAN;
            if (lstTN == null)
            {
                Session["TiepNhan"] = lstTN;
            }
            return lstTN;
        }
        public ActionResult ThemThongTinTiepNhan(String id)
        {
            TIEPNHAN lstTN = LayThongTinTiepNhan();
            TIEPNHAN newTN;
            if (lstTN == null)
            {
                newTN = new TIEPNHAN(id);
            }
            return RedirectToAction("Create");
        }





        // GET: THONGTINTIEPNHAN
        public ActionResult Index()
        {
            return View(db.THONGTINTIEPNHANs.ToList());
        }

        // GET: THONGTINTIEPNHAN/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
            if (tHONGTINTIEPNHAN == null)
            {
                return HttpNotFound();
            }
            return View(tHONGTINTIEPNHAN);
        }

        // GET: THONGTINTIEPNHAN/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: THONGTINTIEPNHAN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaTN,TN_MaKH,TN_BienSoXe,TN_MaNV,ThoiGianTiepNhan,ThoiGianGiaoDuKien,GhiChuKH")] THONGTINTIEPNHAN tHONGTINTIEPNHAN)
        {
            if (ModelState.IsValid)
            {
                db.THONGTINTIEPNHANs.Add(tHONGTINTIEPNHAN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tHONGTINTIEPNHAN);
        }

        // GET: THONGTINTIEPNHAN/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
            if (tHONGTINTIEPNHAN == null)
            {
                return HttpNotFound();
            }
            return View(tHONGTINTIEPNHAN);
        }

        // POST: THONGTINTIEPNHAN/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTN,TN_MaKH,TN_BienSoXe,TN_MaNV,ThoiGianTiepNhan,ThoiGianGiaoDuKien,GhiChuKH")] THONGTINTIEPNHAN tHONGTINTIEPNHAN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tHONGTINTIEPNHAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tHONGTINTIEPNHAN);
        }

        // GET: THONGTINTIEPNHAN/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
            if (tHONGTINTIEPNHAN == null)
            {
                return HttpNotFound();
            }
            return View(tHONGTINTIEPNHAN);
        }

        // POST: THONGTINTIEPNHAN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
            db.THONGTINTIEPNHANs.Remove(tHONGTINTIEPNHAN);
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
