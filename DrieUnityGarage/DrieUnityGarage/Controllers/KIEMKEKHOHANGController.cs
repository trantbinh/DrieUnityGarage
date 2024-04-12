using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGarage.Models;
using Rotativa;

namespace DrieUnityGarage.Controllers
{
    public class KIEMKEKHOHANGController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: KIEMKEKHOHANG
        public ActionResult LayDanhSachPhieuKiemKeKhoHang()
        {
            var kIEMKEKHOHANGs = db.KIEMKEKHOHANGs.Include(k => k.NHANVIEN);
            return View(kIEMKEKHOHANGs.ToList());
        }

        // GET: KIEMKEKHOHANG/LayChiTietKiemKeKhoHang/5
        public ActionResult LayChiTietKiemKeKhoHang(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KIEMKEKHOHANG kIEMKEKHOHANG = db.KIEMKEKHOHANGs.Find(id);
            if (kIEMKEKHOHANG == null)
            {
                return HttpNotFound();
            }
            String idNV = Session["MaTaiKhoanNV"].ToString();
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.MaBC = TaoMaKiemKe();
            ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy");


            return View(kIEMKEKHOHANG);
        }
        public ActionResult Partial_XemKK_ChiTietKiemKe(String id)
        {
            var ct = db.CT_KIEMKEKHOHANG.Where(m=>m.CTKK_MaKK.Equals(id)).ToList();
            return PartialView(ct);
        }
        [ValidateAntiForgeryToken]
        public ActionResult PrintIndex(String id)
        {
            return new ActionAsPdf("PrintPage", new { name = "Giorgio" , id = id }) { FileName = "KiemKeKhoHang.pdf" };
        }

        public ActionResult PrintPage(String id)
        {
            String idNV = "NV002";
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.MaBC = TaoMaKiemKe();
            ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy");
            var kk = db.CT_KIEMKEKHOHANG.Where(m=>m.CTKK_MaKK.Equals(id)).ToList();
            return View(kk);
        }

        // GET: KIEMKEKHOHANG/TaoPhieuKiemKeKhoHang
        public ActionResult TaoPhieuKiemKeKhoHang()
        {
            String idNV = Session["MaTaiKhoanNV"].ToString();
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.MaBC = TaoMaKiemKe();
            ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy");

            var sp = db.HANGHOAs.ToList();
            return View(sp);
        }

        // POST: KIEMKEKHOHANG/TaoPhieuKiemKeKhoHang
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoPhieuKiemKeKhoHang([Bind(Include = "MaKK,ThoiDiemKiemKe,KK_MaNV")] KIEMKEKHOHANG kIEMKEKHOHANG, List<int> listSL )
        {
            List<CT_KIEMKEKHOHANG> ctkk = new List<CT_KIEMKEKHOHANG>();
            var lstSP = db.HANGHOAs.ToList();
            String id = TaoMaKiemKe();
            //Tạo 1 list CT_KK để lưu danh sách hàng hoá vào trước
            for(int i = 0; i < lstSP.Count(); i++)
            {
                CT_KIEMKEKHOHANG chitiet = new CT_KIEMKEKHOHANG();
                chitiet.CTKK_MaKK = kIEMKEKHOHANG.MaKK;
                chitiet.CTKK_MaHH = lstSP[i].MaHH;
                chitiet.CTKK_SoLuongSoSach = lstSP[i].SoLuongTon;
                chitiet.CTKK_ThanhTienSoSach = lstSP[i].SoLuongTon * lstSP[i].DonGia;
                chitiet.CTKK_SoLuongKiemKe = null;
                chitiet.CTKK_ThanhTienKiemKe = null;
                ctkk.Add(chitiet);
            }

            // Chạy vòng lặp để gán 2 giá trị mới là số lượng và thành tiền kiểm kê
            for(int j =0; j < listSL.Count(); j++)
            {
                var donGia = db.HANGHOAs.Find(ctkk[j].CTKK_MaHH).DonGia;
                ctkk[j].CTKK_SoLuongKiemKe = listSL[j];
                ctkk[j].CTKK_ThanhTienKiemKe= listSL[j]*donGia;
            }
            if (ModelState.IsValid)
            {
                kIEMKEKHOHANG.MaKK = id;
                kIEMKEKHOHANG.KK_MaNV = Session["MaTaiKhoanNV"].ToString();
                kIEMKEKHOHANG.ThoiDiemKiemKe = DateTime.Now;
                db.KIEMKEKHOHANGs.Add(kIEMKEKHOHANG);
                db.SaveChanges();
                //Gán giá trị và lưu lên db
                foreach(var item in ctkk)
                {
                    CT_KIEMKEKHOHANG kiemkedb = new CT_KIEMKEKHOHANG();
                    kiemkedb.CTKK_MaKK = id;
                    kiemkedb.CTKK_MaHH = item.CTKK_MaHH;
                    kiemkedb.CTKK_SoLuongSoSach = item.CTKK_SoLuongSoSach;
                    kiemkedb.CTKK_SoLuongKiemKe = item.CTKK_SoLuongKiemKe;
                    kiemkedb.CTKK_ThanhTienKiemKe = item.CTKK_ThanhTienKiemKe;
                    kiemkedb.CTKK_ThanhTienSoSach = item.CTKK_ThanhTienSoSach;
                    db.CT_KIEMKEKHOHANG.Add(kiemkedb);
                    db.SaveChanges();

                }

                return RedirectToAction("LayDanhSachPhieuKiemKeKhoHang");
            }

            ViewBag.KK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", kIEMKEKHOHANG.KK_MaNV);
            return View(kIEMKEKHOHANG);
        }

