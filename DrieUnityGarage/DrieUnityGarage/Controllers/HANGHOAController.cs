﻿using System;
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
        public ActionResult LayThongTinBangGia()
        {
            var bangGia = db.BANGGIAs.ToList();
            return View(bangGia);   
        }

        public ActionResult TaoBangGia() {
            var hh = db.HANGHOAs.Where(m=>m.LoaiHang.Equals("Phụ tùng")).ToList();
            return View(hh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoBangGia(List<String> listSanPham)
        {
            if (listSanPham !=null)
            {
                bool check;
                var lstBG = db.BANGGIAs.ToList();
                foreach (var item in lstBG)
                {
                    check = XoaBangGia(item.BangGia_MaHH);
                }
                List<BANGGIA> bangGia = new List<BANGGIA>();
                for (int i = 0; i < listSanPham.Count(); i++)
                {
                    BANGGIA bg = new BANGGIA();
                    bg.BangGia_MaHH = listSanPham[i];
                    bangGia.Add(bg);
                }
                if (ModelState.IsValid)
                {
                    foreach (var item in bangGia)
                    {
                        BANGGIA bgDB = new BANGGIA();
                        bgDB.BangGia_MaHH = item.BangGia_MaHH;
                        db.BANGGIAs.Add(bgDB);
                        db.SaveChanges();
                    }
                    return RedirectToAction("LayThongTinBangGia");

                }

            }
            else
            {
                ViewBag.ThongBao = "!Lỗi: Tạo bảng giá không thành công! Chọn ít nhất một sản phẩm để tạo bảng giá";
            }
            var hh = db.HANGHOAs.Where(m => m.LoaiHang.Equals("Phụ tùng")).ToList();
            return View(hh);

        }
        public bool XoaBangGia(String id)
        {
            var lstBG = db.BANGGIAs.FirstOrDefault(m=>m.BangGia_MaHH.Equals(id));
            db.BANGGIAs.Remove(lstBG);
            db.SaveChanges();
            return true;
        }


        // GET: HANGHOA
        public ActionResult LayDanhSachHangHoa()
        {
            Session.Remove("KhongTheXoa");
            var hANGHOAs = db.HANGHOAs.Include(h => h.NHACUNGCAP);
            return View(hANGHOAs.ToList());
        }
        public ActionResult TraCuuHangHoa(String searchString, String LoaiHang = null, String lstDVT = null, String TinhTrang = null)
        {
            var hANGHOAs = db.HANGHOAs.Include(h => h.NHACUNGCAP);
            ViewBag.Keyword = searchString;
            ViewBag.lstDVT = new SelectList(db.HANGHOAs, "DonViTinh", "DonViTinh");

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                hANGHOAs = hANGHOAs.Where(b => b.TenHH.ToLower().Contains(searchString));
            }
            if (LoaiHang != null)
            {
                hANGHOAs = hANGHOAs.Where(c => c.LoaiHang.Equals(LoaiHang));
            }
            if (lstDVT != null)
            {
                hANGHOAs = hANGHOAs.Where(c => c.DonViTinh.Equals(lstDVT));
            }
            if (TinhTrang != null)
            {
                if (TinhTrang.Equals("Cần nhập hàng"))
                    hANGHOAs = hANGHOAs.Where(c => c.SoLuongTon <= 5);
                else hANGHOAs = hANGHOAs.Where(c => c.SoLuongTon > 5);

            }
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
                hANGHOA.SoLuongTmp = 0;
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
                    ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá hàng hoá này";
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
            }
            return RedirectToAction("LayDanhSachHangHoa");
        }
        //Kiểm tra xem có mã hàng hoá nào đã được tạo trong CTDH không, vì nếu có xoá sẽ bị lỗi
        public bool KiemTraKhoaNgoaiHangHoa(string id)
        {
            List<CT_HOADON> tn = db.CT_HOADON.Where(m => m.CTHD_MaHH.Equals(id)).ToList();
            List<CT_BAOGIA> bg = db.CT_BAOGIA.Where(m => m.CTBG_MaHH.Equals(id)).ToList();
            List<CT_XUATKHO> xk = db.CT_XUATKHO.Where(m => m.CTXK_MaHH.Equals(id)).ToList();
            List<CT_NHAPKHO> nk = db.CT_NHAPKHO.Where(m => m.CTNK_MaHH.Equals(id)).ToList();
            if (tn.Count()==0&& bg.Count() == 0&& xk.Count() == 0&& nk.Count() == 0)
            {
                return false;
            }
            return true;
        }

        // POST: HANGHOA/Delete/5
        public ActionResult XoaHangHoa_ToanBo(string id)
        {
            bool check;
            List<CT_HOADON> tn = db.CT_HOADON.Where(m => m.CTHD_MaHH.Equals(id)).ToList();
            if(tn.Count()!=0) {
                for (int i = 0; i < tn.Count(); i++)
                {
                    check = XoaChiTietHD(tn[i].CTHD_MaHD);
                }
            }
            HANGHOA hANGHOA = db.HANGHOAs.Find(id);
            db.HANGHOAs.Remove(hANGHOA);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachHangHoa");
        }
        private bool XoaChiTietHD(string id)
        {
            CT_HOADON cT_HOADON = db.CT_HOADON.FirstOrDefault(m => m.CTHD_MaHD.Equals(id));
            db.CT_HOADON.Remove(cT_HOADON);
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
        private String TaoMaHangHoa()
        {
            String idHD;
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
