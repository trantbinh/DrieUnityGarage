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
    public class NHANVIENController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: NHANVIEN
        public ActionResult LayDanhSachNhanVien()
        {
            return View(db.NHANVIENs.ToList());
        }

        // GET: NHANVIEN/Details/5
        public ActionResult LayThongTinNhanVien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id);
            if (nHANVIEN == null)
            {
                return HttpNotFound();
            }
            return View(nHANVIEN);
        }

        // GET: NHANVIEN/Create
        public ActionResult ThemNhanVien()
        {
            return View();
        }

        // POST: NHANVIEN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNhanVien([Bind(Include = "MaNV,HoTenNV,DienThoaiNV,NgaySinh,GioiTinh,Email,DiaChi,ChucVu,PhongBan,TenDangNhap,MatKhau,NgayTaoTK")] NHANVIEN nHANVIEN)
        {
            if (ModelState.IsValid)
            {
                List<NHANVIEN> lstNV = db.NHANVIENs.ToList();
                int countLst = lstNV.Count();
                if (countLst == 0)
                {
                    nHANVIEN.MaNV = "NV01";
                }
                else
                {
                    NHANVIEN lastNV = lstNV[countLst - 1];
                    String lastMaNV = lastNV.MaNV;
                    int lastMaNVNum = int.Parse(lastMaNV.Substring(3));
                    int newMaNV = lastMaNVNum + 1;
                    nHANVIEN.MaNV = "NV0" + newMaNV.ToString();
                }

                db.NHANVIENs.Add(nHANVIEN);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachNhanVien");
            }

            return View(nHANVIEN);
        }

        // GET: NHANVIEN/Edit/5
        public ActionResult SuaThongTinNhanVien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id);
            if (nHANVIEN == null)
            {
                return HttpNotFound();
            }
            return View(nHANVIEN);
        }

        // POST: NHANVIEN/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinNhanVien([Bind(Include = "MaNV,HoTenNV,DienThoaiNV,NgaySinh,GioiTinh,Email,DiaChi,ChucVu,PhongBan,TenDangNhap,MatKhau,NgayTaoTK")] NHANVIEN nHANVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHANVIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachNhanVien");
            }
            return View(nHANVIEN);
        }

        // GET: NHANVIEN/Delete/5
        public ActionResult XoaNhanVien(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id);
            if (nHANVIEN == null)
            {
                return HttpNotFound();
            }
            return View(nHANVIEN);
        }

        // POST: NHANVIEN/Delete/5
        [HttpPost, ActionName("XoaNhanVien")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaNhanVienConfirmed(string id)
        {
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id);
            db.NHANVIENs.Remove(nHANVIEN);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachNhanVien");
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
