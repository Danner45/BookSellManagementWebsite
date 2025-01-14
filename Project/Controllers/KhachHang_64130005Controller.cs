using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_64130005.Models;
using System.IO;
using System.Diagnostics;
using Project_64130005.Utils;
using System.Data.Entity.Validation;

namespace Project_64130005.Controllers
{
    public class KhachHang_64130005Controller : Controller
    {
        private Project_64130005Entities1 db = new Project_64130005Entities1();

        // GET: KhachHang_64130005
        public async Task<ActionResult> Index()
        {
            return View();
        }

        public ActionResult Register_64130005()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register_64130005([Bind(Include = "MatKhau,ConfirmPassword,Emai,TenKH,HoKH")] KhachHang khachhang)
        {
            if (ModelState.IsValid)
            {
                var check = db.KhachHangs.FirstOrDefault(s => s.Emai == khachhang.Emai);
                if (check == null)
                {
                    string maKH = LayMaKH();
                    String anh = "user.jpg";
                    khachhang.AnhKH = anh;
                    khachhang.MaKH = maKH;
                    khachhang.MatKhau = ComonUtils_64130005.HashPassword(khachhang.MatKhau);
                    khachhang.ConfirmPassword = ComonUtils_64130005.HashPassword(khachhang.ConfirmPassword);
                    Debug.WriteLine(khachhang.MatKhau);
                    Debug.WriteLine(khachhang.ConfirmPassword);
                    try
                    {
                        db.KhachHangs.Add(khachhang);
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Debug.WriteLine($"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                            }
                        }
                        throw;

                    }
                    return RedirectToAction("Login_64130005");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login_64130005()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_64130005(string emai, string matKhau, NhanVien objUser)
        {
            string passHash = ComonUtils_64130005.HashPassword(matKhau); // Mã hóa mật khẩu đã nhập

            if (string.IsNullOrEmpty(emai) || string.IsNullOrEmpty(matKhau))
            {
                Debug.WriteLine("Hãy nhập email và mật khẩu");
                ViewBag.error = "Email và mật khẩu không được để trống.";
                return View();
            }

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Email: " + emai + " Pass: " + passHash); // In ra giá trị passHash để kiểm tra

                // Kiểm tra tài khoản khách hàng (Customer)
                KhachHang kh = db.KhachHangs.FirstOrDefault(s => s.Emai.Equals(emai, StringComparison.OrdinalIgnoreCase) && s.MatKhau.Equals(passHash));
                if (kh != null)
                {
                    Session["Taikhoan"] = kh;
                    Session["NV"] = null;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Debug.WriteLine("Khách hàng không tồn tại hoặc mật khẩu không khớp.");
                    ViewBag.error = "Sai email hoặc mật khẩu khách hàng";
                }

                // Kiểm tra tài khoản nhân viên (Staff)
                NhanVien nv = db.NhanViens.FirstOrDefault(s => s.Email.Equals(emai, StringComparison.OrdinalIgnoreCase) && s.MatKhau.Equals(passHash));
                if (nv != null)
                {
                    Debug.WriteLine("Staff Login Successful");
                    Session["NV"] = nv;
                    Session["Taikhoan"] = null;
                    return RedirectToAction("Index", "Home", new { area = "Admin_64130005" });
                }
                else
                {
                    Debug.WriteLine("Nhân viên không tồn tại hoặc mật khẩu không khớp.");
                    ViewBag.error = "Sai email hoặc mật khẩu nhân viên";
                }
            }

            return View();
        }
        public static void RegisterAreas(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_64130005_default", // Tên của route
                "Admin_64130005/{controller}/{action}/{id}", // Cấu trúc URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Giá trị mặc định cho controller và action
            );
        }

        /*public ActionResult Login_64130005(string emai, string matKhau)
        {
            if (ModelState.IsValid)
            {
                string hash = ComonUtils_64130005.HashPassword(matKhau);
                KhachHang kh = db.KhachHangs.FirstOrDefault(s => s.Emai.Equals(emai) && s.MatKhau.Equals(hash));
                if (kh != null)
                {
                    Session["Taikhoan"] = kh;
                    Session["NV"] = null;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //ModelState.AddModelError("", "Sai email hoặc mật khẩu");
                    ViewBag.error = "Vui lòng thử lại, xin cảm ơn.";
                    return View();
                }
            }
            return View();
        }*/

        // GET: KhachHang_64130005/Details/5
        public ActionResult Details()
        {
            KhachHang kh = (KhachHang)Session["Taikhoan"];
            KhachHang khachHang = db.KhachHangs.Find(kh.MaKH);

            if (kh != null && khachHang != null)
            {
                return View(khachHang);
            }
            return HttpNotFound();
        }


        // POST: KhachHang_64130005/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        public ActionResult Edit(string MaKH)
        {
            KhachHang khsession = (KhachHang)Session["Taikhoan"];
            KhachHang kh = db.KhachHangs.Find(MaKH);
            kh.ConfirmPassword = kh.MatKhau;
            if (khsession.MaKH != kh.MaKH)
            {
                return HttpNotFound();

            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKH,HoKH,TenKH,Emai,MatKhau,AnhKH,DiaChi,NgaySinh,CCCD,SDT, MatKhau, ConfirmPassword")] KhachHang kh, HttpPostedFileBase file)
        {
            KhachHang khachhang = db.KhachHangs.Find(kh.MaKH);
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                            Server.MapPath("~/Images/KhachHang"), pic);
                    file.SaveAs(path);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                    khachhang.AnhKH = pic;
                }
                khachhang.TenKH = kh.TenKH;
                khachhang.HoKH = kh.HoKH;
                khachhang.Emai = kh.Emai;
                khachhang.DiaChi = kh.DiaChi;
                khachhang.ConfirmPassword = kh.ConfirmPassword;

                if (kh.NgaySinh != null)
                {
                    khachhang.NgaySinh = kh.NgaySinh;
                }
                if (kh.CCCD != null)
                {
                    khachhang.CCCD = kh.CCCD;
                }

                if (kh.SDT != null)
                {
                    khachhang.SDT = kh.SDT;
                }
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details");
            }
            return View(kh);
        }

