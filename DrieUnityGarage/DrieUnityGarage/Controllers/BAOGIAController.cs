using DrieUnityGarage.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace DrieUnityGarage.Controllers
{
    public class BAOGIAController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        //--------------------------CHI TIẾT BÁO GIÁ-------------------------------\\
        //Hàm để lấy danh sách CTBG hiện tại
        public List<THONGTINSANPHAM> CTBG_LayDanhSachSanPham()
        {
            List<THONGTINSANPHAM> lstSPBG = Session["lstSPBG"] as List<THONGTINSANPHAM>;
            //Nếu CTBG chưa tồn tại thì tạo mới và đưa vào Session
            if (lstSPBG == null)    
            {
                lstSPBG = new List<THONGTINSANPHAM>();
                Session["lstSPBG"] = lstSPBG;
            }
            return lstSPBG;
        }

        //Thêm một sản phẩm vào CTHD
        public ActionResult CTBG_ThemSP(String id)
        {
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> lstSPBG = CTBG_LayDanhSachSanPham();
            THONGTINSANPHAM currentProduct = lstSPBG.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new THONGTINSANPHAM(id);
                lstSPBG.Add(currentProduct);
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
            return RedirectToAction("ThemBaoGia", "BAOGIA");
        }

        //Tính tổng số lượng sản phẩm
        private int TinhTongSoLuong()
        {
            int totalNumber = 0;
            List<THONGTINSANPHAM> lstSPBG = CTBG_LayDanhSachSanPham();
            if (lstSPBG != null)
                totalNumber = lstSPBG.Sum(sp => sp.SoLuong);
            return totalNumber;
        }


        //Tính tổng tiền của các sản phẩm
        private decimal TinhTongTien()
        {
            decimal totalPrice = 0;
            List<THONGTINSANPHAM> lstSPBG = CTBG_LayDanhSachSanPham();
            if (lstSPBG != null)
                totalPrice = lstSPBG.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }


        //Xoá sản phẩm khỏi CTBG
        public ActionResult CTBG_XoaSP(String id)

        {
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> myCart = CTBG_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("ThemBaoGia", "BAOGIA", new { id = Session["MaBG"] });
            }
            return RedirectToAction("ThemBaoGia", "BAOGIA", new { id = Session["MaBG"] });

        }
        // Cập nhật lại số lượng sản phẩm
        public ActionResult CTBG_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = CTBG_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return Redirect(Request.UrlReferrer.ToString());
        }


        //--------------------------VIEW ThemBaoGia-------------------------------\\

        //Hiển thị danh sách CTHD
        public ActionResult Partial_TaoBG_LayChiTietBaoGia()
        {
            List<THONGTINSANPHAM> myCart = CTBG_LayDanhSachSanPham();
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(myCart);
        }

        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_TaoBG_ThemChiTietBaoGia()
        {
            ViewBag.CTBG_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_TaoBG_ThemChiTietBaoGia(CT_BAOGIA bg)
        {
            if (Session["DaLayThongTinTiepNhan"] == null)
                return RedirectToAction("ThemBaoGia", "BAOGIA");
            else
            {
                ViewBag.CTBG_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
                ViewBag.Selected = bg.CTBG_MaHH;
                Session["MaHH"] = bg.CTBG_MaHH;
                return RedirectToAction("CTBG_ThemSP", "BAOGIA", new { id = Session["MaHH"].ToString() });
            }

        }


        [HttpPost]
        public ActionResult CTBG_ThemBaoGia(BAOGIA bg, [Bind(Include = "MaKH,HoTenKH,DienThoaiKH,NgaySinh,GioiTinh,Email,DiemThanhVien,DiaChi")] KHACHHANG kHACHHANG)
        {
            var tongtien = TinhTongTien();
            List<THONGTINSANPHAM> lstSP = CTBG_LayDanhSachSanPham();
            if (ModelState.IsValid)
            {
                bg.MaBG = Session["MaBG"].ToString();

                bg.NgayLap = DateTime.Now;
                bg.BG_MaKH = Session["MaKH"].ToString();
                bg.BG_BienSoXe = Session["BienSoXe"].ToString();
                bg.TongThanhToan = tongtien;
                bg.BG_MaTN = Session["MaTN"].ToString();
                
                db.BAOGIAs.Add(bg);
                db.SaveChanges();

                foreach (var item in lstSP)
                {
                    var details = new CT_BAOGIA();
                    details.CTBG_MaBG = Session["MaBG"].ToString();
                    details.SoLuong = item.SoLuong;
                    details.CTBG_MaHH = item.MaSP;
                    details.ThanhTien = item.FinalPrice();
                    db.CT_BAOGIA.Add(details);
                    db.SaveChanges();
                }

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
            Session.Remove("MaBG");
            Session.Remove("MaKH");
            Session.Remove("MaTN");
            Session.Remove("BienSoXe");
            Session.Remove("CheckTN");
            Session.Remove("lstSPBG");

            return RedirectToAction("LayDanhSachBaoGia", "BAOGIA");
        }
        // GET: HOADON
        public ActionResult LayDanhSachBaoGia()
        {
            Session.Remove("DaLayThongTinTiepNhan");
            Session.Remove("c");
            Session.Remove("lstSPBG");

            var bAOGIAs = db.BAOGIAs.Include(h => h.KHACHHANG).Include(h => h.PHUONGTIEN).Include(h => h.THONGTINTIEPNHAN);
            return View(bAOGIAs.ToList());
        }


        // GET: HOADON/Details/5
        public ActionResult XemThongTinBaoGia(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOGIA bAOGIA = db.BAOGIAs.Find(id);
            if (bAOGIA == null)
            {
                return HttpNotFound();
            }

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(bAOGIA.BG_MaTN);

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

            return View(bAOGIA);
        }

        public ActionResult Partial_XemBG_LayDuLieuCTBG(String id)
        {
            var ctbg = db.CT_BAOGIA.Where(m => m.CTBG_MaBG.Equals(id)).ToList();
            List<THONGTINSANPHAM> lstHH = new List<THONGTINSANPHAM>();
            for (int i = 0; i < ctbg.Count(); i++)
            {
                lstHH.Add(new THONGTINSANPHAM(ctbg[i].CTBG_MaHH, ctbg[i].SoLuong));
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


        // GET: HOADON/Create
        public ActionResult ThemBaoGia()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapBG = date;
            String idTN = TaoMaBaoGia();
            ViewBag.MaBG = idTN;

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
        public ActionResult TaoBG_LayThongTinTiepNhan(String lstMaTN/*, DateTime thoiGianGiaoXe*/)
        {
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(lstMaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaBG"] = TaoMaBaoGia();
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

            return RedirectToAction("ThemBaoGia");
        }

        // POST: HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ThemBaoGia([Bind(Include = "MaBG,NgayLap,BG_MaKH,BG_BienSoXe,TongThanhToan,BG_MaTN")] BAOGIA bAOGIA)
        {
            if (ModelState.IsValid)
            {
                String idBG = TaoMaBaoGia();
                String date = String.Format("{0:dd/MM/yy}", DateTime.Now.ToString());
                bAOGIA.MaBG = idBG;
                bAOGIA.NgayLap = DateTime.Now;
                bAOGIA.BG_MaKH = null;
                bAOGIA.BG_MaKH = null;
                bAOGIA.TongThanhToan = null;
                db.BAOGIAs.Add(bAOGIA);
                db.SaveChanges();
                return RedirectToAction("ThemBaoGia", "BAOGIA", new { id = idBG });
            }
            ViewBag.BG_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", bAOGIA.BG_MaTN);
            return View(bAOGIA);
        }



        public ActionResult Partial_CapNhatBG_LayChiTietBaoGia(string id)
        {
            List<THONGTINSANPHAM> lstSp;
            if (Session["c"] == null)
            {
                var ctbg = db.CT_BAOGIA.Where(m => m.CTBG_MaBG.Equals(id)).ToList();
                lstSp = CTBG_LayDanhSachSanPham();
                for (int i = 0; i < ctbg.Count; i++)
                {
                    THONGTINSANPHAM sp = new THONGTINSANPHAM(ctbg[i].CTBG_MaHH, ctbg[i].SoLuong);
                    lstSp.Add(sp);
                }
                Session["c"] = 3;
            }
            else
            {
                lstSp = CTBG_LayDanhSachSanPham();

            }
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(lstSp);
        }


        // GET: HOADON/Edit/5
        public ActionResult SuaBaoGia(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOGIA bAOGIA = db.BAOGIAs.Find(id);
            if (bAOGIA == null)
            {
                return HttpNotFound();
            }
            //Lấy thông tin tiếp nhận
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapBG = date;
            ViewBag.MaBG = id;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin");

            //Lấy ra thông tin tiếp nhận
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(bAOGIA.BG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;


            Session["selectedTiepNhan"] = selectedTN;


            //Thông tin cần lưu của tiếp nhận
            Session["MaBG"] = id;
            Session["MaTN"] = bAOGIA.BG_MaTN;
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

            return View(bAOGIA);
        }

        //Xoá sản phẩm khỏi CTHD
        public ActionResult CapNhatBG_XoaSP(String id)

        {
            List<THONGTINSANPHAM> myCart = CTBG_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("SuaBaoGia", "BAOGIA", new { id = Session["MaBG"] });
            }
            return RedirectToAction("SuaBaoGia", "BAOGIA", new { id = Session["MaBG"] });
        }

        // Cập nhật lại số lượng sản phẩm
        public ActionResult CapNhatBG_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = CTBG_LayDanhSachSanPham();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return RedirectToAction("SuaBaoGia", new { id = Session["MaBG"] });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaBaoGia([Bind(Include = "MaBG,NgayLap,BG_MaKH,BG_BienSoXe,TongThanhToan,BG_MaTN")] BAOGIA bg)
        {
            if (ModelState.IsValid)
            {
                var tongtien = TinhTongTien();
                List<THONGTINSANPHAM> lstSP = CTBG_LayDanhSachSanPham();
                if (ModelState.IsValid)
                {
                    bg.MaBG = Session["MaBG"].ToString();
                    bg.NgayLap = DateTime.Now;
                    bg.TongThanhToan = tongtien;
                    bg.MaBG = Session["MaBG"].ToString();
                    bg.BG_MaKH = Session["MaKH"].ToString();
                    bg.BG_MaTN = Session["MaTN"].ToString();
                    db.BAOGIAs.Attach(bg);
                    db.Entry(bg).Property(s => s.TongThanhToan).IsModified = true;
                    db.SaveChanges();

                    foreach (var item in lstSP)
                    {
                        var details = new CT_BAOGIA();
                        details.CTBG_MaBG = Session["MaBG"].ToString();
                        details.SoLuong = item.SoLuong;
                        details.CTBG_MaHH = item.MaSP;
                        details.ThanhTien = item.FinalPrice();
                        db.CT_BAOGIA.Attach(details);
                        db.Entry(details).Property(s => s.SoLuong).IsModified = true;
                        db.Entry(details).Property(s => s.ThanhTien).IsModified = true;
                        db.SaveChanges();
                    }
                    KHACHHANG kHACHHANG = new KHACHHANG();
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
                Session.Remove("MaBG");
                Session.Remove("MaKH");
                Session.Remove("BienSoXe");
                Session.Remove("CheckTN");
                Session.Remove("lstSPBG");
                Session.Remove("c");
                return RedirectToAction("SuaBaoGia");
            }

            return View(bg);
        }


        // GET: HOADON/Delete/5
        public ActionResult XoaBaoGia(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOGIA bAOGIA = db.BAOGIAs.Find(id);
            if (bAOGIA == null)
            {
                return HttpNotFound();
            }
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(bAOGIA.BG_MaTN);

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

            return View(bAOGIA);
        }

        // POST: HOADON/Delete/5
        [HttpPost, ActionName("XoaBaoGia")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check;
            List<CT_BAOGIA> ctbg = db.CT_BAOGIA.Where(m => m.CTBG_MaBG.Equals(id)).ToList();
            if (ctbg != null)
            {
                for (int i = 0; i < ctbg.Count(); i++)
                {
                    check = XoaChiTietBG(ctbg[i].CTBG_MaBG);
                }
            }


            BAOGIA bAOGIA = db.BAOGIAs.Find(id);
            db.BAOGIAs.Remove(bAOGIA);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachBaoGia");


        }

        public bool XoaChiTietBG(string id)
        {
            CT_BAOGIA cT_BAOGIA = db.CT_BAOGIA.FirstOrDefault(m => m.CTBG_MaBG.Equals(id));
            db.CT_BAOGIA.Remove(cT_BAOGIA);
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

        private String TaoMaBaoGia()
        {
            String idBG = "";
            //Tạo mã nhà cung cấp String
            List<BAOGIA> lstBG = db.BAOGIAs.ToList();
            int countLst = lstBG.Count();
            if (countLst == 0)
            {
                idBG = "BG001";
            }
            else
            {
                BAOGIA lastBG = lstBG[countLst - 1];
                String lastMaBG = lastBG.MaBG;
                int lastMaBGNum = int.Parse(lastMaBG.Substring(2));
                int newMaBG = lastMaBGNum + 1;
                if (newMaBG < 10)
                {
                    idBG = "BG00" + newMaBG.ToString();
                }
                else { idBG = "BG0" + newMaBG.ToString(); }
            }
            return (idBG);
        }

    }
}