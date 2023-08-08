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
    public class NHAPKHOController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        public List<THONGTINNHACUNGCAP> LayDanhSachNhaCungCapDB()
        {
            var newlstNCC = new List<THONGTINNHACUNGCAP>();
            var nccDB = db.NHACUNGCAPs.ToList();
            int c = nccDB.Count();
            for (int i = 0; i < c; i++)
            {
                if (nccDB[i].MaNCC.Equals("NCC001"))
                    continue;
                else
                newlstNCC.Add(new THONGTINNHACUNGCAP(nccDB[i].MaNCC));
            }
            Session["lstNCC"] = newlstNCC;
            return newlstNCC;
        }


        // GET: NHAPKHO
        public ActionResult LayDanhSachPhieuNhapKho()
        {
            Session.Remove("LayThongTin");
            Session.Remove("lstNCC");
            Session.Remove("lstSP");
            Session.Remove("c");

            var nHAPKHOes = db.NHAPKHOes.Include(n => n.NHACUNGCAP).Include(n => n.NHANVIEN);
            return View(nHAPKHOes.ToList());
        }
        //----------------------------------------CHI TIẾT PHIẾU NHẬP KHO----------------------------------\\

        // GET: NHAPKHO/LayThongTinPhieuNhapKho/5
        public ActionResult LayThongTinPhieuNhapKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAPKHO nHAPKHO = db.NHAPKHOes.Find(id);
            if (nHAPKHO == null)
            {
                return HttpNotFound();
            }
            Session["MaNK"] = nHAPKHO.MaNK;
            ViewBag.NgayLap = ((DateTime)nHAPKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINNHACUNGCAP TTNCC = new THONGTINNHACUNGCAP(nHAPKHO.NK_MaNCC);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedNCC = TTNCC.MaNCC + " - " + TTNCC.TenNCC;

            ViewBag.selectedNCC = selectedNCC;

            return View(nHAPKHO);
        }
        public ActionResult Partial_XemNK_LayDuLieuCTNK(String id)
        {
            var cthd = db.CT_NHAPKHO.Where(m => m.CTNK_MaNK.Equals(id)).ToList();
            List<THONGTINSANPHAM> lstHH = new List<THONGTINSANPHAM>();
            for (int i = 0; i < cthd.Count(); i++)
            {
                lstHH.Add(new THONGTINSANPHAM(cthd[i].CTNK_MaHH, cthd[i].SoLuongThucNhap));
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


        //----------------------------------------TẠO PHIẾU NHẬP KHO----------------------------------\\
        // GET: NHAPKHO/TaoPhieuNhapKho
        public ActionResult TaoPhieuNhapKho()
        {
            ViewBag.MaNK = TaoMaPhieuNhapKho();
            ViewBag.MaNV = Session["MaTaiKhoanNV"].ToString();
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            if (Session["LayThongTin"] != null)
            {
                ViewBag.NguoiGiao = Session["NguoiGiao"];
                ViewBag.SoCT = Session["SoCT"];
                

                ViewBag.selectedNCC = Session["selectedNCC"];
            }
            else
            {
                List<THONGTINNHACUNGCAP> lstNhaCungCap = LayDanhSachNhaCungCapDB();
                ViewBag.lstNCC = new SelectList(lstNhaCungCap, "MaNCC", "ThongTin");
            }
            return View();
        }
        public ActionResult TaoNK_LayThongTin(String NguoiGiao, String lstNCC, String SoCT)
        {
            Session["LayThongTin"] = 3;
            Session["NguoiGiao"] = NguoiGiao;
            Session["MaNCC"] = lstNCC;
            Session["SoCT"] = SoCT;
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINNHACUNGCAP TTNCC = new THONGTINNHACUNGCAP(lstNCC);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedNCC = TTNCC.MaNCC + " - " + TTNCC.TenNCC;

            Session["selectedNCC"] = selectedNCC;

            return RedirectToAction("TaoPhieuNhapKho");
        }
        public List<THONGTINSANPHAM> LayDanhSachSanPhamList()
        {
            List<THONGTINSANPHAM> lstSPHD = Session["lstSP"] as List<THONGTINSANPHAM>;
            //Nếu CTDH chưa tồn tại thì tạo mới và đưa vào Session
            if (lstSPHD == null)
            {
                lstSPHD = new List<THONGTINSANPHAM>();
                Session["lstSP"] = lstSPHD;
            }
            return lstSPHD;
        }

        public ActionResult CTNK_ThemSP(String id)
        {
            List<THONGTINSANPHAM> lstSPHD = LayDanhSachSanPhamList();
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
            return RedirectToAction("TaoPhieuNhapKho", "NHAPKHO");
        }

        //Tính tổng số lượng sản phẩm
        private int TinhTongSoLuong()
        {
            int totalNumber = 0;
            List<THONGTINSANPHAM> lstSPHD = LayDanhSachSanPhamList();
            if (lstSPHD != null)
                totalNumber = lstSPHD.Sum(sp => sp.SoLuong);
            return totalNumber;
        }

        //Tính tổng tiền của các sản phẩm
        private decimal TinhTongTien()
        {
            decimal totalPrice = 0;
            List<THONGTINSANPHAM> lstSPHD = LayDanhSachSanPhamList();
            if (lstSPHD != null)
                totalPrice = lstSPHD.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }

        //Xoá sản phẩm khỏi CTHD
        public ActionResult CTNK_XoaSP(String id)

        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("TaoPhieuNhapKho", "NHAPKHO");
            }
            return RedirectToAction("TaoPhieuNhapKho", "NHAPKHO");

        }

        // Cập nhật lại số lượng sản phẩm
        public ActionResult CTNK_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Partial_TaoNK_LayChiTietPhieuNK()
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(myCart);
        }

        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_TaoNK_ThemChiTietPhieuNK()
        {
            var lstHH = db.HANGHOAs.ToList();
            ViewBag.CTNK_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_TaoNK_ThemChiTietPhieuNK(CT_NHAPKHO hh)
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTon > 0).ToList();
            ViewBag.CTNK_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            ViewBag.Selected = hh.CTNK_MaHH;
            Session["MaHH"] = hh.CTNK_MaHH;
            return RedirectToAction("CTNK_ThemSP", "NHAPKHO", new { id = Session["MaHH"].ToString() });
        }

        // POST: NHAPKHO/TaoPhieuNhapKho
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoPhieuNhapKho(NHAPKHO nHAPKHO)
        {
            List<THONGTINSANPHAM> lstNK = LayDanhSachSanPhamList();
            String id = TaoMaPhieuNhapKho();
            if (ModelState.IsValid)
            {
                nHAPKHO.MaNK = id;
                nHAPKHO.NgayLap = DateTime.Now;
                nHAPKHO.NK_MaNCC = Session["MaNCC"].ToString();
                nHAPKHO.HoTenNguoiGiao = Session["NguoiGiao"].ToString();
                nHAPKHO.SoChungTu = Session["SoCT"].ToString();
                nHAPKHO.NK_MaNV = Session["MaTaiKhoanNV"].ToString();
                db.NHAPKHOes.Add(nHAPKHO);
                db.SaveChanges();

                foreach (var item in lstNK)
                {
                    var details = new CT_NHAPKHO();
                    details.CTNK_MaNK = id;
                    details.SoLuongYeuCau = item.SoLuong;
                    details.SoLuongThucNhap = item.SoLuong;
                    details.CTNK_MaHH = item.MaSP;
                    db.CT_NHAPKHO.Add(details);

                    //Tính số lượng tồn kho
                    HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(item.MaSP));
                    int slTon = (int)hANGHOA.SoLuongTon;
                    int slTonMoi = (int)(slTon + item.SoLuong);
                    
                    hANGHOA.SoLuongTon = slTonMoi;
                    hANGHOA.SoLuongTmp = slTonMoi;

                    hANGHOA.DonGia = 0;
                    hANGHOA.MaHH = item.MaSP;
                    hANGHOA.DonViTinh = "";
                    hANGHOA.LoaiHang = "";
                    hANGHOA.HH_MaNCC = "";
                    hANGHOA.HinhAnh = "";
                    hANGHOA.TenHH = "";

                    db.HANGHOAs.Attach(hANGHOA);
                    db.Entry(hANGHOA).Property(s => s.SoLuongTon).IsModified = true;
                    db.Entry(hANGHOA).Property(s => s.SoLuongTmp).IsModified = true;
                    db.SaveChanges();
                }
                return RedirectToAction("LayDanhSachPhieuNhapKho");
            }
            ViewBag.NK_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", nHAPKHO.NK_MaNCC);
            ViewBag.NK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", nHAPKHO.NK_MaNV);
            return View(nHAPKHO);
        }
        //----------------------------------------SỬA PHIẾU NHẬP KHO----------------------------------\\

        public ActionResult Partial_SuaNK_LayChiTietNK(string id)
        {
            List<THONGTINSANPHAM> lstSp;
            if (Session["c"] == null)
            {
                var ctdh = db.CT_NHAPKHO.Where(m => m.CTNK_MaNK.Equals(id)).ToList();
                lstSp = LayDanhSachSanPhamList();
                for (int i = 0; i < ctdh.Count; i++)
                {
                    THONGTINSANPHAM sp = new THONGTINSANPHAM(ctdh[i].CTNK_MaHH, ctdh[i].SoLuongThucNhap);
                    lstSp.Add(sp);
                }
                Session["c"] = 3;
            }
            else
            {
                lstSp = LayDanhSachSanPhamList();
            }
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(lstSp);
        }
        //Xoá sản phẩm khỏi CTHD
        public ActionResult SuaNK_XoaSP(String id)
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("SuaPhieuNhapKho", "NHAPKHO", new { id = Session["MaNK"] });
            }
            return RedirectToAction("SuaPhieuNhapKho", "NHAPKHO", new { id = Session["MaNK"] });
        }
        // Cập nhật lại số lượng sản phẩm
        public ActionResult SuaNK_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return RedirectToAction("SuaPhieuNhapKho", new { id = Session["MaNK"] });
        }

        // GET: NHAPKHO/SuaPhieuNhapKho/5
        public ActionResult SuaPhieuNhapKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAPKHO nHAPKHO = db.NHAPKHOes.Find(id);
            if (nHAPKHO == null)
            {
                return HttpNotFound();
            }
            Session["MaNK"] = nHAPKHO.MaNK;
            ViewBag.NgayLap = ((DateTime)nHAPKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINNHACUNGCAP TTNCC = new THONGTINNHACUNGCAP(nHAPKHO.NK_MaNCC);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedNCC = TTNCC.MaNCC + " - " + TTNCC.TenNCC;

            ViewBag.selectedNCC = selectedNCC;


            return View(nHAPKHO);
        }

        // POST: NHAPKHO/SuaPhieuNhapKho/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaPhieuNhapKho([Bind(Include = "MaNK,NgayLap,NK_MaNCC,HoTenNguoiGiao,SoChungTu,NK_MaNV")] NHAPKHO nHAPKHO)
        {
            String maNK = Session["MaNK"].ToString();
            var a = Session["lstSP"] as List<THONGTINSANPHAM>;
            if (a.Count != 0)
            {
                List<THONGTINSANPHAM> lstSP = LayDanhSachSanPhamList();
                if (ModelState.IsValid)
                {
                    nHAPKHO.MaNK = maNK;
                    nHAPKHO.NgayLap = DateTime.Now;
                    nHAPKHO.NK_MaNCC = "";
                    nHAPKHO.HoTenNguoiGiao = "";
                    nHAPKHO.SoChungTu = "";
                    nHAPKHO.NK_MaNV = "";
                    db.Entry(nHAPKHO).State = EntityState.Modified;
                    db.Entry(nHAPKHO).Property(s => s.NK_MaNCC).IsModified = false;
                    db.Entry(nHAPKHO).Property(s => s.NK_MaNV).IsModified = false;
                    db.Entry(nHAPKHO).Property(s => s.NgayLap).IsModified = false;
                    db.Entry(nHAPKHO).Property(s => s.HoTenNguoiGiao).IsModified = false;
                    db.Entry(nHAPKHO).Property(s => s.SoChungTu).IsModified = false;

                    bool check;
                    List<CT_NHAPKHO> cthd = db.CT_NHAPKHO.Where(m => m.CTNK_MaNK.Equals(maNK)).ToList();
                    if (cthd != null)
                    {
                        for (int i = 0; i < cthd.Count(); i++)
                        {
                            check = XoaChiTietNK(cthd[i].CTNK_MaNK);
                        }
                    }
                    foreach (var item in lstSP)
                    {
                        var details = new CT_NHAPKHO();
                        details.CTNK_MaNK = maNK;
                        details.SoLuongYeuCau = item.SoLuong;
                        details.SoLuongThucNhap = item.SoLuong;
                        details.CTNK_MaHH = item.MaSP;
                        db.CT_NHAPKHO.Add(details);

                        //Tính số lượng tồn kho
                        HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(item.MaSP));
                        int slTon = (int)hANGHOA.SoLuongTon;
                        int slTonMoi = (int)(slTon + item.SoLuong);

                        hANGHOA.SoLuongTon = slTonMoi;
                        hANGHOA.SoLuongTmp = slTonMoi;

                        hANGHOA.DonGia = 0;
                        hANGHOA.MaHH = item.MaSP;
                        hANGHOA.DonViTinh = "";
                        hANGHOA.LoaiHang = "";
                        hANGHOA.HH_MaNCC = "";
                        hANGHOA.HinhAnh = "";
                        hANGHOA.TenHH = "";

                        db.HANGHOAs.Attach(hANGHOA);
                        db.Entry(hANGHOA).Property(s => s.SoLuongTon).IsModified = true;
                        db.Entry(hANGHOA).Property(s => s.SoLuongTmp).IsModified = true;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    return RedirectToAction("LayDanhSachPhieuNhapKho");
                }
                return View(nHAPKHO);
            }
            else
            {
                bool check;
                List<CT_NHAPKHO> cthd = db.CT_NHAPKHO.Where(m => m.CTNK_MaNK.Equals(maNK)).ToList();
                if (cthd != null)
                {
                    for (int i = 0; i < cthd.Count(); i++)
                    {
                        check = XoaChiTietNK(cthd[i].CTNK_MaNK);
                    }
                }

                NHAPKHO nHAPKHO1 = db.NHAPKHOes.Find(maNK);
                db.NHAPKHOes.Remove(nHAPKHO1);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhieuNhapKho");

            }
        }

        // GET: NHAPKHO/XoaPhieuNhapKho/5
        public ActionResult XoaPhieuNhapKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHAPKHO nHAPKHO = db.NHAPKHOes.Find(id);
            if (nHAPKHO == null)
            {
                return HttpNotFound();
            }
            Session["MaNK"] = nHAPKHO.MaNK;
            ViewBag.NgayLap = ((DateTime)nHAPKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINNHACUNGCAP TTNCC = new THONGTINNHACUNGCAP(nHAPKHO.NK_MaNCC);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedNCC = TTNCC.MaNCC + " - " + TTNCC.TenNCC;

            ViewBag.selectedNCC = selectedNCC;


            return View(nHAPKHO);
        }

        // POST: NHAPKHO/XoaPhieuNhapKho/5
        [HttpPost, ActionName("XoaPhieuNhapKho")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check;
            List<CT_NHAPKHO> cthd = db.CT_NHAPKHO.Where(m => m.CTNK_MaNK.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietNK(cthd[i].CTNK_MaNK);
                }
            }
            NHAPKHO nHAPKHO = db.NHAPKHOes.Find(id);
            db.NHAPKHOes.Remove(nHAPKHO);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachPhieuNhapKho");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public bool XoaChiTietNK(string id)
        {
            CT_NHAPKHO cT_NHAPKHO = db.CT_NHAPKHO.FirstOrDefault(m => m.CTNK_MaNK.Equals(id));
            db.CT_NHAPKHO.Remove(cT_NHAPKHO);

            //Tính số lượng tồn kho
            String maSP = cT_NHAPKHO.CTNK_MaHH;
            HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(maSP));
            int slTon = (int)hANGHOA.SoLuongTmp;
            int slTonMoi = (int)(slTon - cT_NHAPKHO.SoLuongThucNhap);
            if (slTonMoi < 0)
            {
                hANGHOA.SoLuongTon = 0;
                hANGHOA.SoLuongTmp = 0;
            }
            else
            {
                hANGHOA.SoLuongTon = slTonMoi;
                hANGHOA.SoLuongTmp = slTonMoi;
            }

            hANGHOA.DonGia = 0;
            hANGHOA.MaHH = maSP;
            hANGHOA.DonViTinh = "";
            hANGHOA.LoaiHang = "";
            hANGHOA.HH_MaNCC = "";
            hANGHOA.HinhAnh = "";
            hANGHOA.TenHH = "";
            db.HANGHOAs.Attach(hANGHOA);
            db.Entry(hANGHOA).Property(s => s.SoLuongTon).IsModified = true;
            db.Entry(hANGHOA).Property(s => s.SoLuongTmp).IsModified = true;
            db.SaveChanges();
            return true;
        }

        private String TaoMaPhieuNhapKho()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<NHAPKHO> lstHD = db.NHAPKHOes.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "NK001";
            }
            else
            {
                NHAPKHO lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaNK;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "NK00" + newMaHD.ToString();
                }
                else { idHD = "NK0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}