        public ActionResult Logout_64130005()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword_64130005(string idUser)
        {
            KhachHang khsession = (KhachHang)Session["Taikhoan"];
            KhachHang kh = db.KhachHangs.Find(idUser);
            kh.ConfirmPassword = null;
            kh.MatKhau = null;
            if (khsession.MaKH != kh.MaKH)
            {
                return HttpNotFound();

            }
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword_64130005([Bind(Include = "MaKH,MatKhau,ConfirmPassword,MatKhauCu")] KhachHang kh, HttpPostedFileBase file)
        {
            KhachHang khachhang = db.KhachHangs.Find(kh.MaKH);
            ModelState.Remove("TenKH");
            ModelState.Remove("HoKH");
            ModelState.Remove("Emai");
            if (ModelState.IsValid)
            {
                if (!khachhang.MatKhau.Equals(kh.MatKhauCu))
                {
                    ModelState.AddModelError(nameof(KhachHang.MatKhauCu), "Mật khẩu cũ không đúng");
                    return View(kh);
                }
                khachhang.MatKhau = kh.MatKhau;
                khachhang.ConfirmPassword = kh.ConfirmPassword;
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(kh);
        }
        public ActionResult OrderList_64130005(string sr)
        {

            var orderList = (from s in db.DonHangs select s).ToList();
            var orderDetail = (from s in db.ChiTietDHs select s).ToList();
            ViewBag.orderDetail = orderDetail;
            //ViewBag.orderList = orderList;
            if (String.IsNullOrEmpty(sr))
            {
                ViewBag.orderList = orderList;
            }
            else
            {
                switch (sr)
                {
                    case "Wait":
                        ViewBag.orderList = orderList.Where(s => s.MaTrangThai == "Chưa giao hàng");
                        break;
                    case "Deli":
                        ViewBag.orderList = orderList.Where(s => s.MaTrangThai == "Đang giao hàng");
                        break;
                    case "Done":
                        ViewBag.orderList = orderList.Where(s => s.MaTrangThai == "Hoàn thành");
                        break;
                    case "Cancel":
                        ViewBag.orderList = orderList.Where(s => s.MaTrangThai == "Đã hủy");
                        break;
                    default:
                        ViewBag.orderList = orderList.Where(s => s.MaDH.ToString().Contains(sr));
                        break;
                }
            }

            //ViewBag.TongSoLuong = TongSoLuong();
            KhachHang kh = new KhachHang();
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Login_64130005", "KhachHang_64130005");
            }
            else
            {
                kh = (KhachHang)Session["Taikhoan"];
            }
            return View(kh);
        }

        public ActionResult CancelOrder_64130005(string MaDH)
        {
            var orderList = (from s in db.DonHangs select s).ToList();
            DonHang order = db.DonHangs.Find(MaDH);
            if (order.MaTrangThai == "Chưa giao hàng")
            {
                order.MaTrangThai = "Đã hủy";
            }
            else
            {
                order.MaTrangThai = order.MaTrangThai;
            }
            return View(order);

        }
        [HttpPost, ActionName("CancelOrder_64130005")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrderConfirmed(string maDH)
        {
            try
            {
                DonHang order = db.DonHangs.Find(maDH);
                order.MaTrangThai = "Đã hủy";
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("OrderList_64130005");
        }

        public ActionResult DonHang_64130005(string MaDH)
        {
            DonHang order = db.DonHangs.Find(MaDH);
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (order.MaKH == kh.MaKH)
            {
                return View(order);
            }
            else
            {
                return HttpNotFound();
            }
        }

        private string LayMaKH()
        {
            int soLuong = db.KhachHangs.Count();

            // Tạo mã sách mới
            string maSachMoi = $"{"KH"}{(soLuong + 1).ToString("D3")}";
            return maSachMoi;
        }


    }
}
