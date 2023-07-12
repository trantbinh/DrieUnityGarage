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
        
        //-----------------ĐĂNG NHẬP------------------//
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(NHANVIEN nv)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(nv.TenDangNhap))
                    ModelState.AddModelError(String.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(nv.MatKhau))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (ModelState.IsValid)
                {
                    var check = db.NHANVIENs.FirstOrDefault(k => k.TenDangNhap.Equals(nv.TenDangNhap)&& k.MatKhau.Equals(nv.MatKhau));

                 

                    if (check != null)
                    {
                        Session["TaiKhoan"] = check;

                        Session["TenDN"] = check.TenDangNhap;

                        Session["MaTaiKhoanNV"] = check.MaNV;

                        if (check.ChucVu.Equals("Quản lý"))
                        {
                            Session["IsAdmin"] = 1;
                        }
                        else
                        {
                            Session["IsAdmin"] = null;
                        }
                        return RedirectToAction("HomePage", "HomePage");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    }

                }


            }
            return View();

        }
        //-----------------ĐĂNG XUẤT------------------//

        public ActionResult LogOut()
        {
            Session.Remove("TaiKhoan");
            Session.Remove("LogName");
            Session.Remove("IDCus");

            return RedirectToAction("HomePage", "CitricStore");
        }






















        // GET: NHANVIEN
        public ActionResult Index()
        {
            return View(db.NHANVIENs.ToList());
        }

        // GET: NHANVIEN/Details/5
        public ActionResult Details(string id)
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: NHANVIEN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNV,HoTenNV,DienThoaiNV,NgaySinh,GioiTinh,Email,DiaChi,ChucVu,PhongBan,TenDangNhap,MatKhau,NgayTaoTK")] NHANVIEN nHANVIEN)
        {
            if (ModelState.IsValid)
            {
                db.NHANVIENs.Add(nHANVIEN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nHANVIEN);
        }

        // GET: NHANVIEN/Edit/5
        public ActionResult Edit(string id)
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
        public ActionResult Edit([Bind(Include = "MaNV,HoTenNV,DienThoaiNV,NgaySinh,GioiTinh,Email,DiaChi,ChucVu,PhongBan,TenDangNhap,MatKhau,NgayTaoTK")] NHANVIEN nHANVIEN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nHANVIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nHANVIEN);
        }

        // GET: NHANVIEN/Delete/5
        public ActionResult Delete(string id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id);
            db.NHANVIENs.Remove(nHANVIEN);
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
