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
    public class DATLICHController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        //Lấy danh sách khách hàng
        public List<THONGTINKHACHHANG> LayDanhSachKhachHangDB()
        {
            var newlstKH = new List<THONGTINKHACHHANG>();
            var khDB = db.KHACHHANGs.ToList();
            int c = khDB.Count();
            for (int i = 0; i < c; i++)
            {

                newlstKH.Add(new THONGTINKHACHHANG(khDB[i].MaKH));
            }
            Session.Remove("lstKH");
            Session["lstKH"] = newlstKH;
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
        public ActionResult LayDanhSachThongTinDatLich()
        {
            Session["DaLayThongTinXe"] = null;
            Session["KhongCoThongTinXe"] = null;
            return View(db.DATLICHes.ToList());
        }

        // GET: THONGTINTIEPNHAN/LayThongTinTiepNhan/5
        public ActionResult LayThongTinDatLich(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATLICH dATLICH = db.DATLICHes.Find(id);
            if (dATLICH == null)
            {
                return HttpNotFound();
            }
            return View(dATLICH);
        }


        // GET: THONGTINTIEPNHAN/TaoThongTinTiepNhan
        public ActionResult TaoThongTinDatLich()
        {
            String date = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            ViewBag.NgayLap = date;
            String idDL = TaoMaDatLich();
            ViewBag.MaDL = idDL;
            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");


            if (Session["DaLayThongTinXe"] != null)
            {
                List<THONGTINPHUONGTIEN> a = Session["lstXe"] as List<THONGTINPHUONGTIEN>;
                ViewBag.lstXe = new SelectList(a, "BienSoXe", "BienSoXe");
                ViewBag.selectedKhachHang = Session["selectedKhachHang"];
                ViewBag.selectedNhanVien = Session["selectedNhanVien"];
                ViewBag.ThoiGianGiaoXe = Session["ThoiGianGiaoXe"];
            }
            else if (Session["KhongCoThongTinXe"] != null)
            {
                ViewBag.ThongBao = "Khách hàng chưa được tạo thông tin phương tiện!";
            }
            return View();
        }
        //Hàm được sử dụng sau khi nhấn nút lấy danh sách xe
        public ActionResult TaoTTDL_LayThongTinXe(String lstNhanVien, String lstMaKH)
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

            //Thông tin cần lưu của đặt lịch
            Session["MaDL"] = TaoMaDatLich();
            Session["MaKH"] = lstMaKH;
            Session.Remove("lstXe");
            Session["lstXe"] = lstXe;

            //Check đã lấy thông tin xe hay chưa, có null không
            if (lstXe.Count() == 0)
                Session["KhongCoThongTinXe"] = 3;
            else
                Session["DaLayThongTinXe"] = 3;


            return RedirectToAction("TaoThongTinDatLich");
        }

        //Lưu thông tin tiếp nhận vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoThongTinDatLich(String GhiChuKH, String lstXe,String TinhTrangXe,DateTime NgayHen)
        {
            DATLICH dATLICH = new DATLICH();
            if (ModelState.IsValid)
            {
                String id = TaoMaDatLich();
                dATLICH.MaDL = id;
                dATLICH.DL_MaKH = Session["MaKH"].ToString();
                dATLICH.DL_BienSoXe = lstXe;
                dATLICH.TinhTrangXe = TinhTrangXe;
                dATLICH.NgayLap = DateTime.Now;
                dATLICH.NgayHen = NgayHen;
                dATLICH.GhiChuKH = GhiChuKH;
                db.DATLICHes.Add(dATLICH);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinDatLich");
            }

            return View(dATLICH);
        }

        // GET: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        public ActionResult SuaThongTinDatLich(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATLICH dATLICH = db.DATLICHes.Find(id);
            if (dATLICH == null)
            {
                return HttpNotFound();
            }

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.DL_MaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin", dATLICH.DL_MaKH);


            //Lấy danh sách xe từ database sau đó lọc ra những xe của khách hàng
            var xe = LayDanhSachXeDB();
            List<THONGTINPHUONGTIEN> lstXe = new List<THONGTINPHUONGTIEN>();
            int c = xe.Count();
            for (int i = 0; i < c; i++)
            {
                if (xe[i].TTPT_MaKH.Equals(dATLICH.DL_MaKH))
                    lstXe.Add(new THONGTINPHUONGTIEN(xe[i].BienSoXe));
            }

            ViewBag.lstXe = new SelectList(lstXe, "BienSoXe", "BienSoXe", dATLICH.DL_BienSoXe);
            DateTime date;
            if (dATLICH.NgayHen != null)
            {
                date = (DateTime)dATLICH.NgayHen;
                ViewBag.NgayHen = date.ToString("yyyy/MM/dd");
            }

            return View(dATLICH);
        }


        // POST: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinDatLich(DATLICH dATLICH
            , String DL_MaKH, String lstXe, String GhiChuKH, DateTime NgayHen,String TinhTrangXe)
        {
            if (ModelState.IsValid)
            {
                dATLICH.DL_MaKH = DL_MaKH;
                dATLICH.DL_BienSoXe = lstXe;
                dATLICH.NgayHen = (DateTime)NgayHen;
                dATLICH.TinhTrangXe = TinhTrangXe;
                dATLICH.GhiChuKH = GhiChuKH;
                db.Entry(dATLICH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinDatLich");
            }
            return View(dATLICH);
        }


        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinDatLich(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATLICH dATLICH = db.DATLICHes.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá";
            }
            else
            {
                if (dATLICH == null)
                {
                    return HttpNotFound();
                }
            }
            return View(dATLICH);
        }


        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinDatLich")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DATLICH dATLICH = db.DATLICHes.Find(id);
            db.DATLICHes.Remove(dATLICH);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachThongTinDatLich");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private String TaoMaDatLich()
        {
            String idDL = "";
            //Tạo mã nhà cung cấp String
            List<DATLICH> lstDL = db.DATLICHes.ToList();
            int countLst = lstDL.Count();
            if (countLst == 0)
            {
                idDL = "DL001";
            }
            else
            {
                DATLICH lastDL = lstDL[countLst - 1];
                String lastMaDL = lastDL.MaDL;
                int lastMaDLNum = int.Parse(lastMaDL.Substring(2));
                int newMaDL = lastMaDLNum + 1;
                if (newMaDL < 10)
                {
                    idDL = "DL00" + newMaDL.ToString();
                }
                else { idDL = "DLN0" + newMaDL.ToString(); }
            }
            return (idDL);
        }

    }
}