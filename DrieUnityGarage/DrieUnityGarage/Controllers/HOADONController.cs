﻿using System;
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
        public List<THONGTINTIEPNHANXE> LayDanhSachTiepNhanDB()
        {
            var newlstKH = new List<THONGTINTIEPNHANXE>();
            var khDB = db.THONGTINTIEPNHANs.Where(m => m.TrangThai.Equals("Chưa hoàn thành")).ToList();
            int c = khDB.Count();
            for (int i = 0; i < c; i++)
            {
                newlstKH.Add(new THONGTINTIEPNHANXE(khDB[i].MaTN));
            }
            Session.Remove("lstTN");
            Session["lstTN"] = newlstKH;
            return newlstKH;
        }

        //--------------------------CHI TIẾT HOÁ ĐƠN-------------------------------\\
        //Hàm để lấy danh sách CTHD hiện tại
        public List<THONGTINSANPHAM> CTHD_LayDanhSachSanPham()
        {
            List<THONGTINSANPHAM> lstSPHD = Session["lstSPHD"] as List<THONGTINSANPHAM>;
            //Nếu CTDH chưa tồn tại thì tạo mới và đưa vào Session
            if (lstSPHD == null)
            {
                lstSPHD = new List<THONGTINSANPHAM>();
                Session["lstSPHD"] = lstSPHD;
            }
            return lstSPHD;
        }

        //Thêm một sản phẩm vào CTHD
        public ActionResult CTHD_ThemSP(String id)
        {
            List<THONGTINSANPHAM> lstSPHD = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM currentProduct = lstSPHD.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new THONGTINSANPHAM(id);
                lstSPHD.Add(currentProduct);
            }
            else
            {
                currentProduct.SoLuong++;
            }
            return RedirectToAction("ThemHoaDon", "HOADON");
        }

        //Tính tổng số lượng sản phẩm
        private int TinhTongSoLuong()
        {
            int totalNumber = 0;
            List<THONGTINSANPHAM> lstSPHD = CTHD_LayDanhSachSanPham();
            if (lstSPHD != null)
                totalNumber = lstSPHD.Sum(sp => sp.SoLuong);
            return totalNumber;
        }

        //Tính tổng tiền của các sản phẩm
        private decimal TinhTongTien()
        {
            decimal totalPrice = 0;
            List<THONGTINSANPHAM> lstSPHD = CTHD_LayDanhSachSanPham();
            if (lstSPHD != null)
                totalPrice = lstSPHD.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }

        //Xoá sản phẩm khỏi CTHD
        public ActionResult CTDH_XoaSP(String id)
        {
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("ThemHoaDon", "HOADON", new { id = Session["MaHD"] });
            }
            return RedirectToAction("ThemHoaDon", "HOADON", new { id = Session["MaHD"] });
        }


        // Cập nhật lại số lượng sản phẩm
        public ActionResult CTDH_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        //--------------------------Tạo hoá đơn-------------------------------\\

        //Hiển thị danh sách CTHD
        public ActionResult Partial_TaoHD_LayChiTietHoaDon()
        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(myCart);
        }

        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_TaoHD_ThemChiTietHoaDon()
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTmp > 0).ToList();
            ViewBag.CTHD_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_TaoHD_ThemChiTietHoaDon(CT_HOADON hh)
        {
            if (Session["DaLayThongTinTiepNhan"] ==null)
                return RedirectToAction("ThemHoaDon", "HOADON");
            else
            {
                var lstHH = db.HANGHOAs.Where(m => m.SoLuongTmp > 0).ToList();
                ViewBag.CTHD_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
                ViewBag.Selected = hh.CTHD_MaHH;
                Session["MaHH"] = hh.CTHD_MaHH;
                return RedirectToAction("CTHD_ThemSP", "HOADON", new { id = Session["MaHH"].ToString() });
            }
           
        }
 
        [HttpPost]
        public ActionResult CTDH_ThemHoaDon(HOADON hd)
        {
            var lstSPHD = Session["lstSPHD"] as List<THONGTINSANPHAM>;
            if (Session["DaLayThongTinTiepNhan"] != null && lstSPHD.Count()!=0)
            {
                var tongtien = TinhTongTien();
                List<THONGTINSANPHAM> lstSP = CTHD_LayDanhSachSanPham();
                if (ModelState.IsValid)
                {
                    hd.MaHD = Session["MaHD"].ToString();
                    hd.NgayLap = DateTime.Now;
                    hd.TongThanhToan = tongtien;
                    hd.HD_BienSoXe = Session["BienSoXe"].ToString();
                    hd.HD_MaKH = Session["MaKH"].ToString();
                    hd.HD_MaTN = Session["MaTN"].ToString();
                    hd.HD_MaTT = null;
                    db.HOADONs.Add(hd);

                    String maKH = Session["MaKH"].ToString();
                    KHACHHANG kHACHHANG = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(maKH));
                    int diemThanhVien = (int)tongtien * 10 / 100;

                    if (kHACHHANG.DiemThanhVien == null)
                        kHACHHANG.DiemThanhVien = diemThanhVien;
                    else kHACHHANG.DiemThanhVien = kHACHHANG.DiemThanhVien + (int)diemThanhVien;

                    kHACHHANG.MaKH = maKH;
                    kHACHHANG.HoTenKH = "";
                    kHACHHANG.DienThoaiKH = "";
                    kHACHHANG.NgaySinh = DateTime.Now;
                    kHACHHANG.GioiTinh = "";
                    kHACHHANG.Email = "";
                    kHACHHANG.DiaChi = "";

                    db.KHACHHANGs.Attach(kHACHHANG);
                    db.Entry(kHACHHANG).Property(s => s.DiemThanhVien).IsModified = true;

                    db.SaveChanges();

                    foreach (var item in lstSP)
                    {
                        var details = new CT_HOADON();
                        details.CTHD_MaHD = Session["MaHD"].ToString();
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


                return RedirectToAction("TaoThongTinThanhToan", "THONGTINTHANHTOAN");


            }
            else
            {
                return RedirectToAction("ThemHoaDon", "HOADON");

            }
        }
        // GET: HOADON
        public ActionResult LayDanhSachHoaDon()
        {
            Session.Remove("DaLayThongTinTiepNhan");
                Session.Remove("c");
            Session.Remove("lstSPHD");
            var tn = db.THONGTINTHANHTOANs.ToList();
            Session["lstTT"] = tn;

            var hOADONs = db.HOADONs.Include(h => h.KHACHHANG).Include(h => h.PHUONGTIEN).Include(h => h.THONGTINTHANHTOAN).Include(h => h.THONGTINTIEPNHAN);
            return View(hOADONs.ToList());
        }

        // GET: HOADON/Details/5
        public ActionResult XemThongTinHoaDon(string id)
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

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(hOADON.HD_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan= selectedTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            ViewBag.KhachHang = kh;
            ViewBag.BienSoXe = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            return View(hOADON);
        }
        public ActionResult Partial_XemHD_LayDuLieuCTHD(String id)
        {
            var cthd = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
            List<THONGTINSANPHAM> lstHH = new List<THONGTINSANPHAM>();
            for(int i =0; i< cthd.Count(); i++)
            {
                lstHH.Add(new THONGTINSANPHAM(cthd[i].CTHD_MaHH, cthd[i].SoLuong));
            }
                int totalNumber = 0;
                
                if (lstHH != null)
                    totalNumber = lstHH.Sum(sp => sp.SoLuong);

                decimal totalPrice = 0;
                if (lstHH != null)
                    totalPrice = lstHH.Sum(sp => sp.FinalPrice());
            

            ViewBag.TotalNumber = totalNumber;
            ViewBag.TotalPrice = totalPrice;

            return PartialView(lstHH);
        }



        // GET: HOADON/Create
        public ActionResult ThemHoaDon()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;
         
            String idTN = TaoMaHoaDon();
            ViewBag.MaHD = idTN;
            Session["MaHD_ThanhToan"] = idTN;
            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin");

            if (Session["DaLayThongTinTiepNhan"] != null)
            {
                ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
                ViewBag.KhachHang = Session["KhachHang"];
                ViewBag.BienSoXe = Session["BienSoXe"];
                ViewBag.NhanVien = Session["NhanVien"];
            }
            return View();
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
            Session["MaKH"] = TTTN.MaKH;
            Session["MaKH"] = TTTN.MaKH;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            Session["KhachHang"] = kh;
            Session["BienSoXe"] = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            Session["NhanVien"] = nv;

            //Check đã lấy thông tin xe hay chưa, có null không
            Session["DaLayThongTinTiepNhan"] = 3;

            return RedirectToAction("ThemHoaDon");
        }


        // POST: HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        //KHÔNG XÀI
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ThemHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                String idHD = TaoMaHoaDon();
                hOADON.MaHD = idHD;
                hOADON.NgayLap = DateTime.Now;
                hOADON.HD_MaKH = null;
                hOADON.HD_MaKH = null;
                hOADON.TongThanhToan = null;
                hOADON.HD_MaTT = null;
                db.HOADONs.Add(hOADON);
                db.SaveChanges();
                return RedirectToAction("ThemHoaDon", "HOADON", new { id = idHD });
            }
            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", hOADON.HD_MaTN);
            return View(hOADON);
        }

        //--------------------------Tạo hoá đơn từ báo giá-------------------------------\\

        // GET: HOADON/Create
        public ActionResult ThemHoaDon_PhieuBG(String idBG)
        {
            var bg = db.BAOGIAs.FirstOrDefault(m=>m.MaBG.Equals(idBG));

            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;

            String idTN = TaoMaHoaDon();
            ViewBag.MaHD = idTN;

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(bg.BG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaHD"] = TaoMaHoaDon();
            Session["MaTN"] = bg.BG_MaTN;
            Session["MaKH"] = TTTN.MaKH;
            Session["MaBG"] = idBG;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            Session["KhachHang"] = kh;
            Session["BienSoXe"] = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            Session["NhanVien"] = nv;

            ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
            ViewBag.KhachHang = Session["KhachHang"];
            ViewBag.BienSoXe = Session["BienSoXe"];
            ViewBag.NhanVien = Session["NhanVien"];

            ViewBag.MaBG = idBG;
            return View();
        }

        //Hiển thị danh sách CTHD
        public ActionResult Partial_TaoHD_PhieuBG_LayChiTietHoaDon(String id)
        {
            List<THONGTINSANPHAM> lstSp;
            if (Session["c"] == null)
            {
                var cthd = db.CT_BAOGIA.Where(m => m.CTBG_MaBG.Equals(id)).ToList();
                lstSp = CTHD_LayDanhSachSanPham();
                for (int i = 0; i < cthd.Count; i++)
                {
                    THONGTINSANPHAM sp = new THONGTINSANPHAM(cthd[i].CTBG_MaHH, cthd[i].SoLuong);
                    lstSp.Add(sp);
                }
                Session["c"] = 3;
            }
            else
            {
                lstSp = CTHD_LayDanhSachSanPham();
            }
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(lstSp);
        }

        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_TaoHD_PhieuBG_ThemChiTietHoaDon()
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTmp > 0).ToList();
            ViewBag.CTHD_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_TaoHD_PhieuBG_ThemChiTietHoaDon(CT_HOADON hh)
        {
                var lstHH = db.HANGHOAs.Where(m => m.SoLuongTmp > 0).ToList();
                ViewBag.CTHD_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
                ViewBag.Selected = hh.CTHD_MaHH;
                Session["MaHH"] = hh.CTHD_MaHH;
                return RedirectToAction("TaoHD_PhieuBG_ThemSP", "HOADON", new { id = Session["MaHH"].ToString() });
        }
        //Thêm một sản phẩm vào CTHD
        public ActionResult TaoHD_PhieuBG_ThemSP(String id)
        {
            List<THONGTINSANPHAM> lstSPHD = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM currentProduct = lstSPHD.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new THONGTINSANPHAM(id);
                lstSPHD.Add(currentProduct);
            }
            else
            {
                currentProduct.SoLuong++;
            }
            return RedirectToAction("ThemHoaDon_PhieuBG", "HOADON", new {idBG = Session["MaBG"]});
        }

        //Xoá sản phẩm khỏi 
        public ActionResult TaoHD_PhieuBG_XoaSP(String id)

        {
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("ThemHoaDon_PhieuBG", "HOADON", new { idBG = Session["MaBG"] });
            }
            return RedirectToAction("ThemHoaDon_PhieuBG", "HOADON", new { idBG = Session["MaBG"] });
        }


        // Cập nhật lại số lượng sản phẩm
        public ActionResult TaoHD_PhieuBG_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult ThemHoaDon_PhieuBG(HOADON hd)
        {
            var lstSPHD = Session["lstSPHD"] as List<THONGTINSANPHAM>;
            if (lstSPHD.Count() != 0)
            {
                var tongtien = TinhTongTien();
                List<THONGTINSANPHAM> lstSP = CTHD_LayDanhSachSanPham();
                if (ModelState.IsValid)
                {
                    hd.MaHD = Session["MaHD"].ToString();
                    hd.NgayLap = DateTime.Now;
                    hd.TongThanhToan = tongtien;
                    hd.HD_BienSoXe = Session["BienSoXe"].ToString();
                    hd.HD_MaKH = Session["MaKH"].ToString();
                    hd.HD_MaTN = Session["MaTN"].ToString();
                    hd.HD_MaTT = null;
                    db.HOADONs.Add(hd);

                    //Tính điểm thành viên
                    String maKH = Session["MaKH"].ToString();
                    KHACHHANG kHACHHANG = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(maKH));
                    int diemThanhVien = (int)tongtien * 10 / 100;

                    if (kHACHHANG.DiemThanhVien == null)
                        kHACHHANG.DiemThanhVien = diemThanhVien;
                    else kHACHHANG.DiemThanhVien = kHACHHANG.DiemThanhVien + (int)diemThanhVien;

                    kHACHHANG.MaKH = maKH;
                    kHACHHANG.HoTenKH = "";
                    kHACHHANG.DienThoaiKH = "";
                    kHACHHANG.NgaySinh = DateTime.Now;
                    kHACHHANG.GioiTinh = "";
                    kHACHHANG.Email = "";
                    kHACHHANG.DiaChi = "";

                    db.KHACHHANGs.Attach(kHACHHANG);
                    db.Entry(kHACHHANG).Property(s => s.DiemThanhVien).IsModified = true;

                    db.SaveChanges();

                    foreach (var item in lstSP)
                    {
                        var details = new CT_HOADON();
                        details.CTHD_MaHD = Session["MaHD"].ToString();
                        details.SoLuong = item.SoLuong;
                        details.CTHD_MaHH = item.MaSP;
                        details.ThanhTien = item.FinalPrice();
                        db.CT_HOADON.Add(details);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("LayDanhSachHoaDon", "HOADON");
            }
            else
            {
                return RedirectToAction("ThemHoaDon_PhieuBG", "HOADON");
            }
        }

        //--------------------------DANH SÁCH HOÁ ĐƠN-------------------------------\\

        // GET: HOADON

        //--------------------------XEM CHI TIẾT HOÁ ĐƠN-------------------------------\\

        // GET: HOADON/Details/5
     

        public ActionResult Partial_CapNhatHD_LayChiTietHoaDon(string id)
        {
            List<THONGTINSANPHAM> lstSp;
            if (Session["c"] == null)
            {
                var ctdh = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
                lstSp = CTHD_LayDanhSachSanPham();
                for (int i = 0; i < ctdh.Count; i++)
                {
                    THONGTINSANPHAM sp = new THONGTINSANPHAM(ctdh[i].CTHD_MaHH, ctdh[i].SoLuong);
                    lstSp.Add(sp);
                }
                Session["c"] = 3;
            }
            else
            {
                lstSp = CTHD_LayDanhSachSanPham();

            }
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(lstSp);
        }
        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_CapNhatHD_ThemChiTietHoaDon()
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTmp > 0).ToList();
            ViewBag.CTHD_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_CapNhatHD_ThemChiTietHoaDon(CT_HOADON hh)
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTmp > 0).ToList();
            ViewBag.CTHD_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            ViewBag.Selected = hh.CTHD_MaHH;
            Session["MaHH"] = hh.CTHD_MaHH;
            return RedirectToAction("SuaHoaDon_ThemSP", "HOADON", new { id = Session["MaHH"].ToString() });
        }
        //Thêm một sản phẩm vào CTHD
        public ActionResult SuaHoaDon_ThemSP(String id)
        {
            List<THONGTINSANPHAM> lstSPHD = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM currentProduct = lstSPHD.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new THONGTINSANPHAM(id);
                lstSPHD.Add(currentProduct);
            }
            else
            {
                currentProduct.SoLuong++;
            }
            return RedirectToAction("SuaHoaDon", "HOADON", new { id = Session["MaHD"] });
        }
        //Xoá sản phẩm khỏi CTHD
        public ActionResult CapNhatHD_XoaSP(String id)

        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("SuaHoaDon", "HOADON", new { id = Session["MaHD"] });
            }
            return RedirectToAction("SuaHoaDon", "HOADON", new { id = Session["MaHD"] });

        }

        // Cập nhật lại số lượng sản phẩm
        public ActionResult CapNhatHD_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return RedirectToAction("SuaHoaDon", new { id = Session["MaHD"] });
        }





        // GET: HOADON/SuaHoaDon/5
        public ActionResult SuaHoaDon(string id)
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
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;
            ViewBag.MaHD = id;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin");

            //Lấy ra thông tin tiếp nhận
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(hOADON.HD_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;
            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaHD"] = id;
            Session["MaTN"] = hOADON.HD_MaTN;
            Session["MaKH"] = TTTN.MaKH;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            Session["KhachHang"] = kh;
            Session["BienSoXe"] = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            Session["NhanVien"] = nv;

            ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
            ViewBag.KhachHang = Session["KhachHang"];
            ViewBag.BienSoXe = Session["BienSoXe"];
            ViewBag.NhanVien = Session["NhanVien"];

            return View(hOADON);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hd)
        {
            if (ModelState.IsValid)
            {
                var tongtien = TinhTongTien();
                String id= Session["MaHD"].ToString();
                List<THONGTINSANPHAM> lstSP = CTHD_LayDanhSachSanPham();
                if (ModelState.IsValid)
                {
                    hd.MaHD = id;
                    hd.NgayLap = DateTime.Now;
                    hd.TongThanhToan = tongtien;
                    hd.HD_BienSoXe = Session["BienSoXe"].ToString();
                    hd.HD_MaKH = Session["MaKH"].ToString();
                    hd.HD_MaTN = Session["MaTN"].ToString();
                    hd.HD_MaTT = null;
                    db.HOADONs.Attach(hd);
                    db.Entry(hd).Property(s => s.TongThanhToan).IsModified = true;
                    db.SaveChanges();

                    bool check;
                    List<CT_HOADON> cthd = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
                    if (cthd != null)
                    {
                        for (int i = 0; i < cthd.Count(); i++)
                        {
                            check = XoaChiTietHD(cthd[i].CTHD_MaHD);
                        }
                    }
                    foreach (var item in lstSP)
                    {
                        var details = new CT_HOADON();
                        details.CTHD_MaHD = Session["MaHD"].ToString();
                        details.SoLuong = item.SoLuong;
                        details.CTHD_MaHH = item.MaSP;
                        details.ThanhTien = item.FinalPrice();
                         db.CT_HOADON.Add(details);

                        //Tính số lượng tồn kho
                        HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(item.MaSP));
                        int slTon = (int)hANGHOA.SoLuongTmp;
                        int slTonMoi = (int)(slTon - item.SoLuong);
                        if (slTonMoi < 0)
                        {
                            hANGHOA.SoLuongTmp = 0;
                        }
                        else hANGHOA.SoLuongTmp = slTonMoi;
                        hANGHOA.DonGia = 0;
                        hANGHOA.MaHH = item.MaSP;
                        hANGHOA.DonViTinh = "";
                        hANGHOA.LoaiHang = "";
                        hANGHOA.HH_MaNCC = "";
                        hANGHOA.HinhAnh = "";
                        hANGHOA.TenHH = "";
                        db.HANGHOAs.Attach(hANGHOA);
                        db.Entry(hANGHOA).Property(s => s.SoLuongTmp).IsModified = true;

                        db.SaveChanges();
                    }
                    String maKH = Session["MaKH"].ToString(); 
                    KHACHHANG kHACHHANG= db.KHACHHANGs.FirstOrDefault(m=>m.MaKH.Equals(maKH));
                    int diemThanhVien = (int)tongtien * 10 / 100;
                    
                    if (kHACHHANG.DiemThanhVien == null)
                        kHACHHANG.DiemThanhVien = diemThanhVien;
                    else kHACHHANG.DiemThanhVien = kHACHHANG.DiemThanhVien + (int)diemThanhVien;

                    kHACHHANG.MaKH = maKH;
                    kHACHHANG.HoTenKH = "A";
                    kHACHHANG.DienThoaiKH = "A";
                    kHACHHANG.NgaySinh = (DateTime)DateTime.Now;
                    kHACHHANG.GioiTinh = "A";
                    kHACHHANG.Email = "A";
                    kHACHHANG.DiaChi = "A";

                    db.KHACHHANGs.Attach(kHACHHANG);
                    db.Entry(kHACHHANG).Property(s => s.DiemThanhVien).IsModified = true;
                    db.SaveChanges();
                }
                Session.Remove("MaHD");
                Session.Remove("MaKH");
                Session.Remove("MaTN");
                Session.Remove("BienSoXe");
                Session.Remove("CheckTN");
                Session.Remove("lstSPHD");
                Session.Remove("c");
                return RedirectToAction("SuaHoaDon");
            }
            
            return View(hd);
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
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(hOADON.HD_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan = selectedTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            ViewBag.KhachHang = kh;
            ViewBag.BienSoXe = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            return View(hOADON);
        }

        // POST: HOADON/Delete/5
        [HttpPost, ActionName("XoaHoaDon")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check;

            //Điểm thành viên
            String maKH = db.HOADONs.FirstOrDefault(m=>m.MaHD.Equals(id)).HD_MaKH;
            KHACHHANG kHACHHANG = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(maKH));
            decimal tongTien = (decimal) db.HOADONs.Find(id).TongThanhToan;
            int diemTV = (int) tongTien * 10 / 100;
            int diemMoi = (int)(kHACHHANG.DiemThanhVien - diemTV);
            if (kHACHHANG.DiemThanhVien<=0 || kHACHHANG.DiemThanhVien == null||diemMoi<=0)
            {
                kHACHHANG.DiemThanhVien = 0;
            }
            else kHACHHANG.DiemThanhVien -= (int)diemTV;
            kHACHHANG.MaKH =maKH ;
            kHACHHANG.HoTenKH ="";
            kHACHHANG.DienThoaiKH = "";
            kHACHHANG.NgaySinh =DateTime.Now;
            kHACHHANG.GioiTinh = "";
            kHACHHANG.Email = "";
            kHACHHANG.DiaChi = "";
            db.KHACHHANGs.Attach(kHACHHANG);
            db.Entry(kHACHHANG).Property(s => s.DiemThanhVien).IsModified = true;
            HOADON hOADON = db.HOADONs.Find(id);


            THONGTINTIEPNHAN TTTN = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(hOADON.HD_MaTN));
            TTTN.TrangThai = "Chưa hoàn thành";
            TTTN.MaTN = hOADON.HD_MaTN;
            TTTN.TN_MaNV = "";
            TTTN.TN_MaKH = "";
            TTTN.TN_BienSoXe = "";
            TTTN.ThoiGianGiaoDuKien = DateTime.Now;
            TTTN.ThoiGianTiepNhan = DateTime.Now;
            TTTN.GhiChuKH = "";
            db.THONGTINTIEPNHANs.Attach(TTTN);
            db.Entry(TTTN).Property(s => s.TrangThai).IsModified = true;

            List<CT_HOADON> cthd = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietHD(cthd[i].CTHD_MaHD);
                }
            }
                db.HOADONs.Remove(hOADON);
                db.SaveChanges();
            return RedirectToAction("LayDanhSachHoaDon");
        }

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
    }
}
