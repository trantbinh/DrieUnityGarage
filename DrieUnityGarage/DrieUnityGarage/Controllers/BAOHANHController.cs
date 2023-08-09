using DrieUnityGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;

namespace DrieUnityGarage.Controllers
{
    public class BAOHANHController : Controller
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

        public ActionResult LayDanhSachThongTinBaoHanh()
        {
            Session["DaLayThongTinXe"] = null;
            Session["KhongCoThongTinXe"] = null;
            return View(db.BAOHANHs.ToList());
        }

        public ActionResult LayThongTinBaoHanh(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOHANH bAOHANH = db.BAOHANHs.Find(id);
            if (bAOHANH == null)
            {
                return HttpNotFound();
            }
            return View(bAOHANH);
        }


        public ActionResult TaoThongTinBaoHanh()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.ThoiGianBatDau = date;

            String idBH = TaoMaBaoHanh();
            ViewBag.MaBH = idBH;
            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.lstMaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin");

            ViewBag.lstHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", "HH000");
            
 
            if (Session["DaLayThongTinXe"] != null)
            {
                List<THONGTINPHUONGTIEN> a = Session["lstXe"] as List<THONGTINPHUONGTIEN>;
                ViewBag.lstXe = new SelectList(a, "BienSoXe", "BienSoXe");
                ViewBag.selectedKhachHang = Session["selectedKhachHang"];

            }
            else if (Session["KhongCoThongTinXe"] != null)
            {
                ViewBag.ThongBao = "Khách hàng chưa được tạo thông tin phương tiện!";
            }
            return View();
        }

        //Hàm được sử dụng sau khi nhấn nút lấy danh sách xe
        public ActionResult TaoTTBH_LayThongTinXe(String lstMaKH)
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
            Session["MaBH"] = TaoMaBaoHanh();
            Session["MaKH"] = lstMaKH;
            Session.Remove("lstXe");
            Session["lstXe"] = lstXe;

            //Check đã lấy thông tin xe hay chưa, có null không
            if (lstXe.Count() == 0)
                Session["KhongCoThongTinXe"] = 3;
            else
                Session["DaLayThongTinXe"] = 3;

            return RedirectToAction("TaoThongTinBaoHanh");
        }
        //Lưu thông tin tiếp nhận vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoThongTinBaoHanh(String lstXe,int ThoiHanBaoHanh,DateTime ThoiGianKetThuc,String NoiBaoHanh, String lstHH)
        {
            BAOHANH bAOHANH = new BAOHANH();
            if (ModelState.IsValid)
            {
                String id = TaoMaBaoHanh();
                bAOHANH.MaBH = id;
                bAOHANH.BH_MaKH = Session["MaKH"].ToString();
                bAOHANH.BH_BienSoXe = lstXe;
                bAOHANH.BH_MaHH = lstHH;
                bAOHANH.ThoiHanBaoHanh = ThoiHanBaoHanh;
                bAOHANH.ThoiGianBatDau = DateTime.Now;
                bAOHANH.ThoiGianKetThuc = ThoiGianKetThuc;
                bAOHANH.NoiBaoHanh = NoiBaoHanh;
                db.BAOHANHs.Add(bAOHANH);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinBaoHanh");
            }
            ViewBag.BH_MaHH = new SelectList(db.HANGHOAs, "MaHH", "MaHH");
            return View(bAOHANH);
        }

        // GET: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        public ActionResult SuaThongTinBaoHanh(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOHANH bAOHANH = db.BAOHANHs.Find(id);
            if (bAOHANH == null)
            {
                return HttpNotFound();
            }

            List<THONGTINKHACHHANG> lstKhachHang = LayDanhSachKhachHangDB();
            ViewBag.BH_MaKH = new SelectList(lstKhachHang, "MaKH", "ThongTin", bAOHANH.BH_MaKH);
            ViewBag.BH_MaHH = new SelectList(db.HANGHOAs, "MaHH", "MaHH");

            //Lấy danh sách xe từ database sau đó lọc ra những xe của khách hàng
            var xe = LayDanhSachXeDB();
            List<THONGTINPHUONGTIEN> lstXe = new List<THONGTINPHUONGTIEN>();
            int c = xe.Count();
            for (int i = 0; i < c; i++)
            {
                if (xe[i].TTPT_MaKH.Equals(bAOHANH.BH_MaKH))
                    lstXe.Add(new THONGTINPHUONGTIEN(xe[i].BienSoXe));
            }

            ViewBag.lstXe = new SelectList(lstXe, "BienSoXe", "BienSoXe", bAOHANH.BH_BienSoXe);
            DateTime date;
            if (bAOHANH.ThoiGianKetThuc != null)
            {
                date = (DateTime)bAOHANH.ThoiGianKetThuc;
                ViewBag.ThoiGianBatDau = DateTime.Now;
                ViewBag.ThoiGianKetThuc = date.ToString("yyyy/MM/dd");
            }

            return View(bAOHANH);
        }


        // POST: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinBaoHanh(BAOHANH bAOHANH
            , String BH_MaKH, String lstXe,  int ThoiHanBaoHanh, DateTime ThoiGianKetThuc,String NoiBaoHanh)
        {
            if (ModelState.IsValid)
            {
                bAOHANH.BH_MaKH = BH_MaKH;
                bAOHANH.BH_BienSoXe = lstXe;
                bAOHANH.ThoiHanBaoHanh = ThoiHanBaoHanh;
                bAOHANH.ThoiGianBatDau = DateTime.Now;
                bAOHANH.ThoiGianKetThuc = ThoiGianKetThuc;
                bAOHANH.NoiBaoHanh = NoiBaoHanh;
                db.Entry(bAOHANH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinBaoHanh");
            }
            ViewBag.BH_MaHH = new SelectList(db.HANGHOAs, "MaHH", "MaHH");
            return View(bAOHANH);
        }

        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinBaoHanh(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOHANH bAOHANH = db.BAOHANHs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá";
            }
            else
            {
                if (bAOHANH == null)
                {
                    return HttpNotFound();
                }
            }
            DateTime date;
            if(bAOHANH.ThoiGianBatDau == DateTime.Now)
            {
                date = (DateTime)bAOHANH.ThoiGianKetThuc;
                ViewBag.ThoiGianKetThuc = date.ToString("yyyy/MM/dd");
            }
            return View(bAOHANH);
        }


        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinBaoHanh")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            BAOHANH bAOHANH = db.BAOHANHs.Find(id);
            db.BAOHANHs.Remove(bAOHANH);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachThongTinBaoHanh");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private String TaoMaBaoHanh()
        {
            String idBH = "";
            //Tạo mã nhà cung cấp String
            List<BAOHANH> lstBH = db.BAOHANHs.ToList();
            int countLst = lstBH.Count();
            if (countLst == 0)
            {
                idBH = "BH001";
            }
            else
            {
                BAOHANH lastBH = lstBH[countLst - 1];
                String lastMaBH = lastBH.MaBH;
                int lastMaBHNum = int.Parse(lastMaBH.Substring(2));
                int newMaBH = lastMaBHNum + 1;
                if (newMaBH < 10)
                {
                    idBH = "BH00" + newMaBH.ToString();
                }
                else { idBH = "BHN0" + newMaBH.ToString(); }
            }
            return (idBH);
        }

    }
}