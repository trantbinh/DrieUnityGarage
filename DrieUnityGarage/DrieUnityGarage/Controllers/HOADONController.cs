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
using DrieUnityGarage.Models;

namespace DrieUnityGarage.Controllers
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
            return RedirectToAction("TaoHoaDon", "HOADON", new { id = Session["MaHD"] });
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
                return RedirectToAction("TaoHoaDon", "HOADON", new { id = Session["MaHD"] });
            }
            return RedirectToAction("TaoHoaDon", "HOADON", new { id = Session["MaHD"] });

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
            return RedirectToAction("TaoHoaDon", "HOADON", new { id = Session["MaHD"] });
        }

        //--------------------------VIEW TAOHOADON-------------------------------\\

        //Hiển thị danh sách CTHD
        public ActionResult Partial_LayChiTietHoaDonList()
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
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_ThemChiTietHoaDon(CT_HOADON hh)
        {
            if (Session["CheckTN"]==null)
                return RedirectToAction("TaoHoaDon", "HOADON");
            else
            {
                ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
                ViewBag.Selected = hh.CTHD_MaHH;
                Session["MaHH"] = hh.CTHD_MaHH;
                        return RedirectToAction("CTHD_ThemSP", "HOADON", new { id = Session["MaHH"].ToString() });
            }
           
        }
        /*
                public ActionResult Partial_LayThongTinTiepNhan(String id) { 

                    var a = db.THONGTINTIEPNHANs.Where(m=> m.MaTN.Equals(id)).ToList();
                    return PartialView(a);


                }

        */
        [HttpPost]
        public ActionResult CTDH_TaoHoaDon(HOADON hd)
        {
            var tongtien = TinhTongTien();
            List<SanPham> lstSP = CTHD_LayDanhSachSanPham();
            if (ModelState.IsValid)
            {
                hd.MaHD = Session["MaHD"].ToString();
                
                hd.NgayLap = DateTime.Now;
               
                hd.TongThanhToan = tongtien;
                hd.HD_BienSoXe =Session["BienSoXe"].ToString();
                hd.HD_MaKH = Session["MaKH"].ToString();
                hd.HD_MaTN =Session["MaTN"].ToString();
                hd.HD_MaTT = null;
                db.Entry(hd).State = EntityState.Modified;
                db.SaveChanges();

                foreach (var item in lstSP) { 
                    var details = new CT_HOADON();
                    details.CTHD_MaHD= Session["MaHD"].ToString();
                    details.SoLuong = item.SoLuong;
                    details.CTHD_MaHH = item.MaSP;
                    details.ThanhTien = item.FinalPrice();
                    db.CT_HOADON.Add(details);
                    db.SaveChanges();
                }
            }
            Session.Remove("MaHD");
            Session.Remove("MaKH");
            Session.Remove("MaTN");
            Session.Remove("BienSoXe");
            Session.Remove("CheckTN");
            Session.Remove("lstSPHD");

            return RedirectToAction("LayDanhSachHoaDon", "HOADON");
        }
