using MvcBookStore.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBookStore.Controllers
{
    public class NguoiDungController : Controller
    {
        // Sử dụng DbContext để quản lý cơ sở dữ liệu
        QLBANSACH2Entities2 database = new QLBANSACH2Entities2();

        // GET: NguoiDung/DangKy
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: NguoiDung/DangKy
        [HttpPost]
        public ActionResult DangKy(KHACHHANG kh)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(kh.TenDN))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(kh.Matkhau))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (string.IsNullOrEmpty(kh.Email))
                    ModelState.AddModelError(string.Empty, "Email không được để trống");
                if (string.IsNullOrEmpty(kh.DienthoaiKH))
                    ModelState.AddModelError(string.Empty, "Điện thoại không được để trống");

                // Kiểm tra mật khẩu và xác nhận mật khẩu
                if (kh.Matkhau != Request.Form["MatkhauConfirm"])
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu và xác nhận mật khẩu không khớp");
                }

                // Kiểm tra xem tên đăng nhập đã tồn tại trong cơ sở dữ liệu hay chưa
                var khachhang = database.KHACHHANGs.FirstOrDefault(k => k.TenDN == kh.TenDN);
                if (khachhang != null)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản đã được đăng ký");
                }

                // Nếu tất cả kiểm tra hợp lệ
                if (ModelState.IsValid)
                {
                    // Lưu khách hàng vào cơ sở dữ liệu
                    database.KHACHHANGs.Add(kh);
                    database.SaveChanges();

                    // Sau khi đăng ký thành công, chuyển hướng đến trang đăng nhập
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                }
                else
                {
                    return View(kh);
                }    
            }
            // Chuyển hướng tới trang đăng nhập
            return RedirectToAction("DangNhap", "NguoiDung");
        }
                    

        // GET: NguoiDung/DangNhap
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(KHACHHANG kh)
        {
            // Kiểm tra tính hợp lệ của Model
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập có rỗng không
                if (string.IsNullOrEmpty(kh.TenDN))
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống.");
                }

                // Kiểm tra xem mật khẩu có rỗng không
                if (string.IsNullOrEmpty(kh.Matkhau))
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống.");
                }

                // Nếu ModelState vẫn hợp lệ sau khi kiểm tra
                if (ModelState.IsValid)
                {
                    // Tìm khách hàng trong cơ sở dữ liệu với tên đăng nhập và mật khẩu hợp lệ
                    var khachhang = database.KHACHHANGs
                        .FirstOrDefault(k => k.TenDN == kh.TenDN && k.Matkhau == kh.Matkhau);

                    // Kiểm tra xem có khách hàng nào tìm thấy không
                    if (khachhang != null)
                    {
                        // Đăng nhập thành công, lưu thông tin người dùng vào session
                        Session["Account"] = khachhang;

                        // Lưu thông báo vào TempData
                        TempData["SuccessMessage"] = "Đăng nhập thành công!";

                        // Chuyển hướng đến trang chính (BookStore) và đảm bảo có layout
                        return RedirectToAction("Index", "BookStore");
                    }
                    else
                    {
                        // Thông báo lỗi nếu tên đăng nhập hoặc mật khẩu không chính xác
                        ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                    }
                }
            }

            // Nếu có lỗi, trả về view với model hiện tại để hiển thị lỗi
            return View();
        }

        // Phương thức mã hóa mật khẩu (ví dụ dùng SHA256 hoặc các phương thức khác)
        private string MaHoaMatKhau(string matKhau)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(matKhau));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Mã hóa mật khẩu thành dạng hex
            }
        }
        // GET: NguoiDung/ThongTin
        [HttpGet]
        public ActionResult ThongTin()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung"); // Chuyển hướng đến trang Đăng Nhập nếu chưa đăng nhập
            }

            var khachHang = (KHACHHANG)Session["Account"];
            return View(khachHang);
        }

        // POST: NguoiDung/CapNhatThongTin
        [HttpPost]
        public ActionResult CapNhatThongTin(KHACHHANG khachHang)
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            if (ModelState.IsValid)
            {
                var kh = database.KHACHHANGs.Find(khachHang.MaKH); // Giả sử bạn có trường MaKH làm khóa chính
                if (kh != null)
                {
                    kh.Email = khachHang.Email;
                    kh.DienthoaiKH = khachHang.DienthoaiKH;
                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                }
            }

            return RedirectToAction("ThongTin");
        }

        // Đăng xuất
        public ActionResult DangXuat()
        {
            Session["Account"] = null; // Xóa session
            return RedirectToAction("Index", "BookStore"); // Quay lại trang chủ
        }
    }
}
