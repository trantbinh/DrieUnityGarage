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
    public class NHACUNGCAPController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: NHACUNGCAP
        public ActionResult LayDanhSachNhaCungCap()
        {
            return View(db.NHACUNGCAPs.ToList());
        }

        // GET: NHACUNGCAP/Details/5
        public ActionResult LayThongTinNhaCungCap(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHACUNGCAP nHACUNGCAP = db.NHACUNGCAPs.Find(id);
            if (nHACUNGCAP == null)
            {
                return HttpNotFound();
            }
            return View(nHACUNGCAP);
        }

        // GET: NHACUNGCAP/Create
        public ActionResult ThemNhaCungCap()
        {
            return View();
        }

        // POST: NHACUNGCAP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNhaCungCap([Bind(Include = "MaNCC,TenNCC,DiaChiNCC,DienThoaiNCC,MaSoThueNCC,Email,LoaiHinh,HoTenNguoiDaiDien")] NHACUNGCAP nHACUNGCAP, String LoaiHinh)
        {
            if (ModelState.IsValid)
            {
                //Tạo mã nhà cung cấp String
                List<NHACUNGCAP> lstNCC = db.NHACUNGCAPs.ToList();
                int countLst = lstNCC.Count();
                if(countLst == 0)
                {
                    nHACUNGCAP.MaNCC = "NC01";
                }
                else
                {
                    NHACUNGCAP lastNCC = lstNCC[countLst - 1];
                    String lastMaNCC = lastNCC.MaNCC;
                    int lastMaNCCNum = int.Parse(lastMaNCC.Substring(3));
                    int newMaNCC = lastMaNCCNum + 1;
                    nHACUNGCAP.MaNCC = "NC0" + newMaNCC.ToString();
                }
                nHACUNGCAP.LoaiHinh= LoaiHinh;
                db.NHACUNGCAPs.Add(nHACUNGCAP);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachNhaCungCap");
            }

            return View(nHACUNGCAP);
        }

        // GET: NHACUNGCAP/Edit/5
        public ActionResult SuaNhaCungCap(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHACUNGCAP nHACUNGCAP = db.NHACUNGCAPs.Find(id);
            if (nHACUNGCAP == null)
            {
                return HttpNotFound();
            }
            return View(nHACUNGCAP);
        }

        // POST: NHACUNGCAP/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaNhaCungCap([Bind(Include = "MaNCC,TenNCC,DiaChiNCC,DienThoaiNCC,MaSoThueNCC,Email,LoaiHinh,HoTenNguoiDaiDien")] NHACUNGCAP nHACUNGCAP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHACUNGCAP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachNhaCungCap");
            }
            return View(nHACUNGCAP);
        }

        // GET: NHACUNGCAP/Delete/5
        public ActionResult XoaNhacungCap(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHACUNGCAP nHACUNGCAP = db.NHACUNGCAPs.Find(id);
            if (nHACUNGCAP == null)
            {
                return HttpNotFound();
            }
            return View(nHACUNGCAP);
        }

        // POST: NHACUNGCAP/Delete/5
        [HttpPost, ActionName("XoaNhacungCap")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(String id)
        {
            NHACUNGCAP nHACUNGCAP = db.NHACUNGCAPs.Find(id);
            db.NHACUNGCAPs.Remove(nHACUNGCAP);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachNhaCungCap");
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
