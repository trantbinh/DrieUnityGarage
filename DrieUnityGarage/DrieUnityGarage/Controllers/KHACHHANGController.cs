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
            ViewBag.MaKH = TaoMaKhachHang();
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

                var email = db.KHACHHANGs.FirstOrDefault(k => k.Email.Equals(kHACHHANG.Email));
                if (email != null)
                    ModelState.AddModelError(string.Empty, "Email đã tồn tại!");

                if (ModelState.IsValid)
                {
                    kHACHHANG.MaKH = TaoMaKhachHang();
                    kHACHHANG.DiemThanhVien = 0;
                    db.KHACHHANGs.Add(kHACHHANG);
                    db.SaveChanges();
                    return RedirectToAction("LayDanhSachKhachHang");
                }
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
            DateTime date;
            if (kHACHHANG.NgaySinh != null)
            {
                date = (DateTime)kHACHHANG.NgaySinh;
                ViewBag.NgaySinh = date.ToString("yyyy/MM/dd");
            }

            return View(kHACHHANG);
        }
        private String TaoMaKhachHang()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<KHACHHANG> lstHD = db.KHACHHANGs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "KH001";
            }
            else
            {
                KHACHHANG lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaKH;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "KH00" + newMaHD.ToString();
                }
                else { idHD = "KH0" + newMaHD.ToString(); }
            }
            return (idHD);
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
        public ActionResult XoaKhachHang(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "Không thể xoá khách hàng này vì mã khách hàng đã được dùng để tạo thông tin khác";
            }
            else
            {
                if (kHACHHANG == null)
                {
                    return HttpNotFound();
                }
            }
            return View(kHACHHANG);
        }

        // POST: KHACHHANG/Delete/5
        [HttpPost, ActionName("XoaKhachHang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check = KiemTraKhoaNgoaiKhachHang(id);
            if (check == true)
            {

                return XoaKhachHang(id, 0);
            }
            else
            {
                KHACHHANG kHACHHANG = db.KHACHHANGs.Find(id);
                db.KHACHHANGs.Remove(kHACHHANG);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachKhachHang");
            }
        }
        public bool KiemTraKhoaNgoaiKhachHang(string id)
        {
            List<THONGTINTIEPNHAN> tn = db.THONGTINTIEPNHANs.Where(m => m.TN_MaKH.Equals(id)).ToList();
             List<PHUONGTIEN> pt = db.PHUONGTIENs.Where(m => m.PT_MaKH.Equals(id)).ToList();
            List<HOADON> hd = db.HOADONs.Where(m => m.HD_MaKH.Equals(id)).ToList();
            if (pt != null || tn != null||hd!=null)
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
    }
}
