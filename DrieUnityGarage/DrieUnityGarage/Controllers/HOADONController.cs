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
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> lstSPHD = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM currentProduct = lstSPHD.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new THONGTINSANPHAM(id);
                lstSPHD.Add(currentProduct);
            }
            else
            {
                if (currentProduct.TonKho == currentProduct.SoLuong)
                {
                    Session["QuaTonKho"] = 3;
                }
                else
                    currentProduct.SoLuong++;
            }
            return RedirectToAction("TaoHoaDon", "HOADON");
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
                return RedirectToAction("TaoHoaDon", "HOADON", new { id = Session["MaHD"] });
            }
            return RedirectToAction("TaoHoaDon", "HOADON", new { id = Session["MaHD"] });

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

        //--------------------------VIEW TAOHOADON-------------------------------\\

        //Hiển thị danh sách CTHD
        public ActionResult Partial_LayChiTietHoaDonList()
        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
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
            if (Session["DaLayThongTinTiepNhan"] ==null)
                return RedirectToAction("TaoHoaDon", "HOADON");
            else
            {
                ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
                ViewBag.Selected = hh.CTHD_MaHH;
                Session["MaHH"] = hh.CTHD_MaHH;
                return RedirectToAction("CTHD_ThemSP", "HOADON", new { id = Session["MaHH"].ToString() });
            }
           
        }
 
        [HttpPost]
        public ActionResult CTDH_TaoHoaDon(HOADON hd, [Bind(Include = "MaKH,HoTenKH,DienThoaiKH,NgaySinh,GioiTinh,Email,DiemThanhVien,DiaChi")] KHACHHANG kHACHHANG)
        {
            var tongtien = TinhTongTien();
            List<THONGTINSANPHAM> lstSP = CTHD_LayDanhSachSanPham();
            if (ModelState.IsValid)
            {
                hd.MaHD = Session["MaHD"].ToString();
                
                hd.NgayLap = DateTime.Now;
               
                hd.TongThanhToan = tongtien;
                hd.HD_BienSoXe =Session["BienSoXe"].ToString();
                hd.HD_MaKH = Session["MaKH"].ToString();
                hd.HD_MaTN =Session["MaTN"].ToString();
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

                int diemThanhVien =(int) tongtien * 10 / 100;
                if (kHACHHANG.DiemThanhVien == null)
                    kHACHHANG.DiemThanhVien = diemThanhVien;
                else kHACHHANG.DiemThanhVien += (int)diemThanhVien;
                kHACHHANG.MaKH= Session["MaKH"].ToString(); ;
                kHACHHANG.HoTenKH = "A";
                kHACHHANG.DienThoaiKH = "A";
                kHACHHANG.NgaySinh = (DateTime)DateTime.Now;
                kHACHHANG.GioiTinh = "A";
                kHACHHANG.Email = "A";
                kHACHHANG.DiaChi= "A";
                
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

            return RedirectToAction("LayDanhSachHoaDon", "HOADON");
        }
    // GET: HOADON
        public ActionResult LayDanhSachHoaDon()
        {
            Session.Remove("DaLayThongTinTiepNhan");
                Session.Remove("c");
            Session.Remove("lstSPHD");

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
        //Xoá sản phẩm khỏi CTHD
        public ActionResult CapNhatHD_XoaSP(String id)

        {
            List<THONGTINSANPHAM> myCart = CTHD_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("CapNhatHoaDon", "HOADON", new { id = Session["MaHD"] });
            }
            return RedirectToAction("CapNhatHoaDon", "HOADON", new { id = Session["MaHD"] });

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
            return RedirectToAction("CapNhatHoaDon", new { id = Session["MaHD"] });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatHoaDon([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hd)
        {
            if (ModelState.IsValid)
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
                    db.HOADONs.Attach(hd);
                    db.Entry(hd).Property(s => s.TongThanhToan).IsModified = true;
                    db.SaveChanges();

                    foreach (var item in lstSP)
                    {
                        var details = new CT_HOADON();
                        details.CTHD_MaHD = Session["MaHD"].ToString();
                        details.SoLuong = item.SoLuong;
                        details.CTHD_MaHH = item.MaSP;
                        details.ThanhTien = item.FinalPrice();
                        db.CT_HOADON.Attach(details);
                        db.Entry(details).Property(s => s.SoLuong).IsModified = true;
                        db.Entry(details).Property(s => s.ThanhTien).IsModified = true;
                        db.SaveChanges();
                    }
                    KHACHHANG kHACHHANG=new KHACHHANG();
                    int diemThanhVien = (int)tongtien * 10 / 100;
                    if (kHACHHANG.DiemThanhVien == null)
                        kHACHHANG.DiemThanhVien = diemThanhVien;
                    else kHACHHANG.DiemThanhVien += (int)diemThanhVien;
                    kHACHHANG.MaKH = Session["MaKH"].ToString(); ;
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
                return RedirectToAction("CapNhatHoaDon");
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
