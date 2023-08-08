using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
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
            var tk = db.NHANVIENs.Where(m => m.TenDangNhap != null).ToList();
            return View(tk);
        }
        public ActionResult TaoTaiKhoan()
        {
            var nvDB = db.NHANVIENs.ToList();
            var newlstNV = new List<THONGTINNHANVIEN>();
            for (int i =0; i< nvDB.Count(); i++)
            {
                if (nvDB[i].TenDangNhap != null || nvDB[i].MatKhau!=null)
                    continue;
                else
                    newlstNV.Add(new THONGTINNHANVIEN(nvDB[i].MaNV));
            }
            ViewBag.lstNhanVien = new SelectList(newlstNV, "MaNV", "ThongTin");

            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayTaoTK = date;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoTaiKhoan([Bind(Include = "MaNV,HoTenNV,DienThoaiNV,NgaySinh,GioiTinh,Email,DiaChi,ChucVu,PhongBan,TenDangNhap,MatKhau,NgayTaoTK")] NHANVIEN nv, String lstNhanVien, String XacNhanMK)
        {
            if (ModelState.IsValid)
            {
                if(nv.MatKhau != XacNhanMK)
                    ModelState.AddModelError(string.Empty, "Mật khẩu không khớp!");

                if (ModelState.IsValid)
                {
                    nv.MaNV = lstNhanVien;
                    nv.HoTenNV = "";
                    nv.ChucVu = "";
                    nv.DienThoaiNV = "";
                    nv.NgaySinh = DateTime.Now;
                    nv.GioiTinh = "";
                    nv.Email = "";
                    nv.DiaChi = "";
                    nv.PhongBan = "";

                    nv.NgayTaoTK = DateTime.Now;
                    db.Entry(nv).State = EntityState.Modified;
                    db.Entry(nv).Property(s => s.MatKhau).IsModified = true;
                    db.Entry(nv).Property(s => s.TenDangNhap).IsModified = true;
                    db.Entry(nv).Property(s => s.NgayTaoTK).IsModified = true;
                    db.Entry(nv).Property(s => s.HoTenNV).IsModified = false;
                    db.Entry(nv).Property(s => s.ChucVu).IsModified = false;
                    db.Entry(nv).Property(s => s.DienThoaiNV).IsModified = false;
                    db.Entry(nv).Property(s => s.NgaySinh).IsModified = false;
                    db.Entry(nv).Property(s => s.GioiTinh).IsModified = false;
                    db.Entry(nv).Property(s => s.Email).IsModified = false;
                    db.Entry(nv).Property(s => s.DiaChi).IsModified = false;
                    db.Entry(nv).Property(s => s.PhongBan).IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("LayDanhSachTaiKhoan");
                }
                ViewBag.MaNV = new SelectList(db.NHANVIENs, "MaNV", "MaNV");
                var nvDB = db.NHANVIENs.ToList();
                var newlstNV = new List<THONGTINNHANVIEN>();
                for (int i = 0; i < nvDB.Count(); i++)
                {
                    if (nvDB[i].TenDangNhap != null || nvDB[i].MatKhau != null)
                        continue;
                    else
                        newlstNV.Add(new THONGTINNHANVIEN(nvDB[i].MaNV));
                }
                ViewBag.lstNhanVien = new SelectList(newlstNV, "MaNV", "ThongTin",nv.MaNV);


            }
            return View(nv);
        }

        // GET: NHANVIEN/Edit/5
        public ActionResult SuaTaiKhoan(string id)
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
            String nv = id + " - " + nHANVIEN.HoTenNV + " - "+nHANVIEN.ChucVu;
            ViewBag.selectedNhanVien = nv;

            DateTime date = (DateTime)nHANVIEN.NgayTaoTK;
            String dateFormat = date.ToString("dd/MM/yyyy");
            ViewBag.NgayTaoTK = dateFormat;
            Session["MaNV"] = id;
            return View(nHANVIEN);
        }

        // POST: NHANVIEN/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaTaiKhoan([Bind(Include = "MaNV,HoTenNV,DienThoaiNV,NgaySinh,GioiTinh,Email,DiaChi,ChucVu,PhongBan,TenDangNhap,MatKhau,NgayTaoTK")] NHANVIEN nv, String XacNhanMK)
        {
            var tendn = db.NHANVIENs.FirstOrDefault(k => k.TenDangNhap.Equals(nv.TenDangNhap));
            if (tendn != null)
                ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại!");
            if (nv.MatKhau != XacNhanMK)
                ModelState.AddModelError(string.Empty, "Mật khẩu không khớp!");

            if (ModelState.IsValid)
            {
                nv.MaNV = Session["MaNV"].ToString();
                nv.HoTenNV = "";
                nv.ChucVu = "";
                nv.DienThoaiNV = "";
                nv.NgaySinh = DateTime.Now;
                nv.GioiTinh = "";
                nv.Email = "";
                nv.DiaChi = "";
                nv.PhongBan = "";

                nv.NgayTaoTK = DateTime.Now;
                db.Entry(nv).State = EntityState.Modified;
                db.Entry(nv).Property(s => s.MatKhau).IsModified = true;
                db.Entry(nv).Property(s => s.TenDangNhap).IsModified = true;
                db.Entry(nv).Property(s => s.NgayTaoTK).IsModified = true;
                db.Entry(nv).Property(s => s.HoTenNV).IsModified = false;
                db.Entry(nv).Property(s => s.ChucVu).IsModified = false;
                db.Entry(nv).Property(s => s.DienThoaiNV).IsModified = false;
                db.Entry(nv).Property(s => s.NgaySinh).IsModified = false;
                db.Entry(nv).Property(s => s.GioiTinh).IsModified = false;
                db.Entry(nv).Property(s => s.Email).IsModified = false;
                db.Entry(nv).Property(s => s.DiaChi).IsModified = false;
                db.Entry(nv).Property(s => s.PhongBan).IsModified = false;

                db.SaveChanges();
                return RedirectToAction("LayDanhSachTaiKhoan");
            }
            return View(nv);
        }
        // GET: NHANVIEN/Details/5
        public ActionResult LayThongTinTaiKhoan(string id)
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
            String nv = id + " - " + nHANVIEN.HoTenNV + " - " + nHANVIEN.ChucVu;
            ViewBag.selectedNhanVien = nv;

            DateTime date = (DateTime)nHANVIEN.NgayTaoTK;
            String dateFormat = date.ToString("dd/MM/yyyy");
            ViewBag.NgayTaoTK = dateFormat;

            return View(nHANVIEN);
        }
        public ActionResult XoaTaiKhoan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id); ;

                if (nHANVIEN == null)
                {
                    return HttpNotFound();
                }

            String nv = id + " - " + nHANVIEN.HoTenNV + " - " + nHANVIEN.ChucVu;
            ViewBag.selectedNhanVien = nv;

            DateTime date = (DateTime)nHANVIEN.NgayTaoTK;
            String dateFormat = date.ToString("dd/MM/yyyy");
            ViewBag.NgayTaoTK = dateFormat;

            return View(nHANVIEN);
        }

        // POST: NHANVIEN/Delete/5
        [HttpPost, ActionName("XoaTaiKhoan")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaTaiKhoanConfirmed(string id)
        {
            NHANVIEN nv = db.NHANVIENs.Find(id);
            nv.MaNV = id;
            nv.HoTenNV = "";
            nv.ChucVu = "";
            nv.DienThoaiNV = "";
            nv.NgaySinh = DateTime.Now;
            nv.GioiTinh = "";
            nv.Email = "";
            nv.DiaChi = "";
            nv.PhongBan = "";
            nv.TenDangNhap = null;
            nv.MatKhau = null;
            nv.NgayTaoTK = null;

            db.Entry(nv).State = EntityState.Modified;
            db.Entry(nv).Property(s => s.MatKhau).IsModified = true;
            db.Entry(nv).Property(s => s.TenDangNhap).IsModified = true;
            db.Entry(nv).Property(s => s.NgayTaoTK).IsModified = true;
            db.Entry(nv).Property(s => s.HoTenNV).IsModified = false;
            db.Entry(nv).Property(s => s.ChucVu).IsModified = false;
            db.Entry(nv).Property(s => s.DienThoaiNV).IsModified = false;
            db.Entry(nv).Property(s => s.NgaySinh).IsModified = false;
            db.Entry(nv).Property(s => s.GioiTinh).IsModified = false;
            db.Entry(nv).Property(s => s.Email).IsModified = false;
            db.Entry(nv).Property(s => s.DiaChi).IsModified = false;
            db.Entry(nv).Property(s => s.PhongBan).IsModified = false;

            db.SaveChanges();
            return RedirectToAction("LayDanhSachTaiKhoan");
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
            ViewBag.MaNV = TaoMaNhanVien();
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

                var email = db.NHANVIENs.FirstOrDefault(k => k.Email.Equals(nHANVIEN.Email));
                if (email != null)
                    ModelState.AddModelError(string.Empty, "Email đã tồn tại!");
                if (ModelState.IsValid)
                {
                    nHANVIEN.MaNV = TaoMaNhanVien();
                    db.NHANVIENs.Add(nHANVIEN);
                    db.SaveChanges();
                    return RedirectToAction("LayDanhSachNhanVien");
                }
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
            DateTime date;
            if (nHANVIEN.NgaySinh!= null)
            {
                date = (DateTime)nHANVIEN.NgaySinh;
                ViewBag.NgaySinh = date.ToString("yyyy/MM/dd");
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
        public ActionResult XoaNhanVien(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nHANVIEN = db.NHANVIENs.Find(id); ;


;            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Không thể xoá nhân viên này vì dữ liệu nhân viên đã được liên kết với các dữ liệu khác";
            }
           else
            { 
                if (nHANVIEN == null)
                {
                    return HttpNotFound();
                }
            }
            DateTime date;
            if (nHANVIEN.NgaySinh != null)
            {
                date = (DateTime)nHANVIEN.NgaySinh;
                ViewBag.NgaySinh = date.ToString("yyyy/MM/dd");
            }

            return View(nHANVIEN);
        }

        // POST: NHANVIEN/Delete/5
        [HttpPost, ActionName("XoaNhanVien")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaNhanVienConfirmed(string id)
        {
            bool check = KiemTraKhoaNgoaiNhanVien(id);
            if(check == true)
            {
                return XoaNhanVien(id, 0) ;
            }
            else
            {
                NHANVIEN nHANVIEN = db.NHANVIENs.Find(id);
                db.NHANVIENs.Remove(nHANVIEN);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachNhanVien");
            }
           
        }
        public bool KiemTraKhoaNgoaiNhanVien(string id)
        {
            List<THONGTINTIEPNHAN> tn = db.THONGTINTIEPNHANs.Where(m => m.TN_MaNV.Equals(id)).ToList();
            if (tn.Count()!=0)
            {
                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private String TaoMaNhanVien()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<NHANVIEN> lstHD = db.NHANVIENs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "NV001";
            }
            else
            {
                NHANVIEN lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaNV;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "NV00" + newMaHD.ToString();
                }
                else { idHD = "NV0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}
