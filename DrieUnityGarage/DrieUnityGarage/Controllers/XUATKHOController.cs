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
            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH");
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV");
            return View();
        }

        // POST: XUATKHO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaXK,NgayLap,XK_MaNV,LyDoXuat,XK_MaBG,SoChungTu")] XUATKHO xUATKHO)
        {
            if (ModelState.IsValid)
            {
                db.XUATKHOes.Add(xUATKHO);
                db.SaveChanges();
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
            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH", xUATKHO.XK_MaBG);
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", xUATKHO.XK_MaNV);
            return View(xUATKHO);
        }

        // POST: XUATKHO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaXK,NgayLap,XK_MaNV,LyDoXuat,XK_MaBG,SoChungTu")] XUATKHO xUATKHO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xUATKHO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.XK_MaBG = new SelectList(db.BAOGIAs, "MaBG", "BG_MaKH", xUATKHO.XK_MaBG);
            ViewBag.XK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", xUATKHO.XK_MaNV);
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
