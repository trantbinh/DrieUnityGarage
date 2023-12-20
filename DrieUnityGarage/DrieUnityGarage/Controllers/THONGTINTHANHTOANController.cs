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
    public class THONGTINTHANHTOANController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        public ActionResult LayDanhSachThongTinThanhToan()
        {
            return View(db.THONGTINTHANHTOANs.ToList());
        }

        public ActionResult LayThongTinThanhToan(string id) { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTHANHTOAN tHONGTINTHANHTOAN = db.THONGTINTHANHTOANs.Find(id);
            if (tHONGTINTHANHTOAN == null)
            {
                return HttpNotFound();
            }
            return View(tHONGTINTHANHTOAN);
        }

        public ActionResult TaoThongTinThanhToan(String idHD)
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayTT = date;

            String idTT = TaoMaThanhToan();
            ViewBag.MaTT = idTT;
            ViewBag.MaHD = idHD;
           /* ViewBag.TT_MaHD = Session["MaHD_ThanhToan"].ToString(); 
*/
            return View();
        }
        //Lưu thông tin tiếp nhận vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoThongTinThanhToan(String HinhThuc, String lstHD)
        {
            THONGTINTHANHTOAN tHONGTINTHANHTOAN = new THONGTINTHANHTOAN();
            var hd = db.HOADONs.Find(lstHD);
            if (ModelState.IsValid)
            {
                String id = TaoMaThanhToan();
                tHONGTINTHANHTOAN.MaTT = id;
                tHONGTINTHANHTOAN.TT_MaHD = lstHD;
                tHONGTINTHANHTOAN.TT_MaKH = hd.HD_MaKH;
                tHONGTINTHANHTOAN.TongThanhToan = hd.TongThanhToan;
                tHONGTINTHANHTOAN.HinhThuc = HinhThuc;
                tHONGTINTHANHTOAN.TyLeThanhToan = 1;
                tHONGTINTHANHTOAN.NgayTT=DateTime.Now;
                db.THONGTINTHANHTOANs.Add(tHONGTINTHANHTOAN);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachHoaDon","HOADON");
            }

            return View(tHONGTINTHANHTOAN);
        }
        // GET: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        public ActionResult SuaThongTinThanhToan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTHANHTOAN tHONGTINTHANHTOAN = db.THONGTINTHANHTOANs.Find(id);
            if (tHONGTINTHANHTOAN == null)
            {
                return HttpNotFound();
            }

/*            List<HOADON> lstHoaDon = LayDanhSachHoaDonDB();
            ViewBag.TT_MaHD = new SelectList(lstHoaDon, "MaHD", "ThongTin", tHONGTINTHANHTOAN.TT_MaHD);
            ViewBag.TT_MaKH = new SelectList(lstHoaDon, "MaHD", "ThongTin", tHONGTINTHANHTOAN.TT_MaKH);
*/

            
            DateTime date;
            if (tHONGTINTHANHTOAN.NgayTT != null)
            {
                date = (DateTime)tHONGTINTHANHTOAN.NgayTT;
                ViewBag.NgayTT = date.ToString("yyyy/MM/dd");
            }

            return View(tHONGTINTHANHTOAN);
        }
        // POST: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinThanhToan(THONGTINTHANHTOAN tHONGTINTHANHTOAN
            , String TT_MaHD, String lstHD, String HinhThuc, String TT_MaKH)
        {
            if (ModelState.IsValid)
            {
                tHONGTINTHANHTOAN.TT_MaHD = TT_MaHD;
                tHONGTINTHANHTOAN.TT_MaHD = lstHD;
                tHONGTINTHANHTOAN.HinhThuc = HinhThuc;

                db.Entry(tHONGTINTHANHTOAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachThongTinThanhToan");
            }
            return View(tHONGTINTHANHTOAN);
        }

        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinThanhToan(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            THONGTINTHANHTOAN tHONGTINTHANHTOAN = db.THONGTINTHANHTOANs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá";
            }
            else
            {
                if (tHONGTINTHANHTOAN == null)
                {
                    return HttpNotFound();
                }
            }
            return View(tHONGTINTHANHTOAN);
        }
        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinTiepNhan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            THONGTINTHANHTOAN tHONGTINTHANHTOAN = db.THONGTINTHANHTOANs.Find(id);
            db.THONGTINTHANHTOANs.Remove(tHONGTINTHANHTOAN);
            db.SaveChanges();


            return RedirectToAction("LayDanhSachThongTinThanhToan");
        }









        private String TaoMaThanhToan()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<THONGTINTHANHTOAN> lstHD = db.THONGTINTHANHTOANs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "TT001";
            }
            else
            {
                THONGTINTHANHTOAN lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaTT;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "TT00" + newMaHD.ToString();
                }
                else { idHD = "TN0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}