using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
namespace MvcBookStore.Controllers
{
    public class BookStoreController : Controller
    {
        // Use DbContext to manage database
        QLBANSACH2Entities2 database = new QLBANSACH2Entities2();
        private List<SACH> LaySachMoi(int soluong)
        {
            // Sắp xếp sách theo ngày cập nhật giảm dần, lấy đúng số lượng sách cần
            // Chuyển qua dạng danh sách kết quả đạt được
            return database.SACHes.OrderByDescending(sach =>
            sach.Ngaycapnhat).Take(soluong).ToList();
        }
        // GET: BookStore
        public ActionResult Index(int? page)
        {
            int pageSize = 6; // Số sách mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là 1 nếu không có tham số

            var books = database.SACHes.OrderBy(s => s.Tensach).ToPagedList(pageNumber, pageSize); // Lấy danh sách sách và phân trang
            return View(books);
        }
        public ActionResult LayChuDe()
        {
            var dsChuDe = database.CHUDEs.ToList();
            return PartialView(dsChuDe);
        }

        public ActionResult SPTheoChuDe(int id, int? page)
        {
            int pageSize = 6; // Số sách mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là 1 nếu không có tham số

            // Lấy danh sách sách theo chủ đề và phân trang
            var dsSachTheoChuDe = database.SACHes
                .Where(sach => sach.MaCD == id)
                .OrderBy(s => s.Tensach) // Sắp xếp theo tên sách
                .ToPagedList(pageNumber, pageSize);

            // Truyền ID chủ đề hiện tại để sử dụng trong View
            ViewBag.CurrentId = id;
            ViewBag.CurrentAction = "SPTheoChuDe";

            return View("Index", dsSachTheoChuDe);
        }


        public ActionResult LayNhaXuatBan()
        {
            var dsNhaXB = database.NHAXUATBANs.ToList();
            return PartialView(dsNhaXB);
        }
        public ActionResult SPTheoNXB(int id, int? page)
        {
            int pageSize = 6; // Số sách mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là 1 nếu không có tham số

            // Lấy danh sách sách theo nhà xuất bản và phân trang
            var dsSachNXB = database.SACHes
                .Where(sach => sach.MaNXB == id)
                .OrderBy(s => s.Tensach) // Sắp xếp theo tên sách
                .ToPagedList(pageNumber, pageSize);

            // Truyền ID nhà xuất bản hiện tại để sử dụng trong View
            ViewBag.CurrentId = id;
            ViewBag.CurrentAction = "SPTheoNXB";

            return View("Index", dsSachNXB);
        }


        public ActionResult Details(int id)
        {
            //Lấy sách có mã vạch tương ứng
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            return View(sach);
        }
    }
}