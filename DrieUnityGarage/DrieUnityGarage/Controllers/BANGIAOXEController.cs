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
    public class BANGIAOXEController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        // GET: HOADON
        public ActionResult LayDanhSachBanGiaoXe()
        {
            Session.Remove("DaLayThongTinTiepNhan");
            Session.Remove("c");
            Session.Remove("lstSPHD");

            var bANGIAOXEs = db.BANGIAOXEs.Include(h => h.THONGTINTIEPNHAN);
            return View(bANGIAOXEs.ToList());
        }

        // GET: HOADON/Details/5
        public ActionResult XemThongTinBanGiaoXe(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BANGIAOXE bANGIAOXE = db.BANGIAOXEs.Find(id);
            if (bANGIAOXE == null)
            {
                return HttpNotFound();
            }

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(bANGIAOXE.BG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan = selectedTN;

            return View(bANGIAOXE);
        }

        public List<THONGTINTIEPNHANXE> LayDanhSachTiepNhanDB()
        {
            var newlstKH = new List<THONGTINTIEPNHANXE>();
            var khDB = db.THONGTINTIEPNHANs.Where(m => m.TrangThai.Equals("Chưa hoàn thành")).ToList();
            int c = khDB.Count();
            for (int i = 0; i < c; i++)
            {
                newlstKH.Add(new THONGTINTIEPNHANXE(khDB[i].MaTN));
            }
            Session.Remove("lstTN");
            Session["lstTN"] = newlstKH;
            return newlstKH;
        }
        //Hàm được sử dụng sau khi nhấn nút
        public ActionResult TaoBGX_LayThongTinTiepNhan(String lstMaTN)
        {
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(lstMaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaHD"] = TaoMaBanGiaoXe();
            Session["MaTN"] = lstMaTN;



            //Check đã lấy thông tin xe hay chưa, có null không
            Session["DaLayThongTinTiepNhan"] = 3;

            return RedirectToAction("ThemBanGiaoXe");
        }
        // GET: HOADON/Create
        public ActionResult ThemBanGiaoXe()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapBG = date;
            String idTN = TaoMaBanGiaoXe();
            ViewBag.MaBG = idTN;
            Session["MaHD_ThanhToan"] = idTN;
            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "MaTN","TN000");

            if (Session["DaLayThongTinTiepNhan"] != null)
            {
                ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
            }
            return View();
        }


        // POST: HOADON/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ThemBanGiaoXe(int DanhGiaTinhTrang, int DanhGiaTrangBi, String lstMaTN)
        {
            BANGIAOXE bANGIAOXE = new BANGIAOXE();
            if (ModelState.IsValid)
            {
                String idHD = TaoMaBanGiaoXe();
                bANGIAOXE.MaBG = idHD;
                bANGIAOXE.NgayLap = DateTime.Now;
                bANGIAOXE.BG_MaTN = lstMaTN;
                bANGIAOXE.DanhGiaTinhTrang = DanhGiaTinhTrang;
                bANGIAOXE.DanhGiaTrangBi = DanhGiaTrangBi;
                db.BANGIAOXEs.Add(bANGIAOXE);

                THONGTINTIEPNHAN TTTN = db.THONGTINTIEPNHANs.FirstOrDefault(m => m.MaTN.Equals(lstMaTN));
                TTTN.TrangThai = "Đã hoàn thành";
                TTTN.MaTN = lstMaTN;
                TTTN.TN_MaNV = "";
                TTTN.TN_MaKH = "";
                TTTN.TN_BienSoXe = "";
                TTTN.ThoiGianGiaoDuKien = DateTime.Now;
                TTTN.ThoiGianTiepNhan = DateTime.Now;
                TTTN.GhiChuKH = "";
                db.THONGTINTIEPNHANs.Attach(TTTN);
                db.Entry(TTTN).Property(s => s.TrangThai).IsModified = true;


                db.SaveChanges();
                return RedirectToAction("LayDanhSachBanGiaoXe");
            }
            ViewBag.BG_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN", "TN000");
            return View(bANGIAOXE);
        }
        // GET: HOADON/SuaHoaDon/5
        public ActionResult SuaBanGiaoXe(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BANGIAOXE bANGIAOXE = db.BANGIAOXEs.Find(id);
            if (bANGIAOXE == null)
            {
                return HttpNotFound();
            }
            //Lấy thông tin tiếp nhận
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapBG = date;
            ViewBag.MaBG = id;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "MaTN","TN000");

            //Lấy ra thông tin tiếp nhận
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(bANGIAOXE.BG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;


            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaBG"] = id;
            Session["MaTN"] = bANGIAOXE.BG_MaTN;

            ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
            return View(bANGIAOXE);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaBanGiaoXe([Bind(Include = "MaBG,NgayLap,BG_MaTN,DanhGiaTinhTrang,DanhGiaTrangBi")] BANGIAOXE bg)
        {
            if (ModelState.IsValid)
            {
                String id = Session["MaBG"].ToString();
                if (ModelState.IsValid)
                {
                    bg.MaBG = id;
                    bg.NgayLap = DateTime.Now;
                    bg.BG_MaTN = Session["MaTN"].ToString();
                    bg.DanhGiaTinhTrang = bg.DanhGiaTinhTrang;
                    bg.DanhGiaTrangBi = bg.DanhGiaTrangBi;
                    db.Entry(bg).State = EntityState.Modified;
                    db.SaveChanges();
                }
                Session.Remove("MaHD");
                Session.Remove("MaKH");
                Session.Remove("MaTN");
                Session.Remove("BienSoXe");
                Session.Remove("CheckTN");
                Session.Remove("lstSPHD");
                Session.Remove("c");
                return RedirectToAction("LayDanhSachBanGiaoXe");
            }

            return View(bg);
        }

        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinBanGiaoXe(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BANGIAOXE bANGIAOXE = db.BANGIAOXEs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá";
            }
            else
            {
                if (bANGIAOXE == null)
                {
                    return HttpNotFound();
                }
            }
            return View(bANGIAOXE);
        }


        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinDatLich")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            BANGIAOXE bANGIAOXE = db.BANGIAOXEs.Find(id);
            db.BANGIAOXEs.Remove(bANGIAOXE);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachBanGiaoXe");
        }
        private String TaoMaBanGiaoXe()
        {
            String idDL = "";
            //Tạo mã nhà cung cấp String
            List<BANGIAOXE> lstDL = db.BANGIAOXEs.ToList();
            int countLst = lstDL.Count();
            if (countLst == 0)
            {
                idDL = "BG001";
            }
            else
            {
                BANGIAOXE lastDL = lstDL[countLst - 1];
                String lastMaDL = lastDL.MaBG;
                int lastMaDLNum = int.Parse(lastMaDL.Substring(2));
                int newMaDL = lastMaDLNum + 1;
                if (newMaDL < 10)
                {
                    idDL = "BG00" + newMaDL.ToString();
                }
                else { idDL = "BGN0" + newMaDL.ToString(); }
            }
            return (idDL);
        }
    }
}