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
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.ThoiGianTiepNhan = date;
            String idTN = TaoMaTiepNhan();
            ViewBag.MaTN = idTN;

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");

            if (Session["DaLayThongTinXe"] != null)
            {
                List<THONGTINPHUONGTIEN> a = Session["lstXe"] as List<THONGTINPHUONGTIEN>;
                ViewBag.lstXe = new SelectList(a, "BienSoXe", "BienSoXe");
                ViewBag.selectedKhachHang = Session["selectedKhachHang"];
                ViewBag.ThoiGianGiaoXe = Session["ThoiGianGiaoXe"];
            }
            else if (Session["KhongCoThongTinXe"] != null) {
                ViewBag.ThongBao = "Khách hàng chưa được tạo thông tin phương tiện!";
            }
            return View();
        }

        //Hàm được sử dụng sau khi nhấn nút lấy danh sách xe
        public ActionResult TaoTTTN_LayThongTinXe([Bind(Include = "MaTN,TN_MaKH,TN_BienSoXe,TN_MaNV,ThoiGianTiepNhan,ThoiGianGiaoDuKien,GhiChuKH")] THONGTINTIEPNHAN tHONGTINTIEPNHAN, String lstMaKH/*, DateTime thoiGianGiaoXe*/)
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


            //String date = thoiGianGiaoXe.ToString("dd/MMM/yyyy");

            Session["selectedKhachHang"] = selectedKH;

            //Thông tin cần lưu của tiếp nhận
            //Session["ThoiGianGiaoXe"] = date;
            Session["MaTN"] = TaoMaTiepNhan();
            Session["MaKH"] = lstMaKH;
            
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
                /* tHONGTINTIEPNHAN.TN_MaNV = Session["MaTaiKhoanNV"].ToString();*/
                tHONGTINTIEPNHAN.TN_MaNV = "NV001";
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
            ,String TN_MaKH, String lstXe,  String GhiChuKH, DateTime ThoiGianDuKien)
        {
            if (ModelState.IsValid)
            {
                tHONGTINTIEPNHAN.TN_MaKH = TN_MaKH;
                tHONGTINTIEPNHAN.TN_BienSoXe = lstXe;
                
                    tHONGTINTIEPNHAN.ThoiGianGiaoDuKien = (DateTime)ThoiGianDuKien;
                tHONGTINTIEPNHAN.GhiChuKH = GhiChuKH;

                db.Entry(tHONGTINTIEPNHAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinTiepNhan");
            }
            return View(tHONGTINTIEPNHAN);
        }

        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinTiepNhan(string id)
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

        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinTiepNhan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            THONGTINTIEPNHAN tHONGTINTIEPNHAN = db.THONGTINTIEPNHANs.Find(id);
            db.THONGTINTIEPNHANs.Remove(tHONGTINTIEPNHAN);
            db.SaveChanges();
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
