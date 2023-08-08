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
    public class BAOCAOTONKHOController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        private DateTime currentTime = new DateTime(2015, 12, 31);
        // GET: BAOCAOTONKHO
        public ActionResult LayDanhSachBaoCaoTonKho(DateTime? dateStart, DateTime? dateEnd, String lstNhanVien)
        {
            var bAOCAOTONKHOes = db.BAOCAOTONKHOes.Include(b => b.NHANVIEN);

            ViewBag.lstNhanVien = new SelectList(db.NHANVIENs, "HoTenNV", "HoTenNV");
            if (dateStart != null)
            {
                bAOCAOTONKHOes= bAOCAOTONKHOes.Where(c=>c.NgayLap>=dateStart);
            }
            if (dateEnd != null)
            {
                bAOCAOTONKHOes = bAOCAOTONKHOes.Where(c => c.NgayLap <= dateEnd);
            }
            if (lstNhanVien != null)
            {
                bAOCAOTONKHOes = bAOCAOTONKHOes.Where(c => c.NHANVIEN.HoTenNV.Equals(lstNhanVien));
            }
            return View(bAOCAOTONKHOes.ToList());
        }

        // GET: BAOCAOTONKHO/LayChiTietBaoCaoTonKho/5
        public ActionResult LayChiTietBaoCaoTonKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOCAOTONKHO bAOCAOTONKHO = db.BAOCAOTONKHOes.Find(id);
            if (bAOCAOTONKHO == null)
            {
                return HttpNotFound();
            }
            String tenNV = db.NHANVIENs.Find(bAOCAOTONKHO.BCTK_MaNV).HoTenNV; ;
            String nv = bAOCAOTONKHO.BCTK_MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            Session["MaBC"] = id;
            return View(bAOCAOTONKHO);
        }
        public ActionResult Partial_XemBC_ChiTietBaoCao(String id)
        {
            var hanghoa = db.CT_BAOCAOTONKHO.Include(m=>m.HANGHOA).Where(m=>m.CTBCTK_MaBCTK.Equals(id)).ToList();
            return PartialView(hanghoa);
        }

        // GET: BAOCAOTONKHO/TaoBaoCaoTonKho
        public ActionResult TaoBaoCaoTonKho()
        {
            String idNV = "NV001";
            String tenNV = db.NHANVIENs.Find(idNV).HoTenNV; ;
            String nv = idNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.MaBC = TaoMaBaoCao();
            ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy");
            return View();
        }
        public ActionResult Partial_TaoBC_ChiTietBaoCao()
        {
            var hanghoa = db.HANGHOAs.Where(m=>m.LoaiHang.Equals("Phụ tùng")).Include(m=>m.NHACUNGCAP).ToList();
            return PartialView(hanghoa);


        }
        [ValidateAntiForgeryToken]
        public ActionResult PrintIndex(String id)
        {
            return new ActionAsPdf("PrintPage", new { name = "Giorgio", id = id}) { FileName = "BaoCaoTonKho.pdf" };
        }

        public ActionResult PrintPage(String id)
        {
            var hanghoa = db.CT_BAOCAOTONKHO.Where(m=>m.CTBCTK_MaBCTK.Equals(id)).ToList();
            String tenNV = db.NHANVIENs.Find("NV001").HoTenNV; ;
            String nv = "NV001" + " - " + tenNV;
            ViewBag.NhanVien = nv;
            ViewBag.MaBC = TaoMaBaoCao();
            ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy");

            return View(hanghoa);
        }
        // POST: BAOCAOTONKHO/TaoBaoCaoTonKho
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoBaoCaoTonKho([Bind(Include = "MaBCTK,NgayLap,BCTK_MaNV")] BAOCAOTONKHO bAOCAOTONKHO)
        {
            String id = TaoMaBaoCao();
            var hanghoa = db.HANGHOAs.Where(m => m.LoaiHang.Equals("Phụ tùng")).Include(m => m.NHACUNGCAP).ToList();
            if (ModelState.IsValid)
            {
                bAOCAOTONKHO.MaBCTK = id;
                bAOCAOTONKHO.NgayLap = DateTime.Now;
                bAOCAOTONKHO.BCTK_MaNV = "NV001";
                db.BAOCAOTONKHOes.Add(bAOCAOTONKHO);
                db.SaveChanges();

                foreach(var item in hanghoa)
                {
                    var de = new CT_BAOCAOTONKHO();
                    de.CTBCTK_MaBCTK = id;
                    de.CTBCTK_MaHH = item.MaHH;
                    de.CTBCTK_NhaCungCap = item.NHACUNGCAP.TenNCC;
                    de.CTBCTK_SoLuongTonKho = item.SoLuongTon;
                    db.CT_BAOCAOTONKHO.Add(de);
                    db.SaveChanges();
                }



                return RedirectToAction("LayDanhSachBaoCaoTonKho");
            }

            ViewBag.BCTK_MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTenNV", bAOCAOTONKHO.BCTK_MaNV);
            return View(bAOCAOTONKHO);
        }

        // GET: BAOCAOTONKHO/XoaBaoCaoTonKho/5
        public ActionResult XoaBaoCaoTonKho(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BAOCAOTONKHO bAOCAOTONKHO = db.BAOCAOTONKHOes.Find(id);
            if (bAOCAOTONKHO == null)
            {
                return HttpNotFound();
            }
            String tenNV = db.NHANVIENs.Find(bAOCAOTONKHO.BCTK_MaNV).HoTenNV; ;
            String nv = bAOCAOTONKHO.BCTK_MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;
            Session["MaBC"] = id;
            return View(bAOCAOTONKHO);
        }

        // POST: BAOCAOTONKHO/XoaBaoCaoTonKho/5
        [HttpPost, ActionName("XoaBaoCaoTonKho")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            bool check;
            List<CT_BAOCAOTONKHO> cthd = db.CT_BAOCAOTONKHO.Where(m => m.CTBCTK_MaBCTK.Equals(id)).ToList();
            if (cthd != null)
            {
                for (int i = 0; i < cthd.Count(); i++)
                {
                    check = XoaChiTietBC(cthd[i].CTBCTK_MaBCTK);
                }
            }

            BAOCAOTONKHO bAOCAOTONKHO = db.BAOCAOTONKHOes.Find(id);
            db.BAOCAOTONKHOes.Remove(bAOCAOTONKHO);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachBaoCaoTonKho");
        }
        public bool XoaChiTietBC(string id)
        {
            CT_BAOCAOTONKHO cT_BAOCAOTONKHO = db.CT_BAOCAOTONKHO.FirstOrDefault(m => m.CTBCTK_MaBCTK.Equals(id));
            db.CT_BAOCAOTONKHO.Remove(cT_BAOCAOTONKHO);
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

        private String TaoMaBaoCao()
        {
            String idBG = "";
            //Tạo mã nhà cung cấp String
            List<BAOCAOTONKHO> lstBG = db.BAOCAOTONKHOes.ToList();
            int countLst = lstBG.Count();
            if (countLst == 0)
            {
                idBG = "TK001";
            }
            else
            {
                BAOCAOTONKHO lastBG = lstBG[countLst - 1];
                String lastMaBG = lastBG.MaBCTK;
                int lastMaBGNum = int.Parse(lastMaBG.Substring(2));
                int newMaBG = lastMaBGNum + 1;
                if (newMaBG < 10)
                {
                    idBG = "TK00" + newMaBG.ToString();
                }
                else { idBG = "TK0" + newMaBG.ToString(); }
            }
            return (idBG);
        }

    }
}
