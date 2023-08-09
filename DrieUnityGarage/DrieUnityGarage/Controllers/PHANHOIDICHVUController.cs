using DrieUnityGarage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;

namespace DrieUnityGarage.Controllers
{
    public class PHANHOIDICHVUController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();

        public ActionResult LayDanhSachPhanHoiDichVu()
        {
            Session.Remove("DaLayThongTinTiepNhan");
            Session.Remove("c");

            var dANHGIADICHVUs = db.DANHGIADICHVUs.Include(h => h.THONGTINTIEPNHAN);
            return View(dANHGIADICHVUs.ToList());
        }

        // GET: HOADON/Details/5
        public ActionResult LayThongTinPhanHoiDichVu(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHGIADICHVU dANHGIADICHVU = db.DANHGIADICHVUs.Find(id);
            if (dANHGIADICHVU == null)
            {
                return HttpNotFound();
            }

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(dANHGIADICHVU.DGDV_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan = selectedTN;


            return View(dANHGIADICHVU);
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
        // GET: HOADON/Create
        public ActionResult ThemPhanHoiDichVu()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLap = date;
            String idDGDV = TaoMaPhanHoiDV();
            ViewBag.MaDGDV = idDGDV;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin");

            if (Session["DaLayThongTinTiepNhan"] != null)
            {
                ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
            }
            return View();
        }
        //Hàm được sử dụng sau khi nhấn nút
        public ActionResult TaoDGDV_LayThongTinTiepNhan(String lstMaTN)
        {
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(lstMaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            Session["selectedTiepNhan"] = selectedTN;


            //Check đã lấy thông tin xe hay chưa, có null không
            Session["DaLayThongTinTiepNhan"] = 3;

            return RedirectToAction("ThemPhanHoiDichVu");
        }

        //Lưu thông tin tiếp nhận vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemPhanHoiDichVu(String lstMaTN, int MucDoChatLuong, int MucDoDichVu, int MucDoThaiDo, String GopYKhac)
        {
            DANHGIADICHVU dANHGIADICHVU = new DANHGIADICHVU();
            if (ModelState.IsValid)
            {
                String id = TaoMaPhanHoiDV();
                dANHGIADICHVU.MaDGDV = id;
                dANHGIADICHVU.NgayLap = DateTime.Now;
                dANHGIADICHVU.DGDV_MaTN = lstMaTN;
                dANHGIADICHVU.MucDoChatLuong = MucDoChatLuong;
                dANHGIADICHVU.MucDoDichVu = MucDoDichVu;
                dANHGIADICHVU.MucDoThaiDo = MucDoThaiDo;
                dANHGIADICHVU.GopYKhac = GopYKhac;
                db.DANHGIADICHVUs.Add(dANHGIADICHVU);
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhanHoiDichVu");
            }
            ViewBag.DGDV_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "MaTN","TN000");
            return View(dANHGIADICHVU);
        }


        // GET: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        public ActionResult SuaThongTinPhanHoiDV(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHGIADICHVU dANHGIADICHVU = db.DANHGIADICHVUs.Find(id);
            if (dANHGIADICHVU == null)
            {
                return HttpNotFound();
            }

            
            DateTime date;
            if (dANHGIADICHVU.NgayLap != null)
            {
                date = (DateTime)dANHGIADICHVU.NgayLap;
                ViewBag.NgayLap = DateTime.Now;
            }
            ViewBag.DGDV_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN","MaTN" , dANHGIADICHVU.DGDV_MaTN);
            return View(dANHGIADICHVU);
        }



        // POST: THONGTINTIEPNHAN/SuaThongTinTiepNhan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTinPhanHoiDV(DANHGIADICHVU dANHGIADICHVU
            , String DGDV_MaTN, int MucDoChatLuong, int MucDoDichVu, int MucDoThaiDo, String GopYKhac)
        {
            if (ModelState.IsValid)
            {
                dANHGIADICHVU.NgayLap = DateTime.Now;
                dANHGIADICHVU.DGDV_MaTN = DGDV_MaTN;
                dANHGIADICHVU.MucDoChatLuong = MucDoChatLuong;
                dANHGIADICHVU.MucDoDichVu = MucDoDichVu;
                dANHGIADICHVU.MucDoThaiDo = MucDoThaiDo;
                dANHGIADICHVU.GopYKhac = GopYKhac;
                db.Entry(dANHGIADICHVU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LayDanhSachPhanHoiDichVu");
            }
            return View(dANHGIADICHVU);
        }

        // GET: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        public ActionResult XoaThongTinPhanHoiDV(string id, int check)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHGIADICHVU dANHGIADICHVU = db.DANHGIADICHVUs.Find(id);
            if (check == 0)
            {
                ViewBag.ThongBao = "!Lưu ý: Dữ liệu có liên quan đến các dữ liệu khác. Không thể xoá";
            }
            else
            {
                if (dANHGIADICHVU == null)
                {
                    return HttpNotFound();
                }
            }
            DateTime date;
            if (dANHGIADICHVU.NgayLap == DateTime.Now)
            {
                date = (DateTime)dANHGIADICHVU.NgayLap;
                ViewBag.NgayLap = date.ToString("yyyy/MM/dd");
            }
            return View(dANHGIADICHVU);
        }


        // POST: THONGTINTIEPNHAN/XoaThongTinTiepNhan/5
        [HttpPost, ActionName("XoaThongTinPhanHoiDV")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DANHGIADICHVU dANHGIADICHVU = db.DANHGIADICHVUs.Find(id);
            db.DANHGIADICHVUs.Remove(dANHGIADICHVU);
            db.SaveChanges();
            return RedirectToAction("LayDanhSachPhanHoiDichVu");
        }

 
        private String TaoMaPhanHoiDV()
        {
            String idDGDV = "";
            //Tạo mã nhà cung cấp String
            List<DANHGIADICHVU> lstDGDV = db.DANHGIADICHVUs.ToList();
            int countLst = lstDGDV.Count();
            if (countLst == 0)
            {
                idDGDV = "DV001";
            }
            else
            {
                DANHGIADICHVU lastDGDV = lstDGDV[countLst - 1];
                String lastMaDGDV = lastDGDV.MaDGDV;
                int lastMaDGDVNum = int.Parse(lastMaDGDV.Substring(2));
                int newMaDGDV = lastMaDGDVNum + 1;
                if (newMaDGDV < 10)
                {
                    idDGDV = "DV00" + newMaDGDV.ToString();
                }
                else { idDGDV = "DVN0" + newMaDGDV.ToString(); }
            }
            return (idDGDV);
        }

    }
}