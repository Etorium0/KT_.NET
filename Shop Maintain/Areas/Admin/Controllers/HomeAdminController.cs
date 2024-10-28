using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_Maintain.Models;
using X.PagedList;
using Shop_Maintain.Models.Authentication;

namespace Shop_Maintain.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [Authentication]  
    public class HomeAdminController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();

        }
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstSanpham = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(lstSanpham, pageNumber, pageSize);
            return View(lst);
        }
        [Route("themsanphammoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            return View();
        }
        [Route("themsanphammoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.TDanhMucSps.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }
            return View(sanPham);
        }

        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(string maSanPham)
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            var sanPham = db.TDanhMucSps.Find(maSanPham);
            return View(sanPham);
        }
        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            return View(sanPham);
        }

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(string maSanPham)
        {
            TempData["Message"] = "";
            var chiTietSanPham = db.TChiTietSanPhams.Where(x => x.MaSp == maSanPham).ToList();
            if (chiTietSanPham.Count() > 0)
            {
                TempData["Message"] = "Không xóa được sản phẩm này";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin"); 
            }
            var anhSanPhams = db.TAnhSps.Where(x => x.MaSp == maSanPham);
            if (anhSanPhams.Any())
                db.RemoveRange(anhSanPhams);
            db.Remove(db.TDanhMucSps.Find(maSanPham));
            db.SaveChanges();
            TempData["Message"] = "Sản phẩm đã được xóa";
            return RedirectToAction("DanhMucSanPham", "HomeAdmin");
        }

        [Route("danhsachnhanvien")]
        public IActionResult DanhSachNhanVien(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstNhanVien = db.TNhanViens.AsNoTracking().OrderBy(x => x.TenNhanVien);
            PagedList<TNhanVien> lst = new PagedList<TNhanVien>(lstNhanVien, pageNumber, pageSize);
            return View(lst);
        }

        [Route("themnhanvienmoi")]
        [HttpGet]
        public IActionResult ThemNhanVienMoi()
        {
            // Thêm các ViewBag cần thiết dựa trên cấu trúc SQL
            ViewBag.MaChucVu = new SelectList(db.TNhanViens.ToList(), "MaChucVu", "ChucVu");
            ViewBag.GhiChu = new SelectList(db.TNhanViens.Select(x => x.GhiChu).Distinct());
            ViewBag.AnhDaiDien = new SelectList(db.TNhanViens.Select(x => x.AnhDaiDien).Distinct());
            return View();
        }

        [Route("themnhanvienmoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNhanVien(TNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra và gán các giá trị từ form
                nhanVien.Username = nhanVien.Username?.Trim();
                nhanVien.TenNhanVien = nhanVien.TenNhanVien?.Trim();
                nhanVien.DiaChi = nhanVien.DiaChi?.Trim();
                
                // Thêm nhân viên mới
                db.TNhanViens.Add(nhanVien);
                db.SaveChanges();
                return RedirectToAction("DanhSachNhanVien");
            }
            // Nếu ModelState không hợp lệ, load lại các ViewBag
            ViewBag.MaChucVu = new SelectList(db.TNhanViens.ToList(), "MaChucVu", "ChucVu");
            ViewBag.GhiChu = new SelectList(db.TNhanViens.Select(x => x.GhiChu).Distinct());
            ViewBag.AnhDaiDien = new SelectList(db.TNhanViens.Select(x => x.AnhDaiDien).Distinct());
            return View(nhanVien);
        }

        [Route("suanhanvien")]
        [HttpGet]
        public IActionResult SuaNhanVien(string maNhanVien)
        {
            ViewBag.MaChucVu = new SelectList(db.TNhanViens.ToList(), "MaChucVu", "TenChucVu");
            var nhanVien = db.TNhanViens.Find(maNhanVien);
            return View(nhanVien);
        }

        [Route("suanhanvien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNhanVien(TNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachNhanVien");
            }
            return View(nhanVien);
        }

        [Route("xoanhanvien")]
        [HttpGet]
        public IActionResult XoaNhanVien(string maNhanVien)
        {
            TempData["Message"] = "";
            var nhanVien = db.TNhanViens.Find(maNhanVien);
            if (nhanVien != null)
            {
                db.TNhanViens.Remove(nhanVien);
                db.SaveChanges();
                TempData["Message"] = "Nhân viên đã được xóa";
            }
            return RedirectToAction("DanhSachNhanVien");
        }

    }
}
