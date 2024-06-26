﻿using System;
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
        //Lấy danh sách khách hàng
        public List<THONGTINKHACHHANG> LayDanhSachKhachHangDB()
        {
            var newlstKH= new List<THONGTINKHACHHANG>();
            var khDB = db.KHACHHANGs.ToList();
            int c = khDB.Count();
            for (int i = 0;i< c; i++)
            {

                newlstKH.Add(new THONGTINKHACHHANG(khDB[i].MaKH));
            }
            Session.Remove("lstKH");
            Session["lstKH"]= newlstKH;
            return newlstKH;
        }

        //Lấy danh sách khách hàng
        public List<THONGTINNHANVIEN> LayDanhSachNhanVienDB()
        {
            var newlstNV = new List<THONGTINNHANVIEN>();
            var nvDB = db.NHANVIENs.ToList();
            int c = nvDB.Count();
            for (int i = 0; i < c; i++)
            {
                if (nvDB[i].ChucVu.Equals("Quản lý"))
                    continue;
                else
                newlstNV.Add(new THONGTINNHANVIEN(nvDB[i].MaNV));
            }
            Session["lstNV"] = newlstNV;
            return newlstNV;
        }
        public List<THONGTINNHANVIEN> LayDanhSachToanBoNhanVienDB()
        {
            var newlstNV = new List<THONGTINNHANVIEN>();
            var nvDB = db.NHANVIENs.ToList();
            int c = nvDB.Count();
            for (int i = 0; i < c; i++)
            {

                newlstNV.Add(new THONGTINNHANVIEN(nvDB[i].MaNV));
            }
            Session["lstNV"] = newlstNV;
            return newlstNV;
        }


        //Lấy danh sách xe
        public List<THONGTINPHUONGTIEN> LayDanhSachXeDB()
        {
            var newlstKH = new List<THONGTINPHUONGTIEN>();
            var khDB = db.PHUONGTIENs.ToList();
            int c = khDB.Count();
            for (int i = 0; i < c; i++)
            {
                newlstKH.Add(new THONGTINPHUONGTIEN(khDB[i].BienSoXe));
            }
            Session.Remove("lstXe");
            Session["lstXe"] = newlstKH;
            return newlstKH;
        }

        // GET: THONGTINTIEPNHAN
        public ActionResult LayDanhSachThongTinTiepNhan()
        {
            Session["DaLayThongTinXe"] = null;
            Session["KhongCoThongTinXe"] = null;
            return View(db.THONGTINTIEPNHANs.ToList());
        }

        // GET: THONGTINTIEPNHAN/LayThongTinTiepNhan/5
        public ActionResult LayThongTinTiepNhan(string id)
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

        // GET: THONGTINTIEPNHAN/TaoThongTinTiepNhan
        public ActionResult TaoThongTinTiepNhan()
        {
            String date = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            ViewBag.ThoiGianTiepNhan = date;
            String idTN = TaoMaTiepNhan();
            ViewBag.MaTN = idTN;

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");

            List<THONGTINNHANVIEN> lstNhanVien = LayDanhSachNhanVienDB();
            ViewBag.lstNhanVien = new SelectList(lstNhanVien, "MaNV", "ThongTin");


            if (Session["DaLayThongTinXe"] != null)
            {
                List<THONGTINPHUONGTIEN> a = Session["lstXe"] as List<THONGTINPHUONGTIEN>;
                ViewBag.lstXe = new SelectList(a, "BienSoXe", "BienSoXe");
                ViewBag.selectedKhachHang = Session["selectedKhachHang"];
                ViewBag.selectedNhanVien = Session["selectedNhanVien"];
                ViewBag.ThoiGianGiaoXe = Session["ThoiGianGiaoXe"];
            }
            else if (Session["KhongCoThongTinXe"] != null) {
                ViewBag.ThongBao = "Khách hàng chưa được tạo thông tin phương tiện!";
            }
            return View();
        }

        //Hàm được sử dụng sau khi nhấn nút lấy danh sách xe
        public ActionResult TaoTTTN_LayThongTinXe(String lstNhanVien, String lstMaKH/*, DateTime thoiGianGiaoXe*/)
        {
            //Lấy danh sách xe từ database sau đó lọc ra những xe của khách hàng
            var xe = LayDanhSachXeDB();
            List<THONGTINPHUONGTIEN> lstXe = new List<THONGTINPHUONGTIEN>();
            int c = xe.Count();
            for (int i = 0; i < c; i++)
            {
                if (xe[i].TTPT_MaKH.Equals(lstMaKH))
                    lstXe.Add(new THONGTINPHUONGTIEN(xe[i].BienSoXe));
            }

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINKHACHHANG TTKH = new THONGTINKHACHHANG(lstMaKH);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedKH = TTKH.MaKH + " - " + TTKH.DienThoaiKH + " - " + TTKH.HoTenKH;

            Session["selectedKhachHang"] = selectedKH;

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINNHANVIEN TTNV = new THONGTINNHANVIEN(lstNhanVien);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedNV = TTNV.MaNV + " - " + TTNV.HoTenNV + " - " + TTNV.ChucVu;

            Session["selectedNhanVien"] = selectedNV;

            //Thông tin cần lưu của tiếp nhận
            //Session["ThoiGianGiaoXe"] = date;
            Session["MaTN"] = TaoMaTiepNhan();
            Session["MaKH"] = lstMaKH;
            Session["MaNV"] = lstNhanVien;
            Session.Remove("lstXe");
            Session["lstXe"] = lstXe;

            //Check đã lấy thông tin xe hay chưa, có null không
            if (lstXe.Count()==0)
                Session["KhongCoThongTinXe"] = 3;
            else 
                Session["DaLayThongTinXe"] = 3;


            return RedirectToAction("TaoThongTinTiepNhan");
        }
        //Lưu thông tin tiếp nhận vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoThongTinTiepNhan(String GhiChuKH, String lstXe)
        {
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = new THONGTINTIEPNHAN();
            if (ModelState.IsValid)
            {
                String id = TaoMaTiepNhan();
                tHONGTINTIEPNHAN.MaTN = id;
                tHONGTINTIEPNHAN.TN_MaNV = Session["MaNV"].ToString();
                tHONGTINTIEPNHAN.TN_MaKH = Session["MaKH"].ToString();
                tHONGTINTIEPNHAN.TN_BienSoXe = lstXe;
                tHONGTINTIEPNHAN.ThoiGianTiepNhan = DateTime.Now;
                tHONGTINTIEPNHAN.ThoiGianGiaoDuKien = null;
                tHONGTINTIEPNHAN.GhiChuKH=GhiChuKH;
                tHONGTINTIEPNHAN.TrangThai = "Chưa hoàn thành";
                db.THONGTINTIEPNHANs.Add(tHONGTINTIEPNHAN);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinTiepNhan");
            }

            return View(tHONGTINTIEPNHAN);
        }
        // GET: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        public ActionResult SuaThongTinTiepNhan(string id)
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

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.TN_MaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin", tHONGTINTIEPNHAN.TN_MaKH);

            List<THONGTINNHANVIEN> lstNhanVien = LayDanhSachNhanVienDB();
            ViewBag.TN_MaNV = new SelectList(lstNhanVien, "MaNV", "ThongTin", tHONGTINTIEPNHAN.TN_MaNV);

            //Lấy danh sách xe từ database sau đó lọc ra những xe của khách hàng
            var xe = LayDanhSachXeDB();
            List<THONGTINPHUONGTIEN> lstXe = new List<THONGTINPHUONGTIEN>();
            int c = xe.Count();
            for (int i = 0; i < c; i++)
            {
                if (xe[i].TTPT_MaKH.Equals(tHONGTINTIEPNHAN.TN_MaKH))
                    lstXe.Add(new THONGTINPHUONGTIEN(xe[i].BienSoXe));
            }

            ViewBag.lstXe = new SelectList(lstXe, "BienSoXe", "BienSoXe", tHONGTINTIEPNHAN.TN_BienSoXe);
            DateTime date;
            if (tHONGTINTIEPNHAN.ThoiGianGiaoDuKien != null)
            {
                date = (DateTime)tHONGTINTIEPNHAN.ThoiGianGiaoDuKien;
                ViewBag.ThoiGianGiaoDuKien = date.ToString("yyyy/MM/dd");
            }
             
            return View(tHONGTINTIEPNHAN);
        }

        // POST: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinTiepNhan(THONGTINTIEPNHAN tHONGTINTIEPNHAN
            ,String TN_MaKH, String lstXe,  String GhiChuKH, DateTime ThoiGianDuKien, String TN_MaNV)
        {
            if (ModelState.IsValid)
            {
                tHONGTINTIEPNHAN.TN_MaKH = TN_MaKH;
                tHONGTINTIEPNHAN.TN_BienSoXe = lstXe;
                tHONGTINTIEPNHAN.TN_MaNV = TN_MaNV;
                tHONGTINTIEPNHAN.ThoiGianGiaoDuKien = (DateTime)ThoiGianDuKien;
                tHONGTINTIEPNHAN.GhiChuKH = GhiChuKH;

                db.Entry(tHONGTINTIEPNHAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinTiepNhan");
            }
            return View(tHONGTINTIEPNHAN);
        }

        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinTiepNhan(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
                if (check == 0)
                {
                    ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá";
                }
                else
                {
                if (tHONGTINTIEPNHAN == null)
                {
                    return HttpNotFound();
                }
            }           
            return View(tHONGTINTIEPNHAN);
        }
        
        public bool KiemTraKhoaNgoai(string id)
        {
            List<HOADON> tn = db.HOADONs.Where(m => m.HD_MaTN.Equals(id)).ToList();
            List<BAOGIA> bg = db.BAOGIAs.Where(m => m.BG_MaTN.Equals(id)).ToList();
            if (tn.Count() == 0 && bg.Count()==0)
            {
                return false;
            }
            return true;
        }

        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinTiepNhan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check = KiemTraKhoaNgoai(id);
            if (check == true)
            {
                return XoaThongTinTiepNhan(id, 0);
            }
            else
            {
                THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
                db.THONGTINTIEPNHANs.Remove(tHONGTINTIEPNHAN);
                db.SaveChanges();
            }
          
            return RedirectToAction("LayDanhSachThongTinTiepNhan");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private String TaoMaTiepNhan()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<THONGTINTIEPNHAN> lstHD = db.THONGTINTIEPNHANs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "TN001";
            }
            else
            {
                THONGTINTIEPNHAN lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaTN;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "TN00" + newMaHD.ToString();
                }
                else { idHD = "TN0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}
