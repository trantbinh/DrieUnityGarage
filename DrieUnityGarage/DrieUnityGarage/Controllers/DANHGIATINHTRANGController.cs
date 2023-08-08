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
    public class DANHGIATINHTRANGController : Controller
    {
        private DrieUnityGarageEntities db = new DrieUnityGarageEntities();
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

        // GET: DANHGIATINHTRANG
        public ActionResult Index()
        {
            Session.Remove("DaLayThongTinTiepNhan");
            var dANHGIATINHTRANGs = db.DANHGIATINHTRANGs.Include(d => d.THONGTINTIEPNHAN);

            return View(dANHGIATINHTRANGs.ToList());
        }

        // GET: DANHGIATINHTRANG/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHGIATINHTRANG dANHGIATINHTRANG = db.DANHGIATINHTRANGs.Find(id);
            if (dANHGIATINHTRANG == null)
            {
                return HttpNotFound();
            }
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;

            String idTN = TaoMaDanhGia();
            ViewBag.MaDG = idTN;

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(dANHGIATINHTRANG.DG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan = selectedTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            ViewBag.KhachHang = kh;
            ViewBag.BienSoXe = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            return View(dANHGIATINHTRANG);
        }

        // GET: DANHGIATINHTRANG/Create
        public ActionResult Create()
        {
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;

            String idTN = TaoMaDanhGia();
            ViewBag.MaDG = idTN;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin");

            if (Session["DaLayThongTinTiepNhan"] != null)
            {
                ViewBag.selectedTiepNhan = Session["selectedTiepNhan"];
                ViewBag.KhachHang = Session["KhachHang"];
                ViewBag.BienSoXe = Session["BienSoXe"];
                ViewBag.NhanVien = Session["NhanVien"];
            }

            return View();
        }
        //Hàm được sử dụng sau khi nhấn nút
        public ActionResult TaoDG_LayThongTinTiepNhan(String lstMaTN)
        {
            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(lstMaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            Session["selectedTiepNhan"] = selectedTN;

            //Thông tin cần lưu của tiếp nhận
            Session["MaHD"] = TaoMaDanhGia();
            Session["MaTN"] = lstMaTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            Session["KhachHang"] = kh;
            Session["BienSoXe"] = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            Session["NhanVien"] = nv;

            //Check đã lấy thông tin xe hay chưa, có null không
            Session["DaLayThongTinTiepNhan"] = 3;

            return RedirectToAction("Create");
        }

        // POST: DANHGIATINHTRANG/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDG,NgayLap,DG_MaTN,NgoaiThat,KinhChanGio,GatMua,HeThongDen,HeThongDieuHoa,DauDongCo,DauPhanh,DauTroLucLai,NuocRuaKinh,NuocLamMat,LocGioDongCo,LocGioDieuHoa,Acquy,HeThongSac,DauCap,MaPhanh,DiaPhanh,HeThongLai,HeThongTreo,BanhTraiTruoc,BanhTraiSau,BanhPhaiTruoc,BanhPhaiSau,CanChinhThuocLai,CanBangDong,DaoLop,ThayThe,GhiChu")] DANHGIATINHTRANG dANHGIATINHTRANG)
        {
            if (ModelState.IsValid)
            {
                dANHGIATINHTRANG.NgayLap = DateTime.Now;
                dANHGIATINHTRANG.MaDG = TaoMaDanhGia();
                dANHGIATINHTRANG.DG_MaTN = Session["MaTN"].ToString();
                db.DANHGIATINHTRANGs.Add(dANHGIATINHTRANG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DG_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", dANHGIATINHTRANG.DG_MaTN);
            return View(dANHGIATINHTRANG);
        }

        // GET: DANHGIATINHTRANG/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHGIATINHTRANG dANHGIATINHTRANG = db.DANHGIATINHTRANGs.Find(id);
            if (dANHGIATINHTRANG == null)
            {
                return HttpNotFound();
            }
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;

            String idTN = TaoMaDanhGia();
            ViewBag.MaDG = idTN;

            List<THONGTINTIEPNHANXE> lstTiepNhan = LayDanhSachTiepNhanDB();
            ViewBag.lstMaTN = new SelectList(lstTiepNhan, "MaTN", "FullThongTin", dANHGIATINHTRANG.DG_MaTN);

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(dANHGIATINHTRANG.DG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan = selectedTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            ViewBag.KhachHang = kh;
            ViewBag.BienSoXe = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            return View(dANHGIATINHTRANG);
        }

        // POST: DANHGIATINHTRANG/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDG,NgayLap,DG_MaTN,NgoaiThat,KinhChanGio,GatMua,HeThongDen,HeThongDieuHoa,DauDongCo,DauPhanh,DauTroLucLai,NuocRuaKinh,NuocLamMat,LocGioDongCo,LocGioDieuHoa,Acquy,HeThongSac,DauCap,MaPhanh,DiaPhanh,HeThongLai,HeThongTreo,BanhTraiTruoc,BanhTraiSau,BanhPhaiTruoc,BanhPhaiSau,CanChinhThuocLai,CanBangDong,DaoLop,ThayThe,GhiChu")] DANHGIATINHTRANG dANHGIATINHTRANG, String lstMaTN)
        {
            if (ModelState.IsValid)
            {
                dANHGIATINHTRANG.DG_MaTN = lstMaTN;
                db.Entry(dANHGIATINHTRANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DG_MaTN = new SelectList(db.THONGTINTIEPNHANs, "MaTN", "TN_MaKH", dANHGIATINHTRANG.DG_MaTN);
            return View(dANHGIATINHTRANG);
        }

        // GET: DANHGIATINHTRANG/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHGIATINHTRANG dANHGIATINHTRANG = db.DANHGIATINHTRANGs.Find(id);
            if (dANHGIATINHTRANG == null)
            {
                return HttpNotFound();
            }
            String date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.NgayLapHD = date;

            String idTN = TaoMaDanhGia();
            ViewBag.MaDG = idTN;

            //Lấy ra thông tin của khách hàng có mã khách hàng là từ dropdown
            THONGTINTIEPNHANXE TTTN = new THONGTINTIEPNHANXE(dANHGIATINHTRANG.DG_MaTN);

            //Tạo 1 String chứa các thông tin của khách hàng để hiển thị
            String selectedTN = TTTN.MaTN + " - " + TTTN.MaKH + " - " + TTTN.BienSoXe;

            ViewBag.selectedTiepNhan = selectedTN;

            //Lấy ra các thông tin cần thiết
            String tenKH = db.KHACHHANGs.Find(TTTN.MaKH).HoTenKH;
            String kh = TTTN.MaKH + " - " + tenKH;
            ViewBag.KhachHang = kh;
            ViewBag.BienSoXe = TTTN.BienSoXe;

            String tenNV = db.NHANVIENs.Find(TTTN.MaNV).HoTenNV; ;
            String nv = TTTN.MaNV + " - " + tenNV;
            ViewBag.NhanVien = nv;

            return View(dANHGIATINHTRANG);
        }

        // POST: DANHGIATINHTRANG/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DANHGIATINHTRANG dANHGIATINHTRANG = db.DANHGIATINHTRANGs.Find(id);
            db.DANHGIATINHTRANGs.Remove(dANHGIATINHTRANG);
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

        private String TaoMaDanhGia()
        {
            String idHD = "";
            //Tạo mã nhà cung cấp String
            List<DANHGIATINHTRANG> lstHD = db.DANHGIATINHTRANGs.ToList();
            int countLst = lstHD.Count();
            if (countLst == 0)
            {
                idHD = "DG001";
            }
            else
            {
                DANHGIATINHTRANG lastHD = lstHD[countLst - 1];
                String lastMaHD = lastHD.MaDG;
                int lastMaHDNum = int.Parse(lastMaHD.Substring(2));
                int newMaHD = lastMaHDNum + 1;
                if (newMaHD < 10)
                {
                    idHD = "DG00" + newMaHD.ToString();
                }
                else { idHD = "DG0" + newMaHD.ToString(); }
            }
            return (idHD);
        }

    }
}
