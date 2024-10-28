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
            // Kiểm tra xem có tài khoản nào không
            if (!db.TUsers.Any())
            {
                TempData["Message"] = "Không có tài khoản nào. Vui lòng tạo tài khoản trước khi thêm nhân viên.";
                return RedirectToAction("QuanLyTaiKhoan"); // Chuyển hướng đến trang quản lý tài khoản
            }

            // Thêm các ViewBag cần thiết dựa trên cấu trúc SQL
            ViewBag.ChucVu = new SelectList(db.TNhanViens.ToList(), "MaChucVu", "ChucVu");
            ViewBag.GhiChu = new SelectList(db.TNhanViens.Select(x => x.GhiChu).Distinct());
            ViewBag.AnhDaiDien = new SelectList(db.TNhanViens.Select(x => x.AnhDaiDien).Distinct());
            return View();
        }

        [Route("themnhanvienmoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNhanVienMoi(TNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem username đã tồn tại trong tUser chưa
                var existingUser = db.TUsers.FirstOrDefault(x => x.Username == nhanVien.Username);
                if (existingUser == null)
                {
                    // Nếu không tồn tại, tạo tài khoản mới
                    TUser newUser = new TUser
                    {
                        Username = nhanVien.Username,
                        // Gán các thuộc tính khác nếu cần
                    };
                    db.TUsers.Add(newUser);
                    db.SaveChanges(); // Lưu tài khoản mới vào tUser
                }

                // Thêm nhân viên mới
                db.TNhanViens.Add(nhanVien);
                db.SaveChanges(); // Lưu nhân viên mới vào tNhanVien

                return RedirectToAction("DanhSachNhanVien");
            }

            // Nếu ModelState không hợp lệ, load lại các ViewBag
            ViewBag.ChucVu = new SelectList(db.TNhanViens.ToList(), "MaChucVu", "ChucVu");
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

        [Route("quanlytaikhoan")]
        public IActionResult QuanLyTaiKhoan(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstTaiKhoan = db.TUsers.AsNoTracking().OrderBy(x => x.Username);
            PagedList<TUser> lst = new PagedList<TUser>(lstTaiKhoan, pageNumber, pageSize);
            return View(lst);
        }

        [Route("themtaikhoanmoi")]
        [HttpGet]
        public IActionResult ThemTaiKhoanMoi()
        {
            return View();
        }

        [Route("themtaikhoanmoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemTaiKhoanMoi(TUser user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUser = db.TUsers.FirstOrDefault(x => x.Username == user.Username);
                if (existingUser != null)
                {
                    TempData["Message"] = "Username đã tồn tại";
                    return View(user);
                }

                db.TUsers.Add(user);
                db.SaveChanges();
                return RedirectToAction("QuanLyTaiKhoan");
            }
            return View(user);
        }

        [Route("suataikhoan")]
        [HttpGet]
        public IActionResult SuaTaiKhoan(string username)
        {
            var user = db.TUsers.Find(username);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [Route("suataikhoan")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaTaiKhoan(TUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("QuanLyTaiKhoan");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.TUsers.Any(e => e.Username == user.Username))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        [Route("xoataikhoan")]
        [HttpGet]
        public IActionResult XoaTaiKhoan(string username)
        {
            TempData["Message"] = "";
            var user = db.TUsers.Find(username);
            if (user != null)
            {
                try
                {
                    db.TUsers.Remove(user);
                    db.SaveChanges();
                    TempData["Message"] = "Tài khoản đã được xóa";
                }
                catch (Exception)
                {
                    TempData["Message"] = "Không thể xóa tài khoản này";
                }
            }
            return RedirectToAction("QuanLyTaiKhoan");
        }

    }
}
