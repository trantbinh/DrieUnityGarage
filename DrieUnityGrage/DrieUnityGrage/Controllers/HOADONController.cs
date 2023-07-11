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

        //Hàm để lấy giỏ hàng hiện tại
        public List<CTHD> LayDSCTHD()
        {
            List<CTHD> lstCTHD = Session["CTHD"] as List<CTHD>;

            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào Session
            if (lstCTHD == null)
            {
                lstCTHD = new List<CTHD>();
                Session["CTHD"] = lstCTHD;
            }
            return lstCTHD;
        }


        //Thêm một sản phẩm vào giỏ
        public ActionResult ThemCTHD(String id)
        {
            List<CTHD> myCart = LayDSCTHD();
            CTHD currentProduct = myCart.FirstOrDefault(p => p.MaSP.Equals(id));
            if (currentProduct == null)
            {
                currentProduct = new CTHD(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.SoLuong++;
            }
            return RedirectToAction("Create", "HOADON");
        }
        //Tính tổng số lượng mặt hàng được mua
        private int LayTongSoLuong()
        {
            int totalNumber = 0;
            List<CTHD> myCart = LayDSCTHD();
            if (myCart != null)
                totalNumber = myCart.Sum(sp => sp.SoLuong);
            return totalNumber;
        }


        //Tính tổng tiền của các sản phẩm được mua
        private decimal LayTongTien()
        {
            decimal totalPrice = 0;
            List<CTHD> myCart = LayDSCTHD();
            if (myCart != null)
                totalPrice = myCart.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }

        public ActionResult Partial_LayChiTietHoaDon()
        {
            var a = db.CT_HOADON.Include(h=> h.HANGHOA);

            return PartialView(a.ToList());
        }


        public ActionResult Partial_ThemChiTietHoaDon()
        {
            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH");

            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Partial_ThemChiTietHoaDon([Bind(Include = "CTHD_MaHH,CTHD_MaHD,SoLuong,ThanhTien")] CT_HOADON cT_HOADON)
        {
            if (ModelState.IsValid)
            {
                var a = db.HANGHOAs.Find(cT_HOADON.CTHD_MaHH);
                cT_HOADON.CTHD_MaHD = Session["MaHD"].ToString();
                cT_HOADON.ThanhTien = cT_HOADON.SoLuong * a.DonGia;
                db.CT_HOADON.Add(cT_HOADON);
                db.SaveChanges();
                return RedirectToAction("Create","HOADON");
            }

            ViewBag.CTHD_MaHH = new SelectList(db.HANGHOAs, "MaHH", "TenHH", cT_HOADON.CTHD_MaHH);
            ViewBag.CTHD_MaHD = new SelectList(db.HOADONs, "MaHD", "HD_MaKH", cT_HOADON.CTHD_MaHD);
            return View(cT_HOADON);
        }














        public ActionResult Partial_LayThongTinTiepNhan(String id) { 
        
            var a = db.THONGTINTIEPNHANs.Where(m=> m.MaTN.Equals(id)).ToList();

                return PartialView(a);

          
        }







        // GET: HOADON
        public ActionResult Index()
        {
            var hOADONs = db.HOADONs.Include(h => h.KHACHHANG).Include(h => h.PHUONGTIEN).Include(h => h.THONGTINTHANHTOAN).Include(h => h.THONGTINTIEPNHAN);
            return View(hOADONs.ToList());
        }

        // GET: HOADON/Details/5
        public ActionResult Details(string id)
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
        public ActionResult Create(String id)
        {
            Session["CheckTN"] = 0;
            String idHD;
            if (id != null)
            {
                idHD = id;
            }
            else
            { idHD = DemHoaDon(); }
            
            HOADON hOADONs = db.HOADONs.Find(id);
            if (hOADONs == null)
            {
                Session["CheckTN"] = null;
            }


            var tn = db.THONGTINTIEPNHANs.ToList();
            // ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH");
            /*  ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "BienSoXe");
              ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH");*/
                ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN");
                ViewBag.MaHD = idHD;
                String date = String.Format("{0:dd/MM/yy}", DateTime.Now.ToString());
                ViewBag.NgayLapHD = date;
                return View(hOADONs);
        }
        private String DemHoaDon()
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
                if (newMaHD < 10) { 
                    idHD = "HD00" + newMaHD.ToString(); }
                else { idHD = "HD0" + newMaHD.ToString(); }
            }
            return(idHD);
        }



        //Autofill fields
        public ActionResult LayThongTinTiepNhan(string id)
        {
            /*            THONGTINTIEPNHAN thongtinTN = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(id));
                        KHACHHANG kh = db.KHACHHANGs.FirstOrDefault(m => m.MaKH.Equals(thongtinTN.TN_MaKH));
                        PHUONGTIEN pt = db.PHUONGTIENs.FirstOrDefault(m => m.BienSoXe == thongtinTN.TN_BienSoXe);
                        NHANVIEN nv = db.NHANVIENs.FirstOrDefault(m => m.MaNV == thongtinTN.TN_MaNV);
                        List<string> lstThongTin = new List<string>();
                        lstThongTin.Add(kh.MaKH);
                        lstThongTin.Add(kh.HoTenKH);
                        // lstThongTin.Add(kh.Email);
                       // lstThongTin.Add(kh.DienThoaiKH);
                        lstThongTin.Add(nv.HoTenNV);
                        lstThongTin.Add(pt.BienSoXe);
                        *//*lstThongTin.Add(pt.SoMay);
                        lstThongTin.Add(pt.SoKhung);
                        lstThongTin.Add(pt.SoKM.ToString());
                        lstThongTin.Add(pt.MauXe);
                        lstThongTin.Add(pt.LoaiXe);
                        lstThongTin.Add(pt.Model);*/
            var lstThongTin = db.THONGTINTIEPNHANs.Where(m => m.MaTN.Equals(id)).ToList() ;
            return PartialView(lstThongTin);
        }
        public ActionResult Extensions_LayThongTinKhachHang(string id)
        {
            var lstThongTin = db.KHACHHANGs.Where(m => m.MaKH.Equals(id)).ToList() ;
            return PartialView(lstThongTin);
        }