/*        public ActionResult CTHD_ThemCTHD()
        {
                        List<SanPham> lstSP = CTHD_LayDanhSachSanPham();
            if (ModelState.IsValid)
            {
                hd.NgayLap = DateTime.Now;
                hd.MaHD = Session["MaHD"].ToString();
                hd.TongThanhToan = tongtien;
                hd.HD_BienSoXe = "S51";*//* Session["BienSoXe"].ToString();*//*
                hd.HD_MaKH = "KH15";*//*Session["MaKH"].ToString();*//*
                hd.HD_MaTN = "TN22";*//*Session["MaTN"].ToString();*//*
                hd.HD_MaTT = null;
                db.HOADONs.Add(hd); 
                db.SaveChanges();

                foreach (var item in lstSP) { 
                    var details = new CT_HOADON();
                    details.CTHD_MaHD= Session["MaHD"].ToString();
                    details.SoLuong = item.SoLuong;
                    details.CTHD_MaHH = item.MaSP;
                    details.ThanhTien = item.FinalPrice();
                    db.CT_HOADON.Add(details);
                    db.SaveChanges();
                }
             


            }
            return RedirectToAction("LayDanhSachHoaDon", "HOADON");

        }
*/        // GET: HOADON
        public ActionResult LayDanhSachHoaDon()
        {
            Session.Remove("DaLayThongTinTiepNhan");
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
        public List<THONGTINTIEPNHANXE> LayDanhSachTiepNhanDB()
        {
            var newlstKH = new List<THONGTINTIEPNHANXE>();
            var khDB = db.THONGTINTIEPNHANs.Where(m=>m.TrangThai.Equals("Chưa hoàn thành")).ToList();
            int c = khDB.Count();
            for (int i = 0; i < c; i++)
            {

                newlstKH.Add(new THONGTINTIEPNHANXE(khDB[i].MaTN));
            }
            Session.Remove("lstTN");
            Session["lstTN"] = newlstKH;
            return newlstKH;
        }

        // GET: HOADON/Create
        public ActionResult TaoHoaDon()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;
            String idTN = TaoMaHoaDon();
            ViewBag.MaHD = idTN;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin");

            if (Session["DaLayThongTinTiepNhan"] != null)
            {
                ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
                ViewBag.KhachHang = Session["KhachHang"];
                ViewBag.BienSoXe = Session["BienSoXe"];
                ViewBag.NhanVien = Session["NhanVien"] ;
            }
            return View();




            /*            Session["CheckTN"] = 0;
                        String idHD;
                        if (id != null)
                        {
                            idHD = id;

                            //Lấy thông tin tiếp nhận
                            var hd = db.HOADONs.FirstOrDefault(m => m.MaHD.Equals(id));
                            String MaTiepNhan = hd.HD_MaTN;
                            var tn = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(MaTiepNhan));
                            ViewBag.MaTN= tn.MaTN;

                            Session["MaKH"] = tn.TN_MaKH;
                            Session["MaTN"] = tn.MaTN;
                            Session["MaNV"] = tn.TN_MaNV;
                            Session["BienSoXe"] = tn.TN_BienSoXe;

                            //Thông tin khách hàng
                            var kh = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(tn.TN_MaKH));
                            ViewBag.KhachHang =kh.MaKH + " - "+ kh.HoTenKH;
                            ViewBag.SDT = kh.DienThoaiKH;

                            var nv = db.NHANVIENs.FirstOrDefault(m => m.MaNV.Equals(tn.TN_MaNV));
                            ViewBag.NhanVien = nv.MaNV+" - "+nv.HoTenNV;
                        }
                        else
                        { 
                            idHD = TaoMaHoaDon();
                            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN");
                        }

                        HOADON hOADONs = db.HOADONs.Find(id);
                        if (hOADONs == null)
                        {
                            Session["CheckTN"] = null;

                        }

                        ViewBag.MaHD = idHD;
                        String date = DateTime.Now.ToString("dd/MM/yyyy");
                        ViewBag.NgayLapHD = date;
                        Session["MaHD"] = idHD;
                        return View(hOADONs);
            */
        }

        //Hàm được sử dụng sau khi nhấn nút
        public ActionResult TaoHD_LayThongTinTiepNhan(String lstMaTN/*, DateTime thoiGianGiaoXe*/)
        {
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(lstMaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaHD"] = TaoMaHoaDon();
            Session["MaTN"] = lstMaTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            Session["KhachHang"] =kh ;
            Session["BienSoXe"] = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            Session["NhanVien"] = nv;

            //Check đã lấy thông tin xe hay chưa, có null không
            Session["DaLayThongTinTiepNhan"] = 3;

            return RedirectToAction("TaoHoaDon");
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



        public ActionResult Partial_LayChiTietHoaDonDB(string id)
        {
            var ctdh = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
            List<SanPham> lstSp = new List<SanPham>();
            
            for(int i = 0; i< ctdh.Count; i++) {
                
                SanPham sp = new SanPham(ctdh[i].CTHD_MaHH);
                lstSp.Add(sp);
            }
            return PartialView(lstSp);
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
            //Lấy thông tin tiếp nhận
            var hd = db.HOADONs.FirstOrDefault(m => m.MaHD.Equals(id));
            String MaTiepNhan = hd.HD_MaTN;
            var tn = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(MaTiepNhan));
            ViewBag.MaTN = tn.MaTN;

            Session["MaHD"] = id;

            //Thông tin khách hàng
            var kh = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(tn.TN_MaKH));
            ViewBag.KhachHang = kh.MaKH + " - " + kh.HoTenKH;
            ViewBag.SDT = kh.DienThoaiKH;

            var nv = db.NHANVIENs.FirstOrDefault(m => m.MaNV.Equals(tn.TN_MaNV));
            ViewBag.NhanVien = nv.MaNV + " - " + nv.HoTenNV;

            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", hOADON.HD_MaTN);
            return View(hOADON);
        }

        // POST: HOADON/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
