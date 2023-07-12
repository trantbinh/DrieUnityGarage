using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DrieUnityGrage.Models;

namespace DrieUnityGrage.Controllers
{
    public class HOADONController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        //--------------------------CHI TIẾT HOÁ ĐƠN-------------------------------\\
        //Hàm để lấy danh sách CTHD hiện tại
        public List<SanPham> CTHD_LayDanhSachSanPham()
        {
            List<SanPham> lstSPHD = Session["lstSPHD"] as List<SanPham>;
            //Nếu CTDH chưa tồn tại thì tạo mới và đưa vào Session
            if (lstSPHD == null)
            {
                lstSPHD = new List<SanPham>();
                Session["lstSPHD"] = lstSPHD;
            }
            return lstSPHD;
        }

        //Thêm một sản phẩm vào CTHD
        public ActionResult CTHD_ThemSP(String id)
        {
            List<SanPham> lstSPHD = CTHD_LayDanhSachSanPham();
            SanPham currentProduct = lstSPHD.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new SanPham(id);
                lstSPHD.Add(currentProduct);
            }
            else
            {
                currentProduct.SoLuong++;
            }
            return RedirectToAction("TaoHoaDon", "HOADON");
        }

        //Tính tổng số lượng sản phẩm
        private int TinhTongSoLuong()
        {
            int totalNumber = 0;
            List<SanPham> lstSPHD = CTHD_LayDanhSachSanPham();
            if (lstSPHD != null)
                totalNumber = lstSPHD.Sum(sp => sp.SoLuong);
            return totalNumber;
        }

        //Tính tổng tiền của các sản phẩm
        private decimal TinhTongTien()
        {
            decimal totalPrice = 0;
            List<SanPham> lstSPHD = CTHD_LayDanhSachSanPham();
            if (lstSPHD != null)
                totalPrice = lstSPHD.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }

        //Xoá sản phẩm khỏi CTHD
        public ActionResult CTDH_XoaSP(String id)

        {
            List<SanPham> myCart = CTHD_LayDanhSachSanPham();
            SanPham pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("TaoHoaDon", "HOADON");
            }
            if (myCart == null || myCart.Count == 0)
            {
                return RedirectToAction("TaoHoaDon", "HOADON");
            }
            return RedirectToAction("TaoHoaDon", "HOADON");

        }

        // Cập nhật lại số lượng sản phẩm
        public ActionResult CTDH_CapNhatSoLuong(String id, FormCollection f)
        {
            List<SanPham> myCart = CTHD_LayDanhSachSanPham();
            SanPham pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return RedirectToAction("TaoHoaDon", "HOADON");
        }

        //--------------------------VIEW TAOHOADON-------------------------------\\

        //Hiển thị danh sách CTHD
        public ActionResult Partial_LayChiTietHoaDon()
        {
            List<SanPham> myCart = CTHD_LayDanhSachSanPham();
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(myCart);
        }

        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_ThemChiTietHoaDon()
        { 
            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
            ViewBag.Selected = "a";
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_ThemChiTietHoaDon(CT_HOADON hh)
        {
            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
            ViewBag.Selected = hh.CTHD_MaHH;
            Session["MaHH"] = hh.CTHD_MaHH;
            return RedirectToAction("CTHD_ThemSP", "HOADON", new { id = Session["MaHH"].ToString() });
        }
/*
        public ActionResult Partial_LayThongTinTiepNhan(String id) { 
        
            var a = db.THONGTINTIEPNHANs.Where(m=> m.MaTN.Equals(id)).ToList();
            return PartialView(a);

          
        }

*/
        // GET: HOADON
        public ActionResult LayDanhSachHoaDon()
        {
            var hOADONs = db.HOADONs.Include(h => h.KHACHHANG).Include(h => h.PHUONGTIEN).Include(h => h.THONGTINTHANHTOAN).Include(h => h.THONGTINTIEPNHAN);
            return View(hOADONs.ToList());
        }

        // GET: HOADON/Details/5
        public ActionResult XemChiTietHoaDon(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADONs.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            return View(hOADON);
        }

        // GET: HOADON/Create
        public ActionResult TaoHoaDon(String id)
        {
            Session["CheckTN"] = 0;
            
            String idHD;
            if (id != null)
            {
                idHD = id;
            }
            else
            { idHD = TaoMaHoaDon(); }
            
            HOADON hOADONs = db.HOADONs.Find(id);
            if (hOADONs == null)
            {
                Session["CheckTN"] = null;
            }


            var tn = db.THONGTINTIEPNHANs.ToList();
                ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN");
                ViewBag.MaHD = idHD;
                String date = String.Format("{0:dd/MM/yy}", DateTime.Now.ToString());
                ViewBag.NgayLapHD = date;
                return View(hOADONs);
        }

        // POST: HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult TaoHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                String idHD = TaoMaHoaDon();
                String date = String.Format("{0:dd/MM/yy}", DateTime.Now.ToString());
                hOADON.MaHD = idHD;
                hOADON.NgayLap = DateTime.Now;
                hOADON.HD_MaKH = null;
                hOADON.HD_MaKH = null;
                hOADON.TongThanhToan = null;
                hOADON.HD_MaTT = null;
                db.HOADONs.Add(hOADON);
                db.SaveChanges();
                return RedirectToAction("TaoHoaDon", "HOADON", new {id = idHD});
            }
            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", hOADON.HD_MaTN);
            return View(hOADON);
        }

        // GET: HOADON/Edit/5
        public ActionResult CapNhatHoaDon(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADONs.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", hOADON.HD_MaKH);
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "SoMay", hOADON.HD_BienSoXe);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH", hOADON.HD_MaTT);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", hOADON.HD_MaTT);
            return View(hOADON);
        }

        // POST: HOADON/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHoaDon");
            }
            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", hOADON.HD_MaKH);
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "SoMay", hOADON.HD_BienSoXe);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH", hOADON.HD_MaTT);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", hOADON.HD_MaTT);
            return View(hOADON);
        }

        // GET: HOADON/Delete/5
        public ActionResult XoaHoaDon(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOADON hOADON = db.HOADONs.Find(id);
            if (hOADON == null)
            {
                return HttpNotFound();
            }
            return View(hOADON);
        }

        // POST: HOADON/Delete/5
        [HttpPost, ActionName("XoaHoaDon")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HOADON hOADON = db.HOADONs.Find(id);
            db.HOADONs.Remove(hOADON);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachHoaDon");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //-------------------------- EXTENSIONS -------------------------------\\

        private String TaoMaHoaDon()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<HOADON> lstHD = db.HOADONs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "HD001";
            }
            else
            {
                HOADON lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaHD;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "HD00" + newMaHD.ToString();
                }
                else { idHD = "HD0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

        public ActionResult Extensions_LayThongTinKhachHang(string id)
        {
            var lstThongTin = db.KHACHHANGs.Where(m => m.MaKH.Equals(id)).ToList();
            return PartialView(lstThongTin);
        }


    }
}
