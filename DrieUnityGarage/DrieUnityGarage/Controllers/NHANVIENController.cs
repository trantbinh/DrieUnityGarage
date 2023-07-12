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
                        return RedirectToAction("HomePage", "DrieUnityGarage");
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

        public ActionResult DangXuat()
        {
            Session.Remove("TaiKhoan");
            Session.Remove("TenDN");
            Session.Remove("MaTaiKhoanNV");
            Session.Remove("IsAdmin");
            return RedirectToAction("HomePage", "DrieUnityGarage");
        }

        //-----------------DANH SÁCH TÀI KHOẢN---------------//
        public ActionResult LayDanhSachTaiKhoan()
        {
            var tk = db.NHANVIENs.ToList();
            return View(tk);
        }


        public ActionResult TaoTaiKhoan()
        {
            ViewBag.MaNV = new SelectList(db.NHANVIENs, "MaNV", "MaNV");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoTaiKhoan([Bind(Include = "TenDangNhap,MatKhau,NgayTaoTK")]  NHANVIEN nv)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(nv).State = EntityState.Modified;
                db.SaveChanges(); 
                return RedirectToAction("LayDanhSachTaiKhoan");
            }
            ViewBag.MaNV = new SelectList(db.NHANVIENs, "MaNV", "MaNV");
            return View(nv);
        }


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
                //Tạo mã nhà cung cấp String
                List<NHANVIEN> lstHH = db.NHANVIENs.ToList();
                int countLst = lstHH.Count();
                if (countLst == 0)
                {
                    nHANVIEN.MaNV = "NV001";
                }
                else
                {
                    NHANVIEN lastHH = lstHH[countLst - 1];
                    String lastMHH = lastHH.MaNV;
                    int lastMaHHNum = int.Parse(lastMHH.Substring(2));
                    int newMaHH = lastMaHHNum + 1;
                    if (newMaHH < 10)
                    {
                        nHANVIEN.MaNV = "NV00" + newMaHH.ToString();
                    }

                    else
                    {
                        nHANVIEN.MaNV = "NV0" + newMaHH.ToString();

                    }


                }
                nHANVIEN.NgayTaoTK = DateTime.Now;
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