/*        [HttpPost]
        public ActionResult CapNhatThongTinTiepNhan(HOADON hOADON)
        {
            var tn = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(hOADON.HD_MaTN));
            if (ModelState.IsValid)
            {
                hOADON.MaHD = Session["MaHD"].ToString();
                hOADON.NgayLap = DateTime.Now;
                hOADON.HD_MaKH = tn.TN_MaKH;
                
                db.Entry(hOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CapNhatHoaDon");
            }
            Session.Remove("MaHD");
            return RedirectToAction("CapNhatHoaDon");

        }
*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            var tn = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(hOADON.HD_MaTN));
            if (ModelState.IsValid)
            {
                hOADON.HD_MaKH = tn.TN_MaKH;
                hOADON.HD_BienSoXe = tn.TN_BienSoXe;
                db.Entry(hOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CapNhatHoaDon");
            }
            
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
            //Lấy thông tin tiếp nhận
            var hd = db.HOADONs.FirstOrDefault(m => m.MaHD.Equals(id));
            String MaTiepNhan = hd.HD_MaTN;
            var tn = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(MaTiepNhan));
            ViewBag.MaTN = tn.MaTN;

            Session["MaHD"] = id;

            //Thông tin khách hàng
            var kh = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(tn.TN_MaKH));
            ViewBag.KhachHang = kh.MaKH + " - " + kh.HoTenKH;
            ViewBag.SDT = kh.DienThoaiKH;

            var nv = db.NHANVIENs.FirstOrDefault(m => m.MaNV.Equals(tn.TN_MaNV));
            ViewBag.NhanVien = nv.MaNV + " - " + nv.HoTenNV;

            return View(hOADON);
        }

        // POST: HOADON/Delete/5
        [HttpPost, ActionName("XoaHoaDon")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check;
            List<CT_HOADON> cthd = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietHD(cthd[i].CTHD_MaHD);
                }
            }
            
               
                HOADON hOADON = db.HOADONs.Find(id);
                db.HOADONs.Remove(hOADON);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHoaDon");
            
            
        }
     /*   // GET: CT_HOADON/Delete/5
        public ActionResult XoaChiTietHD (string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CT_HOADON cT_HOADON = db.CT_HOADON.Find(id);
            if (cT_HOADON == null)
            {
                return HttpNotFound();
            }
            return View(cT_HOADON);
        }*/

        // POST: CT_HOADON/Delete/5
        public bool XoaChiTietHD(string id)
        {
            CT_HOADON cT_HOADON = db.CT_HOADON.FirstOrDefault(m=>m.CTHD_MaHD.Equals(id));
            db.CT_HOADON.Remove(cT_HOADON);
            db.SaveChanges();
            return true;
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
        public ActionResult Extensions_LayTenKhachHang(string id)
        {
            var lstThongTin = db.KHACHHANGs.Where(m => m.MaKH.Equals(id)).ToList();
            return PartialView(lstThongTin);
        }
        public ActionResult Extensions_LayThongTinKhachHang(string id)
        {
            var lstThongTin = db.KHACHHANGs.Where(m => m.MaKH.Equals(id)).ToList();
            return PartialView(lstThongTin);
        }


    }
}
