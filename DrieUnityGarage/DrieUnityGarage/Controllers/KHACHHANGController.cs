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
    public class KHACHHANGController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: KHACHHANG
        public ActionResult LayDanhSachKhachHang()
        {
            return View(db.KHACHHANGs.ToList());
        }

        // GET: KHACHHANG/Details/5
        public ActionResult LayThongTinKhachHang(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }

        // GET: KHACHHANG/Create
        public ActionResult ThemKhachHang()
        {
            return View();
        }

        // POST: KHACHHANG/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemKhachHang([Bind(Include = "MaKH,HoTenKH,DienThoaiKH,NgaySinh,GioiTinh,Email,DiemThanhVien,DiaChi")] KHACHHANG kHACHHANG)
        {
            if (ModelState.IsValid)
            {
                //Tạo mã nhà cung cấp String
                List<KHACHHANG> lstHH = db.KHACHHANGs.ToList();
                int countLst = lstHH.Count();
                if (countLst == 0)
                {
                    kHACHHANG.MaKH = "KH001";
                }
                else
                {
                    KHACHHANG lastHH = lstHH[countLst - 1];
                    String lastMHH = lastHH.MaKH;
                    int lastMaHHNum = int.Parse(lastMHH.Substring(2));
                    int newMaHH = lastMaHHNum + 1;
                    if (newMaHH < 10)
                    {
                        kHACHHANG.MaKH = "KH00" + newMaHH.ToString();
                    }

                    else
                    {
                        kHACHHANG.MaKH = "KH0" + newMaHH.ToString();

                    }


                }
                kHACHHANG.DiemThanhVien = 0;
                db.KHACHHANGs.Add(kHACHHANG);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachKhachHang");
            }

            return View(kHACHHANG);
        }

        // GET: KHACHHANG/Edit/5
        public ActionResult CapNhatKhachHang(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }

        // POST: KHACHHANG/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatKhachHang([Bind(Include = "MaKH,HoTenKH,DienThoaiKH,NgaySinh,GioiTinh,Email,DiemThanhVien,DiaChi")] KHACHHANG kHACHHANG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kHACHHANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachKhachHang");
            }
            return View(kHACHHANG);
        }

        // GET: KHACHHANG/Delete/5
        public ActionResult XoaKhachHang(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }

        // POST: KHACHHANG/Delete/5
        [HttpPost, ActionName("XoaKhachHang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
            db.KHACHHANGs.Remove(kHACHHANG);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachKhachHang");
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
