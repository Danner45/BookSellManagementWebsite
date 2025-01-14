using Project_64130005.Models;
using Project_64130005.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Project_64130005.Areas.Admin_64130005.Controllers
{
    public class HomeController : Controller
    {
        private readonly Project_64130005Entities1 db = new Project_64130005Entities1();
        // GET: Admin_64130005/Home
        public ActionResult Index()
        {
            if (Session["NV"] == null)
            {
                return RedirectToAction("Login_64130005", "Home");
            }
            var soLuongTheoThang = db.DonHangs
                .Where(o => o.MaTrangThai == "Hoàn thành")
                .GroupBy(o => new { o.NgayDat.Value.Month, o.NgayDat.Value.Year })
                .Select(g => new
                {
                    Thang = g.Key.Month,
                    Nam = g.Key.Year,
                    SoLuong = g.Sum(o => o.ChiTietDHs.Sum(ct => ct.SoLuong))
                }).OrderBy(g => g.Nam).ThenBy(g => g.Thang).ToList();

            var soTienTheoThang = db.DonHangs
                .Where(o => o.MaTrangThai == "Hoàn thành")
                .GroupBy(o => new { o.NgayDat.Value.Month, o.NgayDat.Value.Year })
                .Select(g => new
                {
                    Thang = g.Key.Month,
                    Nam = g.Key.Year,
                    TongTien = g.Sum(o => o.ChiTietDHs.Sum(ct => ct.ThanhTien))
                }).OrderBy(g => g.Nam).ThenBy(g => g.Thang).ToList();

            ViewBag.ProductCounts = soLuongTheoThang;
            ViewBag.Revenues = soTienTheoThang;
            return View();
        }

        [HttpGet]
        public ActionResult getDataDonHang_64130005() {
            bool proxyCreation = db.Configuration.ProxyCreationEnabled;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var result = db.DonHangs.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            finally
            {
                db.Configuration.ProxyCreationEnabled=proxyCreation;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_64130005(NhanVien objUser) {
            if(ModelState.IsValid)
            {
                string passHash = ComonUtils_64130005.HashPassword(objUser.MatKhau);
                var obj = db.NhanViens.Where(a => a.Email.Equals(objUser.Email) && a.MatKhau.Equals(passHash)).FirstOrDefault();
                if (obj != null && obj.MaLoaiNV == "QL")
                {
                    Session["NV"] = obj;
                    return RedirectToAction("Index", "Home");
                }
                else if (obj != null && obj.MaLoaiNV == "NV")
                {
                    Session["NV"] = obj;
                    return RedirectToAction("Index", "DonHang_64130005");
                }
                else
                {
                    ModelState.AddModelError("", "Không phải là tài khoản của quản trị viên");
                }
            }
            return View(objUser);
        }

        [HttpGet]
        public ActionResult Login_64130005()
        {
            if (Session["NV"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Logout_64130005()
        {
            Session["NV"] = null;
            return RedirectToAction("Login_64130005", "Home");
        }
    }
}