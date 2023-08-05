using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGarage.Models;

namespace DrieUnityGarage.Controllers
{
    public class XUATKHOController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
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

        // GET: XUATKHO
        public ActionResult LayDanhSachPhieuXuatKho()
        {
            Session.Remove("lstSP");
            Session.Remove("c");
            Session.Remove("LyDoXuat");
            Session.Remove("SoCT");
            var xUATKHOes = db.XUATKHOes.Include(x => x.BAOGIA).Include(x => x.NHANVIEN);
            return View(xUATKHOes.ToList());
        }

        // GET: XUATKHO/LayThongTinPhieuXuatKho/5
        public ActionResult LayThongTinPhieuXuatKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XUATKHO xUATKHO = db.XUATKHOes.Find(id);
            if (xUATKHO == null)
            {
                return HttpNotFound();
            }
            Session["MaXK"] = xUATKHO.MaXK;
            ViewBag.NgayLap = ((DateTime)xUATKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");
            return View(xUATKHO);
        }
        public ActionResult Partial_XemXK_LayDuLieuCTXK(String id)
        {
            var cthd = db.CT_XUATKHO.Where(m => m.CTXK_MaXK.Equals(id)).ToList();
            List<THONGTINSANPHAM> lstHH = new List<THONGTINSANPHAM>();
            for (int i = 0; i < cthd.Count(); i++)
            {
                lstHH.Add(new THONGTINSANPHAM(cthd[i].CTXK_MaHH, cthd[i].SoLuongXuatKho));
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


        //--------------------------TẠO PHIẾU XUẤT KHO------------------------ 
        // GET: XUATKHO/TaoPhieuXuatKho
        public ActionResult TaoPhieuXuatKho()

        {
            ViewBag.MaXK = TaoMaPhieuXuatKho();
            ViewBag.MaNV = Session["MaTaiKhoanNV"].ToString();
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            ViewBag.LyDoXuat = Session["LyDoXuat"];
            ViewBag.SoCT = Session["SoCT"];
            return View();
        }

        // POST: XUATKHO/TaoPhieuXuatKho
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more LayThongTinPhieuXuatKho see https://go.microsoft.com/fwlink/?LinkId=317598.        //Thêm một sản phẩm vào CTHD
        public ActionResult CTXK_ThemSP(String id)
        {
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> lstSPHD = LayDanhSachSanPhamList();
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
            return RedirectToAction("TaoPhieuXuatKho", "XUATKHO");
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
        public ActionResult CTXK_XoaSP(String id)

        {
            Session.Remove("QuaTonKho");
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("TaoPhieuXuatKho", "XUATKHO");
            }
            return RedirectToAction("TaoPhieuXuatKho", "XUATKHO");

        }

        // Cập nhật lại số lượng sản phẩm
        public ActionResult CTXK_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Partial_TaoXK_LayChiTietPhieuXK()
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            ViewBag.TotalNumber = TinhTongSoLuong();
            ViewBag.TotalPrice = TinhTongTien();
            return PartialView(myCart);
        }

        //Thêm sản phẩm vào chi tiết hoá đơn
        public ActionResult Partial_TaoXK_ThemChiTietPhieuXK()
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTon > 0).ToList();
            ViewBag.CTXK_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_TaoXK_ThemChiTietPhieuXK(CT_XUATKHO hh, String LyDoXuat, String SoCT)
        {
            var lstHH = db.HANGHOAs.Where(m => m.SoLuongTon > 0).ToList();
            ViewBag.CTXK_MaHH = new SelectList(lstHH, "MaHH", "TenHH");
            ViewBag.Selected = hh.CTXK_MaXK;
                Session["MaHH"] = hh.CTXK_MaHH;
                Session["LyDoXuat"] = LyDoXuat;
                Session["SoCT"] = SoCT;
                return RedirectToAction("CTXK_ThemSP", "XUATKHO", new { id = Session["MaHH"].ToString() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoPhieuXuatKho(String MaXK)
        {
            XUATKHO xUATKHO = new XUATKHO();
            List<THONGTINSANPHAM> lstXK = LayDanhSachSanPhamList();
            String id= TaoMaPhieuXuatKho();
            if (ModelState.IsValid)
            {
                xUATKHO.MaXK = id;
                xUATKHO.XK_MaNV = Session["MaTaiKhoanNV"].ToString();
                xUATKHO.XK_MaBG =null;
                xUATKHO.NgayLap = DateTime.Now;
                xUATKHO.LyDoXuat = Session["LyDoXuat"].ToString();
                xUATKHO.SoChungTu = Session["SoCT"].ToString();
                db.XUATKHOes.Add(xUATKHO);
                db.SaveChanges();

                foreach (var item in lstXK)
                {
                    var details = new CT_XUATKHO();
                    details.CTXK_MaXK = id;
                    details.SoLuongXuatKho = item.SoLuong;
                    details.CTXK_MaHH = item.MaSP;
                    db.CT_XUATKHO.Add(details);

                    //Tính số lượng tồn kho
                    HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(item.MaSP));
                    int slTon = (int)hANGHOA.SoLuongTon;
                    int slTonMoi = (int)(slTon - item.SoLuong);
                    if (slTon == 0 || slTonMoi < 0)
                    {
                        hANGHOA.SoLuongTon = 0;
                        hANGHOA.SoLuongTmp = 0;
                    }
                    else { hANGHOA.SoLuongTon = slTonMoi;
                        hANGHOA.SoLuongTmp = slTonMoi;
                    }

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
                return RedirectToAction("LayDanhSachPhieuXuatKho");
            }

            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH", xUATKHO.XK_MaBG);
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", xUATKHO.XK_MaNV);
            return View(xUATKHO);
        }

        //-------------------------------TẠO PHIẾU TỪ BÁO GIÁ------------------------ (Tạm thời đang test trên hoá đơn)
        //Partial Danh sách hàng hoá
        public ActionResult Partial_TaoXKTuBG_LayChiTietXK(String id)
        {
            var cthd = db.CT_HOADON.Where(m => m.CTHD_MaHD.Equals(id)).ToList();
            List<THONGTINSANPHAM> lstHH = new List<THONGTINSANPHAM>();
            for (int i = 0; i < cthd.Count(); i++)
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

            Session["lstSP"] = lstHH;

            return PartialView(lstHH);
        }

        // GET: XUATKHO/TaoPhieuXuatKho
        public ActionResult TaoPhieuXuatKho_PhieuBG(String idBG)
        {
            ViewBag.MaXK = TaoMaPhieuXuatKho();
            ViewBag.LyDoXuat = "Xuất theo báo giá";
            ViewBag.MaBG = idBG;
            ViewBag.MaNV = Session["MaTaiKhoanNV"].ToString();
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            /*var bg = db.HOADONs.Find(idBG);*/
            return View();
        }

        // POST: XUATKHO/TaoPhieuXuatKho
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoPhieuXuatKho_PhieuBG(String MaXK, String MaNV, String MaBG, String LyDoXuat, String SoCT)
        {
            XUATKHO xUATKHO= new XUATKHO();
            List <THONGTINSANPHAM> lstXK = LayDanhSachSanPhamList();
            if (ModelState.IsValid)
            {
                xUATKHO.MaXK= MaXK;
                xUATKHO.XK_MaNV= MaNV;
                xUATKHO.XK_MaBG= MaBG;
                xUATKHO.NgayLap = DateTime.Now;
                xUATKHO.LyDoXuat = LyDoXuat;
                xUATKHO.SoChungTu= SoCT;
                db.XUATKHOes.Add(xUATKHO);
                db.SaveChanges();

                foreach (var item in lstXK)
                {
                    var details = new CT_XUATKHO();
                    details.CTXK_MaXK = MaXK;
                    details.SoLuongXuatKho = item.SoLuong;
                    details.CTXK_MaHH = item.MaSP;
                    db.CT_XUATKHO.Add(details);

                    //Tính số lượng tồn kho
                    //Tính số lượng tồn kho
                    HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(item.MaSP));
                    int slTon = (int)hANGHOA.SoLuongTmp;
                    hANGHOA.SoLuongTon = slTon;
                    hANGHOA.DonGia = 0;
                    hANGHOA.MaHH = item.MaSP;
                    hANGHOA.DonViTinh = "";
                    hANGHOA.LoaiHang = "";
                    hANGHOA.HH_MaNCC = "";
                    hANGHOA.HinhAnh = "";
                    hANGHOA.TenHH = "";
                    hANGHOA.SoLuongTmp = 0;
                    db.HANGHOAs.Attach(hANGHOA);
                    db.Entry(hANGHOA).Property(s => s.SoLuongTon).IsModified = true;

                    db.SaveChanges();
                }
                return RedirectToAction("LayDanhSachPhieuXuatKho");
            }

            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "MaBG", xUATKHO.XK_MaBG);
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", xUATKHO.XK_MaNV);
            return View(xUATKHO);
        }


        //--------------------------SỬA PHIẾU XUẤT KHO------------------------
        public ActionResult Partial_SuaXK_LayChiTietXK(string id)
        {
            List<THONGTINSANPHAM> lstSp;
            if (Session["c"] == null)
            {
                var ctdh = db.CT_XUATKHO.Where(m => m.CTXK_MaXK.Equals(id)).ToList();
                lstSp = LayDanhSachSanPhamList();
                for (int i = 0; i < ctdh.Count; i++)
                {
                    THONGTINSANPHAM sp = new THONGTINSANPHAM(ctdh[i].CTXK_MaHH, ctdh[i].SoLuongXuatKho);
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
        public ActionResult SuaXK_XoaSP(String id)
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                myCart.RemoveAll(n => n.MaSP.Equals(id));
                return RedirectToAction("SuaPhieuXuatKho", "XUATKHO", new { id = Session["MaXK"] });
            }
            return RedirectToAction("SuaPhieuXuatKho", "XUATKHO", new { id = Session["MaXK"] });

        }

        // Cập nhật lại số lượng sản phẩm
        public ActionResult SuaXK_CapNhatSoLuong(String id, FormCollection f)
        {
            List<THONGTINSANPHAM> myCart = LayDanhSachSanPhamList();
            THONGTINSANPHAM pro = myCart.SingleOrDefault(n => n.MaSP.Equals(id));
            if (pro != null)
            {
                pro.SoLuong = int.Parse(f["changequantity"].ToString());
            }
            return RedirectToAction("SuaPhieuXuatKho", new { id = Session["MaXK"] });
        }

        // GET: XUATKHO/SuaPhieuXuatKho/5
        public ActionResult SuaPhieuXuatKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XUATKHO xUATKHO = db.XUATKHOes.Find(id);
            if (xUATKHO == null)
            {
                return HttpNotFound();
            }
            Session["MaXK"] = xUATKHO.MaXK;
            ViewBag.NgayLap = ((DateTime)xUATKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");
            return View(xUATKHO);
        }

        // POST: XUATKHO/SuaPhieuXuatKho/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaPhieuXuatKho()
        {
            String maXK = Session["MaXK"].ToString();
            var a = Session["lstSP"] as List<THONGTINSANPHAM>;
            if (a.Count != 0)
            {
                XUATKHO xUATKHO = new XUATKHO();

                List<THONGTINSANPHAM> lstSP = LayDanhSachSanPhamList();
                if (ModelState.IsValid)
                {
                    xUATKHO.MaXK = maXK;
                    xUATKHO.XK_MaNV = "";
                    xUATKHO.NgayLap = DateTime.Now;
                    xUATKHO.XK_MaBG = "";
                    xUATKHO.LyDoXuat = "";
                    xUATKHO.SoChungTu = "";
                    db.Entry(xUATKHO).State = EntityState.Modified;
                    db.Entry(xUATKHO).Property(s => s.XK_MaNV).IsModified = false;
                    db.Entry(xUATKHO).Property(s => s.XK_MaNV).IsModified = false;
                    db.Entry(xUATKHO).Property(s => s.NgayLap).IsModified = false;
                    db.Entry(xUATKHO).Property(s => s.XK_MaBG).IsModified = false;
                    db.Entry(xUATKHO).Property(s => s.LyDoXuat).IsModified = false;
                    db.Entry(xUATKHO).Property(s => s.SoChungTu).IsModified = false;

                    bool check;
                    List<CT_XUATKHO> cthd = db.CT_XUATKHO.Where(m => m.CTXK_MaXK.Equals(maXK)).ToList();
                    if (cthd != null)
                    {
                        for (int i = 0; i < cthd.Count(); i++)
                        {
                            check = XoaChiTietXK(cthd[i].CTXK_MaXK);
                        }
                    }
                    foreach (var item in lstSP)
                    {
                        var details = new CT_XUATKHO();
                        details.CTXK_MaXK = maXK;
                        details.SoLuongXuatKho = item.SoLuong;
                        details.CTXK_MaHH = item.MaSP;
                        db.CT_XUATKHO.Add(details);

                        //Tính số lượng tồn kho
                        HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(item.MaSP));
                        int slTon = (int)hANGHOA.SoLuongTon;
                        int slTonMoi = (int)(slTon - item.SoLuong);
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
                    return RedirectToAction("LayDanhSachPhieuXuatKho");
                }
                return View(xUATKHO);
            }
            else
            {
                bool check;
                List<CT_XUATKHO> cthd = db.CT_XUATKHO.Where(m => m.CTXK_MaXK.Equals(maXK)).ToList();
                if (cthd != null)
                {
                    for (int i = 0; i < cthd.Count(); i++)
                    {
                        check = XoaChiTietXK(cthd[i].CTXK_MaXK);
                    }
                }

                XUATKHO xUATKHO = db.XUATKHOes.Find(maXK);
                db.XUATKHOes.Remove(xUATKHO);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhieuXuatKho");

            }
        }

        // GET: XUATKHO/XoaPhieuXuatKho/5
        public ActionResult XoaPhieuXuatKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XUATKHO xUATKHO = db.XUATKHOes.Find(id);
            if (xUATKHO == null)
            {
                return HttpNotFound();
            }
            Session["MaXK"] = xUATKHO.MaXK;
            ViewBag.NgayLap = ((DateTime)xUATKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");

            return View(xUATKHO);
        }

        // POST: XUATKHO/XoaPhieuXuatKho/5
        [HttpPost, ActionName("XoaPhieuXuatKho")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaPhieuXuatKhoConfirmed(string id)
        {
            bool check;
            List<CT_XUATKHO> cthd = db.CT_XUATKHO.Where(m => m.CTXK_MaXK.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietXK(cthd[i].CTXK_MaXK);
                }
            }

            XUATKHO xUATKHO = db.XUATKHOes.Find(id);
            db.XUATKHOes.Remove(xUATKHO);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachPhieuXuatKho");
        }
        public bool XoaChiTietXK(string id)
        {
            CT_XUATKHO cT_XUATKHO = db.CT_XUATKHO.FirstOrDefault(m => m.CTXK_MaXK.Equals(id));
            db.CT_XUATKHO.Remove(cT_XUATKHO);

            //Tính số lượng tồn kho
            String maSP = cT_XUATKHO.CTXK_MaHH;
            HANGHOA hANGHOA = db.HANGHOAs.FirstOrDefault(m => m.MaHH.Equals(maSP));
            int slTon = (int)hANGHOA.SoLuongTmp;
            int slTonMoi = (int)(slTon + cT_XUATKHO.SoLuongXuatKho);
            if (slTonMoi < 0)
            {
                hANGHOA.SoLuongTon = 0;
                hANGHOA.SoLuongTmp = 0;
            }
            else { hANGHOA.SoLuongTon = slTonMoi;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private String TaoMaPhieuXuatKho()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<XUATKHO> lstHD = db.XUATKHOes.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "XK001";
            }
            else
            {
                XUATKHO lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaXK;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "XK00" + newMaHD.ToString();
                }
                else { idHD = "XK0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}
