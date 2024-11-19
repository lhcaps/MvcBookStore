using MvcBookStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        // Use DbContext to manage database
        QLBANSACH2Entities2 database = new QLBANSACH2Entities2();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(ADMIN admin)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(admin.UserAdmin))
                    ModelState.AddModelError(string.Empty, "User name không được để trống");
            if (string.IsNullOrEmpty(admin.PassAdmin))
                    ModelState.AddModelError(string.Empty, "Password không được để trống");
                    //Kiểm tra có admin này hay chưa
                    var adminDB = database.ADMINs.FirstOrDefault(ad => ad.UserAdmin ==
                    admin.UserAdmin && ad.PassAdmin == admin.PassAdmin);
                if (adminDB == null)
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                else
                {
                    Session["Admin"] = adminDB;

                    ViewBag.ThongBao = "Đăng nhập admin thành công";
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }
        public ActionResult Sach(int? page)
        {
            var dsSach = database.SACHes.ToList();
            //Tạo biến cho biết số sách mỗi trang
            int pageSize = 7;
            //Tạo biến số trang
            int pageNum = (page ?? 1);
            return View(dsSach.OrderBy(sach => sach.Masach).ToPagedList(pageNum,
            pageSize));
        }

        [HttpGet]
        public ActionResult ThemSach()
        {
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        public ActionResult ThemSach(SACH sach, HttpPostedFileBase Hinhminhoa)
        {
            var filename = Path.GetFileName(Hinhminhoa.FileName);

            var path = Path.Combine(Server.MapPath("~Images"), filename);

            if (System.IO.File.Exists(path))
            {
                ViewBag.ThongBao = "Hình đã tồn tại";
            }    
            else
            {
                Hinhminhoa.SaveAs(path);
                database.SACHes.Add(sach);
                database.SaveChanges();
            }
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");
            return View();
        }

        public ActionResult ChiTietSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }   
            return View(sach);
        }

        [HttpGet]
        public ActionResult SuaSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Gửi dữ liệu cho dropdown Chủ Đề và Nhà Xuất Bản
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }

        [HttpPost]
        public ActionResult SuaSach(SACH sach, HttpPostedFileBase Hinhminhoa)
        {
            if (ModelState.IsValid)
            {
                var sachDB = database.SACHes.FirstOrDefault(s => s.Masach == sach.Masach);
                if (sachDB != null)
                {
                    sachDB.Tensach = sach.Tensach;
                    sachDB.Donvitinh = sach.Donvitinh;
                    sachDB.Donggia = sach.Donggia;
                    sachDB.Mota = sach.Mota;
                    sachDB.MaCD = sach.MaCD;
                    sachDB.MaNXB = sach.MaNXB;
                    sachDB.Ngaycapnhat = sach.Ngaycapnhat;

                    // Kiểm tra và cập nhật hình minh họa
                    if (Hinhminhoa != null && Hinhminhoa.ContentLength > 0)
                    {
                        var filename = Path.GetFileName(Hinhminhoa.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images"), filename);

                        // Lưu hình mới nếu chưa tồn tại
                        if (!System.IO.File.Exists(path))
                        {
                            Hinhminhoa.SaveAs(path);
                        }

                        sachDB.Hinhminhoa = filename;
                    }

                    database.SaveChanges();
                    return RedirectToAction("Sach");
                }
            }

            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }

        public ActionResult XoaSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            database.SACHes.Remove(sach);
            database.SaveChanges();
            return RedirectToAction("Sach");
        }

        //Chủ đề
        public ActionResult QuanLyChuDe()
        {
            var dsChuDe = database.CHUDEs.ToList();
            return View(dsChuDe);
        }
        [HttpGet]
        public ActionResult ThemChuDe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemChuDe(CHUDE chuDe)
        {
            if (ModelState.IsValid)
            {
                database.CHUDEs.Add(chuDe);
                database.SaveChanges();
                return RedirectToAction("QuanLyChuDe");
            }
            return View(chuDe);
        }

        public ActionResult XoaChuDe(int id)
        {
            var chuDe = database.CHUDEs.Find(id);
            if (chuDe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            database.CHUDEs.Remove(chuDe);
            database.SaveChanges();
            return RedirectToAction("QuanLyChuDe");
        }

        public ActionResult SuaChuDe(int id)
        {
            var chuDe = database.CHUDEs.Find(id);
            if (chuDe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chuDe);
        }

        [HttpPost]
        public ActionResult SuaChuDe(CHUDE chuDe)
        {
            if (ModelState.IsValid)
            {
                var chuDeDB = database.CHUDEs.Find(chuDe.MaCD);
                if (chuDeDB != null)
                {
                    chuDeDB.TenChuDe = chuDe.TenChuDe;
                    database.SaveChanges();
                    return RedirectToAction("QuanLyChuDe");
                }
            }
            return View(chuDe);
        }

        public ActionResult QuanLyNhaXuatBan()
        {
            var dsNXB = database.NHAXUATBANs.ToList();
            return View(dsNXB);
        }
        [HttpGet]
        public ActionResult ThemNhaXuatBan()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemNhaXuatBan(NHAXUATBAN nhaXuatBan)
        {
            if (ModelState.IsValid)
            {
                database.NHAXUATBANs.Add(nhaXuatBan);
                database.SaveChanges();
                return RedirectToAction("QuanLyNhaXuatBan");
            }
            return View(nhaXuatBan);
        }

        public ActionResult XoaNhaXuatBan(int id)
        {
            var nhaXuatBan = database.NHAXUATBANs.Find(id);
            if (nhaXuatBan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            database.NHAXUATBANs.Remove(nhaXuatBan);
            database.SaveChanges();
            return RedirectToAction("QuanLyNhaXuatBan");
        }

        public ActionResult SuaNhaXuatBan(int id)
        {
            var nhaXuatBan = database.NHAXUATBANs.Find(id);
            if (nhaXuatBan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nhaXuatBan);
        }

        [HttpPost]
        public ActionResult SuaNhaXuatBan(NHAXUATBAN nhaXuatBan)
        {
            if (ModelState.IsValid)
            {
                var nhaXuatBanDB = database.NHAXUATBANs.Find(nhaXuatBan.MaNXB);
                if (nhaXuatBanDB != null)
                {
                    nhaXuatBanDB.TenNXB = nhaXuatBan.TenNXB;
                    nhaXuatBanDB.Diachi = nhaXuatBan.Diachi;
                    nhaXuatBanDB.DienjThoai = nhaXuatBan.DienjThoai;

                    database.SaveChanges();
                    return RedirectToAction("QuanLyNhaXuatBan");
                }
            }
            return View(nhaXuatBan);
        }
    }
}