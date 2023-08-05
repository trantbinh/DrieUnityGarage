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
        public ActionResult Index()
        {
            Session.Remove("lstSP");
            var xUATKHOes = db.XUATKHOes.Include(x => x.BAOGIA).Include(x => x.NHANVIEN);
            return View(xUATKHOes.ToList());
        }

        // GET: XUATKHO/Details/5
        public ActionResult Details(string id)
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
            return View(xUATKHO);
        }

        // GET: XUATKHO/Create
        public ActionResult Create()

        {
            ViewBag.MaXK = TaoMaPhieuXuatKho();
            ViewBag.MaNV = "NV001";
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            ViewBag.LyDoXuat = Session["LyDoXuat"];
            ViewBag.SoCT = Session["SoCT"];
            return View();
        }

        // POST: XUATKHO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        //Thêm một sản phẩm vào CTHD
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
            return RedirectToAction("Create", "XUATKHO");
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
                return RedirectToAction("Create", "XUATKHO");
            }
            return RedirectToAction("Create", "XUATKHO");

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
            ViewBag.CTXK_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
            return PartialView();
        }
        [HttpPost]
        public ActionResult Partial_TaoXK_ThemChiTietPhieuXK(CT_XUATKHO hh, String LyDoXuat, String SoCT)
        {
                ViewBag.CTXK_MaHH  = new SelectList(db.HANGHOAs, "MaHH", "TenHH");
                ViewBag.Selected = hh.CTXK_MaXK;
                Session["MaHH"] = hh.CTXK_MaHH;
                Session["LyDoXuat"] = LyDoXuat;
                Session["SoCT"] = SoCT;
                return RedirectToAction("CTXK_ThemSP", "XUATKHO", new { id = Session["MaHH"].ToString() });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(String MaXK)
        {
            XUATKHO xUATKHO = new XUATKHO();
            List<THONGTINSANPHAM> lstXK = LayDanhSachSanPhamList();
            String id= TaoMaPhieuXuatKho();
            if (ModelState.IsValid)
            {
                xUATKHO.MaXK = id;
                xUATKHO.XK_MaNV = "NV001";
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
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH", xUATKHO.XK_MaBG);
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", xUATKHO.XK_MaNV);
            return View(xUATKHO);
        }

        //----------TẠO PHIẾU TỪ BÁO GIÁ------------------------ (Tạm thời đang test trên hoá đơn)
        //Partial Danh sách hàng hoá
        public ActionResult Partial_TaoBG_LayDuLieuCTBG(String id)
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

        // GET: XUATKHO/Create
        public ActionResult TaoPhieuXuatKho_PhieuBG(String idBG)
        {
            ViewBag.MaXK = TaoMaPhieuXuatKho();
            ViewBag.LyDoXuat = "Xuất theo báo giá";
            ViewBag.MaBG = idBG;
            ViewBag.MaNV = "NV001";
            ViewBag.NgayLap = DateTime.Now.ToString("hh:mm:ss, dd/MM/yyyy");
            /*var bg = db.HOADONs.Find(idBG);*/
            return View();
        }

        // POST: XUATKHO/Create
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
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "MaBG", xUATKHO.XK_MaBG);
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", xUATKHO.XK_MaNV);
            return View(xUATKHO);
        }



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
                return RedirectToAction("Edit", "XUATKHO", new { id = Session["MaXK"] });
            }
            return RedirectToAction("Edit", "XUATKHO", new { id = Session["MaXK"] });

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
            return RedirectToAction("Edit", new { id = Session["MaXK"] });
        }



        // GET: XUATKHO/Edit/5
        public ActionResult Edit(string id)
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
            Session["MaXK"] = id;
            ViewBag.NgayLap = ((DateTime)xUATKHO.NgayLap).ToString("hh:mm:ss, dd/MM/yyyy");
            return View(xUATKHO);
        }

        // POST: XUATKHO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            XUATKHO xUATKHO = new XUATKHO();
            String maXK = Session["MaXK"].ToString();
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
                db.SaveChanges();
            }
            return View(xUATKHO);
        }

        // GET: XUATKHO/Delete/5
        public ActionResult Delete(string id)
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
            return View(xUATKHO);
        }

        // POST: XUATKHO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            XUATKHO xUATKHO = db.XUATKHOes.Find(id);
            db.XUATKHOes.Remove(xUATKHO);
            db.SaveChanges();
            return RedirectToAction("Index");
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
