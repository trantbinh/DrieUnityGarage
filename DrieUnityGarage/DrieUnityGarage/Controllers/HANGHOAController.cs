using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrieUnityGarage.Models;

namespace DrieUnityGarage.Controllers
{
    public class HANGHOAController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        // GET: HANGHOA
        public ActionResult LayDanhSachHangHoa()
        {
            var hANGHOAs = db.HANGHOAs.Include(h => h.NHACUNGCAP);
            return View(hANGHOAs.ToList());
        }

        // GET: HANGHOA/Details/5
        public ActionResult LayThongTinHangHoa(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            if (hANGHOA == null)
            {
                return HttpNotFound();
            }
            return View(hANGHOA);
        }

        // GET: HANGHOA/Create
        public ActionResult ThemHangHoa()
        {
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC","NC000");
            ViewBag.MaHH = TaoMaHangHoa();
            return View();
        }

        // POST: HANGHOA/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemHangHoa([Bind(Include = "MaHH,TenHH,DonGia,DonViTinh,LoaiHang,SoLuongTon,HH_MaNCC,HinhAnh")] HANGHOA hANGHOA
            ,HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnh != null)
                {
                    var fileName = Path.GetFileName(HinhAnh.FileName);

                    var path = Path.Combine(Server.MapPath("~/Uploads/HangHoa/"), fileName);
                    hANGHOA.HinhAnh= fileName;
                    HinhAnh.SaveAs(path);
                }

                hANGHOA.MaHH= TaoMaHangHoa();
                hANGHOA.SoLuongTon = 0;
                db.HANGHOAs.Add(hANGHOA);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHangHoa");
            }
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC");
            return View(hANGHOA);
        }

        // GET: HANGHOA/Edit/5
        public ActionResult SuaThongTinHangHoa(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            if (hANGHOA == null)
            {
                return HttpNotFound();
            }
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // POST: HANGHOA/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinHangHoa([Bind(Include = "MaHH,TenHH,DonGia,DonViTinh,LoaiHang,SoLuongTon,HH_MaNCC, HinhAnh")] HANGHOA hANGHOA, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                //Xet xem có nhập dữ liệu mới cho hình ảnh không
                if (HinhAnh != null)
                {
                    var fileName = Path.GetFileName(HinhAnh.FileName);

                    var path = Path.Combine(Server.MapPath("~/Uploads/HangHoa/"), fileName);
                    hANGHOA.HinhAnh = fileName;
                    HinhAnh.SaveAs(path);
                    db.Entry(hANGHOA).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(hANGHOA).State = EntityState.Modified;
                    db.Entry(hANGHOA).Property(s=>s.HinhAnh).IsModified= false; //Không sửa đổi field hình ảnh
                    db.SaveChanges();
                }

                return RedirectToAction("LayDanhSachHangHoa");
            }
            ViewBag.HH_MaNCC = new SelectList(db.NHACUNGCAPs, "MaNCC", "TenNCC", hANGHOA.HH_MaNCC);
            return View(hANGHOA);
        }

        // GET: HANGHOA/Delete/5
        public ActionResult XoaHangHoa(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "Không thể xoá hàng hoá này vì mã hàng hoá đã được dùng để tạo thông tin khác";
            }
            else
            {

                if (hANGHOA == null)
                {
                    return HttpNotFound();
                }
            }
            return View(hANGHOA);
        }

        // POST: HANGHOA/Delete/5
        [HttpPost, ActionName("XoaHangHoa")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaHangHoaConfirmed(string id)
        {
            bool check = KiemTraKhoaNgoaiHangHoa(id);
            if (check == true)
            {

                return XoaHangHoa(id, 0);
            }
            else
            {
                HANGHOA hANGHOA = db.HANGHOAs.Find(id);
                db.HANGHOAs.Remove(hANGHOA);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHangHoa");
            }
        }
        public bool KiemTraKhoaNgoaiHangHoa(string id)
        {
            List<CT_HOADON> tn = db.CT_HOADON.Where(m => m.CTHD_MaHH.Equals(id)).ToList();
           
            if (tn != null)
            {
                return true;
            }
            return false;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private String TaoMaHangHoa()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<HANGHOA> lstHD = db.HANGHOAs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "HH001";
            }
            else
            {
                HANGHOA lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaHH;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "HH00" + newMaHD.ToString();
                }
                else { idHD = "HH0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}