/*        [HttpPost]


        public ActionResult GetInfo([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            String idTN;
                idTN = hOADON.HD_MaTN;
                THONGTINTIEPNHAN a = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(idTN));
                ViewBag.TenNV = "c";
            return RedirectToAction("Create","HOADON");
        }

*/

        public PartialViewResult GetInfo(String id)
        {
            List<THONGTINTIEPNHAN> a = db.THONGTINTIEPNHANs.Where(m => m.MaTN.Equals(id)).ToList();
            return PartialView(a);


        }
        // POST: HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                String idHD = DemHoaDon();
                String date = String.Format("{0:dd/MM/yy}", DateTime.Now.ToString());
                hOADON.MaHD = idHD;
                hOADON.NgayLap = DateTime.Now;
                hOADON.HD_MaKH = null;
                hOADON.HD_MaKH = null;
                hOADON.TongThanhToan = null;
                hOADON.HD_MaTT = null;
                db.HOADONs.Add(hOADON);
                db.SaveChanges();
                return RedirectToAction("Create","HOADON", new {id = idHD});
            }
            ViewBag.HD_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", hOADON.HD_MaTN);
            return View(hOADON);
        }

        // GET: HOADON/Edit/5
        public ActionResult Edit(string id)
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
        public ActionResult Edit([Bind(Include = "MaHD,NgayLap,HD_MaKH,HD_BienSoXe,TongThanhToan,HD_MaTT,HD_MaTN")] HOADON hOADON)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOADON).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HD_MaKH = new SelectList(db.KHACHHANGs, "MaKH", "HoTenKH", hOADON.HD_MaKH);
            ViewBag.HD_BienSoXe = new SelectList(db.PHUONGTIENs, "BienSoXe", "SoMay", hOADON.HD_BienSoXe);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTHANHTOANs, "MaTT", "TT_MaKH", hOADON.HD_MaTT);
            ViewBag.HD_MaTT = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", hOADON.HD_MaTT);
            return View(hOADON);
        }

        // GET: HOADON/Delete/5
        public ActionResult Delete(string id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HOADON hOADON = db.HOADONs.Find(id);
            db.HOADONs.Remove(hOADON);
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
    }
}
