using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBookStore.Models;

namespace MvcBookStore.Controllers
{
    public class GioHangController : Controller
    {
        QLBANSACH2Entities2 data = new QLBANSACH2Entities2();
        public List<MatHangMua> LayGioHang()
        {
            List<MatHangMua> gioHang = Session["GioHang"] as List<MatHangMua>;
            if (gioHang == null)
            {
                gioHang = new List<MatHangMua>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }
        public ActionResult ThemSanPhamVaoGio(int MaSp)
        {
            List<MatHangMua> gioHang = LayGioHang();
            MatHangMua sanPham = gioHang.FirstOrDefault(s => s.Masach == MaSp);
            if (sanPham == null)
            {
                sanPham = new MatHangMua(MaSp);
                gioHang.Add(sanPham);
            }
            else
            {
                sanPham.SoLuong++;
            }
            return RedirectToAction("Details", "BookStore", new { id = MaSp });
        }
        private int TinhTongSL()
        {
            int tongSL = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                tongSL = gioHang.Sum(sp => sp.SoLuong);
            return tongSL;
        }
        private double TinhTongTien()
        {
            double TongTien = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                TongTien = gioHang.Sum(sp => sp.ThanhTien());
            return TongTien;
        }
        public ActionResult HienThiGioHang()
        {
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult XoaMatHang(int MaSP)
        {
            // Lay danh sach gio hang
            List<MatHangMua> gioHang = LayGioHang();

            // Tim san pham trong gio hang
            var sanpham = gioHang.FirstOrDefault(s => s.Masach == MaSP);

            // Neu san pham ton tai trong gio hang, xoa san pham do
            if (sanpham != null)
            {
                gioHang.Remove(sanpham);
            }

            // Neu gio hang rong sau khi xoa, chuyen ve trang Index (BookStore)
            if (gioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }

            // Neu gio hang con san pham, quay ve trang hien thi gio hang
            return RedirectToAction("HienThiGioHang");
        }
        public ActionResult CapNhatMatHang(int MaSP, int SoLuong)
        {
            List<MatHangMua> gioHang = LayGioHang();
            var sanpham = gioHang.FirstOrDefault(s => s.Masach == MaSP);
            if (sanpham != null)
            {
                sanpham.SoLuong = SoLuong;
            }
            return RedirectToAction("HienThiGioHang");
        }
        public ActionResult DatHang()
        {
            // Kiểm tra xem tên người dùng trên navbar đã được cập nhật hay chưa (Xem ViewBag.Username)
            if (string.IsNullOrEmpty(ViewBag.Username))
            {
                return RedirectToAction("DangNhap", "NguoiDung"); // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            }

            List<MatHangMua> gioHang = LayGioHang(); // Lấy giỏ hàng từ session

            // Kiểm tra giỏ hàng có sản phẩm hay không
            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore"); // Nếu giỏ hàng rỗng, chuyển về trang chủ
            }

            // Nếu thỏa 2 điều kiện trên, hiển thị view thông tin đặt hàng
            // Cung cấp thông tin giỏ hàng và tổng số lượng, tổng tiền qua ViewData
            ViewData["TongSL"] = TinhTongSL(); // Tính tổng số lượng
            ViewData["TongTien"] = TinhTongTien(); // Tính tổng tiền

            return View(gioHang); // Trả về view thông tin đặt hàng với giỏ hàng
        }

        QLBANSACH2Entities2 database = new QLBANSACH2Entities2();
        public ActionResult DongYDatHang()
        {
            KHACHHANG khach = Session["TaiKhoan"] as KHACHHANG;
            List<MatHangMua> gioHang = LayGioHang(); // Lấy giỏ hàng từ session

            // Kiểm tra giỏ hàng có sản phẩm hay không
            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore"); // Nếu giỏ hàng rỗng, chuyển về trang chủ
            }

            // Tạo đơn đặt hàng
            DONDATHANG DonHang = new DONDATHANG
            {
                MaKH = khach.MaKH,
                NgayDH = DateTime.Now,
                Trigia = (decimal)TinhTongTien(), // Tính tổng tiền cho đơn hàng
                Dagiao = false,
                Tennguoinhan = khach.HoTenKH,
                Diachinhan = khach.DiachiKH,
                Dienthoainhan = khach.DienthoaiKH,
                HTThanhtoan = false, // Chưa thanh toán
                HTGiaohang = false // Chưa giao hàng
            };

            try
            {
                // Lưu đơn hàng vào cơ sở dữ liệu
                database.DONDATHANGs.Add(DonHang);
                database.SaveChanges();

                // Lưu chi tiết đơn hàng
                foreach (var sanpham in gioHang)
                {
                    CTDATHANG chitiet = new CTDATHANG
                    {
                        SoDH = DonHang.SoDH,
                        Masach = sanpham.Masach,
                        Soluong = sanpham.SoLuong,
                        Donggia = (decimal)sanpham.DonGia
                    };
                    database.CTDATHANGs.Add(chitiet);
                }
                database.SaveChanges();

                // Xóa giỏ hàng trong session sau khi đặt hàng thành công
                Session["GioHang"] = null;

                // Chuyển hướng đến trang hoàn thành đơn hàng
                return RedirectToAction("HoanThanhDonHang"); // Chuyển hướng đến view hoàn thành đơn hàng
            }
            catch (Exception ex)
            {
                // Nếu có lỗi trong quá trình lưu đơn hàng, bạn có thể log lỗi và thông báo cho người dùng
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tạo đơn hàng: " + ex.Message;
                return View("Error"); // Tạo một view thông báo lỗi hoặc quay lại trang giỏ hàng
            }
        }

    }
}