        // GET: KIEMKEKHOHANG/XoaPhieuKiemKeKhoHang/5
        public ActionResult XoaPhieuKiemKeKhoHang(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KIEMKEKHOHANG kIEMKEKHOHANG = db.KIEMKEKHOHANGs.Find(id);
            if (kIEMKEKHOHANG == null)
            {
                return HttpNotFound();
            }
            String idNV = Session["MaTaiKhoanNV"].ToString();
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.MaBC = TaoMaKiemKe();
            ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy");


            return View(kIEMKEKHOHANG);
        }

        // POST: KIEMKEKHOHANG/XoaPhieuKiemKeKhoHang/5
        [HttpPost, ActionName("XoaPhieuKiemKeKhoHang")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaPhieuKiemKeKhoHangConfirmed(string id)
        {
            bool check;
            List<CT_KIEMKEKHOHANG> cthd = db.CT_KIEMKEKHOHANG.Where(m => m.CTKK_MaKK.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietKK(cthd[i].CTKK_MaKK);
                }
            }
            KIEMKEKHOHANG kIEMKEKHOHANG = db.KIEMKEKHOHANGs.Find(id);
            db.KIEMKEKHOHANGs.Remove(kIEMKEKHOHANG);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachPhieuKiemKeKhoHang");
        }
        public bool XoaChiTietKK(string id)
        {
            CT_KIEMKEKHOHANG cT_KIEMKEKHOHANG = db.CT_KIEMKEKHOHANG.FirstOrDefault(m => m.CTKK_MaKK.Equals(id));
            db.CT_KIEMKEKHOHANG.Remove(cT_KIEMKEKHOHANG);
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
        private String TaoMaKiemKe()
        {
            String idBG = "";
            //Tạo mã nhà cung cấp String
            List<KIEMKEKHOHANG> lstBG = db.KIEMKEKHOHANGs.ToList();
            int countLst = lstBG.Count();
            if (countLst == 0)
            {
                idBG = "KK001";
            }
            else
            {
                KIEMKEKHOHANG lastBG = lstBG[countLst - 1];
                String lastMaBG = lastBG.MaKK;
                int lastMaBGNum = int.Parse(lastMaBG.Substring(2));
                int newMaBG = lastMaBGNum + 1;
                if (newMaBG < 10)
                {
                    idBG = "KK00" + newMaBG.ToString();
                }
                else { idBG = "KK0" + newMaBG.ToString(); }
            }
            return (idBG);
        }


    }
}
