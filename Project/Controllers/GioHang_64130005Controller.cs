using Project_64130005.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_64130005.Controllers
{
    public class GioHang_64130005Controller : Controller
    {
        private readonly Project_64130005Entities1 db = new Project_64130005Entities1();
        // GET: GioHang_64130005
        public ActionResult Index()
        {
            List<GioHang_64130005> listGioHang = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listGioHang);
        }
        public List<GioHang_64130005> Laygiohang()
        {
            List<GioHang_64130005> listGiohang = Session["Giohang"] as List<GioHang_64130005>;

            if (listGiohang == null)
            {
                listGiohang = new List<GioHang_64130005>();
                Session["Giohang"] = listGiohang;

            }
            return listGiohang;
        }
        public int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang_64130005> listGiohang = Session["Giohang"] as List<GioHang_64130005>;
            if (listGiohang != null)
            {
                iTongSoLuong = listGiohang.Sum(n => n.SoLuong);
            }
            return iTongSoLuong;
        }
        private int TongTien()
        {
            int iTongTien = 0;
            List<GioHang_64130005> listGiohang = Session["Giohang"] as List<GioHang_64130005>;
            if (listGiohang != null)
            {
                iTongTien = listGiohang.Sum(n => n.ThanhTien);
            }
            return iTongTien;
        }
        public ActionResult Xoagiohang(string MaSach)
        {
            // Lấy giỏ hàng từ session
            List<GioHang_64130005> listGiohang = Laygiohang();
            // Kiểm tra sản phẩm có trong giỏ hàng hay không
            GioHang_64130005 product = listGiohang.SingleOrDefault(n => n.MaSach == MaSach);
            //nếu tồn tại thì sửa số lượng
            if (product != null)
            {
                listGiohang.RemoveAll(n => n.MaSach == MaSach);
                return RedirectToAction("Index");
            }
            if (listGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }
        public ActionResult Capnhatgiohang(string MaSach, FormCollection f)
        {
            // Lấy giỏ hàng từ session
            List<GioHang_64130005> listGiohang = Laygiohang();
            // Kiểm tra sản phẩm có trong giỏ hàng hay không
            GioHang_64130005 product = listGiohang.SingleOrDefault(n => n.MaSach == MaSach);
            if (product != null)
            {
                product.SoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAll()
        {
            List<GioHang_64130005> listGiohang = Laygiohang();
            listGiohang.Clear();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Themgiohang(string MaSach, string strURL)
        {
            // Lấy ra session giỏ hàng
            List<GioHang_64130005> listGiohang = Laygiohang();
            // Kiểm tra sản phẩm này có trong giỏ hàng chưa
            GioHang_64130005 product = listGiohang.Find(n => n.MaSach == MaSach);
            if (product == null)
            {
                product = new GioHang_64130005(MaSach);
                product.DonGia = product.DonGia - (product.DonGia * db.Saches.Where(s => s.MaSach == product.MaSach).Select(s => s.GiamGia).FirstOrDefault()??0);
                listGiohang.Add(product);
                return Redirect(strURL);
            }
            else
            {
                product.SoLuong++;
                return Redirect(strURL);
            }
        }
        [HttpGet]
        public ActionResult DatHang_64130005()
        {
            // Kiểm tra đăng nhập
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("DatHangKhongTK_64130005");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Lấy giỏ hàng từ session 
            List<GioHang_64130005> listGiohang = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listGiohang);
        }
        public ActionResult DatHang_64130005(FormCollection collection)
        {
            var dsProduct = (from s in db.Saches select s).ToList();
            //Thêm đơn hàng
            DonHang order = new DonHang();
            KhachHang khsession = (KhachHang)Session["Taikhoan"];
            KhachHang kh = db.KhachHangs.Find(khsession.MaKH);
            List<GioHang_64130005> gh = Laygiohang();
            order.MaDH = LayMaDH();
            order.MaKH = kh.MaKH;
            order.NgayDat = DateTime.Now;
            order.NgayGiao = order.NgayDat.Value.AddDays(3);
            order.MaTrangThai = "Chưa giao hàng";
            order.MaThanhToan = "TM";
            string DiaChi = collection["DiaChi"];
            string Tinh = collection["TenTinh"];
            string Quan = collection["TenQuan"];
            string Phuong = collection["TenPhuong"];
            order.TenNguoiNhan = collection["TenNguoiNhan"];
            order.DiaChi = String.Concat(DiaChi, ", ", Phuong, " ", Quan, " ", Tinh);
            order.TongTien = TongTien();
            order.TongSoLuong = TongSoLuong();
            order.NVDuyetDon = null;
            order.NVGiaoHang = null;
            db.DonHangs.Add(order);
            db.SaveChanges();
            foreach (var item in gh)
            {
                ChiTietDH ctdh = new ChiTietDH();
                ctdh.MaDH = order.MaDH;
                ctdh.MaSach = item.MaSach;
                ctdh.SoLuong = item.SoLuong;
                ctdh.ThanhTien = item.ThanhTien;
                db.ChiTietDHs.Add(ctdh);
            }
            db.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang_64130005", "GioHang_64130005");
        }
        public ActionResult DatHangKhongTK_64130005()
        {
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Lấy giỏ hàng từ session 
            List<GioHang_64130005> listGiohang = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listGiohang);
        }
        public ActionResult DatHangKhongTK_64130005(FormCollection collection)
        {
            var dsProduct = (from s in db.Saches select s).ToList();
            //Thêm đơn hàng
            var HoTen = collection["HoTen"];
            var Sdt = collection["Sdt"];
            var Email = collection["Email"];

            KhachHang kh = new KhachHang();
            kh.HoKH = HoTen.ToString();
            kh.Emai = Email.ToString();
            String anh = "user.jpg";
            kh.AnhKH = anh;
            if (Sdt.ToString().Length == 10)
            {
                kh.SDT = Sdt.ToString();
            }
            else
            {
                kh.SDT = null;
            }
            db.KhachHangs.Add(kh);
            db.SaveChanges();
            DonHang order = new DonHang();
            List<GioHang_64130005> gh = Laygiohang();
            order.MaDH = LayMaDH();
            order.MaKH = kh.MaKH;
            order.NgayDat = DateTime.Now;
            order.NgayGiao = order.NgayDat.Value.AddDays(3);
            order.MaTrangThai = "Chưa giao hàng";
            order.MaThanhToan = null;
            var DiaChi = collection["DiaChi"];
            order.DiaChi = DiaChi.ToString();
            order.TongTien = TongTien();
            order.TongSoLuong = TongSoLuong();
            db.DonHangs.Add(order);
            db.SaveChanges();
            foreach (var item in gh)
            {
                ChiTietDH ctdh = new ChiTietDH();
                ctdh.MaDH = order.MaDH;
                ctdh.MaSach = item.MaSach;
                ctdh.SoLuong = item.SoLuong;
                ctdh.ThanhTien = item.ThanhTien;
                db.ChiTietDHs.Add(ctdh);
            }
            db.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang_64130005", "Giohang_64130005");
        }
        public ActionResult BuyNow(string MaSach)
        {
            List<GioHang_64130005> listGiohang = Laygiohang();
            // Kiểm tra sản phẩm này có trong giỏ hàng chưa
            GioHang_64130005 product = listGiohang.Find(n => n.MaSach == MaSach);
            if (product == null)
            {
                product = new GioHang_64130005(MaSach);
                listGiohang.Add(product);
            }
            else
            {
                product.SoLuong++;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Xacnhandonhang_64130005()
        {
            return View();
        }
        private string LayMaDH()
        {
            DateTime ngayHienTai = DateTime.Now;
            string ngay = ngayHienTai.ToString("ddMMyy");
            int soThuTu = db.DonHangs.Count(d => d.NgayDat == ngayHienTai.Date)+1;
            string soThuTuDinhDang = soThuTu.ToString("D4");
            return $"DH{ngay}{soThuTuDinhDang}";
        }


    }
}