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
using System.Web.Services.Description;
using System.Web.UI;
using PagedList;
using System.IO;
using Ganss.Xss;
using System.Diagnostics;

namespace Project_64130005.Areas.Admin_64130005.Controllers
{
    public class Sach_64130005Controller : Controller
    {
        private Project_64130005Entities1 db = new Project_64130005Entities1();

        // GET: Admin_64130005/Sach_64130005
        public async Task<ActionResult> Index(string sort, int? page, string searchString)
        {
            const int pageSize = 5;
            int pageNumber = page ?? 1;

            var sachs = db.Saches.AsQueryable(); // Truy vấn ban đầu

            // Tìm kiếm sản phẩm
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                sachs = sachs.Where(p => p.TenSach.ToLower().Contains(searchString) ||
                                         p.TheLoai.TenLoai.ToLower().Contains(searchString));
            }

            // Sắp xếp sản phẩm
            switch (sort)
            {
                case "pre-sold":
                    sachs = sachs.Where(p => p.SoLuong < 50 && p.SoLuong > 0);
                    break;
                case "sold":
                    sachs = sachs.Where(p => p.SoLuong == 0);
                    break;
                case "now":
                    sachs = sachs.OrderByDescending(p => p.NgayChinhSua);
                    break;
                default:
                    sachs = sachs.OrderBy(p => p.MaSach); // Sắp xếp mặc định
                    break;
            }


            return View(sachs.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin_64130005/Sach_64130005/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = await db.Saches.FindAsync(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return View(sach);
        }

        // GET: Admin_64130005/Sach_64130005/Create
        public ActionResult Create()
        {
            ViewBag.MaNgonNgu = new SelectList(db.NgonNgus, "MaNgonNgu", "TenNgonNgu");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");
            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG");
            ViewBag.MaTheLoai = new SelectList(db.TheLoais, "MaLoai", "TenLoai");
            return View();
        }

        // POST: Admin_64130005/Sach_64130005/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(HttpPostedFileBase file, [Bind(Include = "MaSach,MaTheLoai,AnhSach,TenSach,GiamGia,DonGia,SoLuong,DaBan,NgayThem,MaNgonNgu,MaNXB,MaTG,NgayChinhSua,MoTa")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                String anh = "holder.jpg";
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(Server.MapPath("~/Images/SanPham"), pic);
                    file.SaveAs(path);
                    anh = pic;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }
                var sanitizer = new HtmlSanitizer();
                sach.MoTa = sanitizer.Sanitize(sach.MoTa);
                sach.MaSach = LayMaSach(sach.MaTheLoai);
                sach.NgayThem = DateTime.Now;
                sach.NgayChinhSua = DateTime.Now;
                sach.AnhSach = anh;
                sach.DaBan = 0;
                db.Saches.Add(sach);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           

            ViewBag.MaNgonNgu = new SelectList(db.NgonNgus, "MaNgonNgu", "TenNgonNgu", sach.MaNgonNgu);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", sach.MaTG);
            ViewBag.MaTheLoai = new SelectList(db.TheLoais, "MaLoai", "TenLoai", sach.MaTheLoai);
            return View(sach);
        }

        // GET: Admin_64130005/Sach_64130005/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = await db.Saches.FindAsync(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNgonNgu = new SelectList(db.NgonNgus, "MaNgonNgu", "TenNgonNgu", sach.MaNgonNgu);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", sach.MaTG);
            ViewBag.MaTheLoai = new SelectList(db.TheLoais, "MaLoai", "TenLoai", sach.MaTheLoai);
            return View(sach);
        }

        // POST: Admin_64130005/Sach_64130005/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(HttpPostedFileBase file, [Bind(Include = "MaSach,MaTheLoai,AnhSach,TenSach,GiamGia,DonGia,SoLuong,DaBan,NgayThem,MaNgonNgu,MaNXB,MaTG,NgayChinhSua,MoTa")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                var sachFromDb = db.Saches.FirstOrDefault(x => x.MaSach == sach.MaSach);
                if (sachFromDb == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy sách để chỉnh sửa.");
                    return View(sach);
                }

                // Nếu không upload file, giữ lại đường dẫn ảnh cũ
                if (file == null)
                {
                    Debug.WriteLine("File upload là null, giữ nguyên ảnh cũ: " + sachFromDb.AnhSach);
                    sach.AnhSach = sachFromDb.AnhSach;
                }
                else
                {
                    Debug.WriteLine("File upload không null, tên file: " + file.FileName);
                    // Xử lý file mới
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/SanPham"), pic);
                    file.SaveAs(path);
                    sach.AnhSach = pic;
                }

                db.Entry(sachFromDb).State = EntityState.Detached;

                /*sach.AnhSach = anh;*/
                sach.NgayChinhSua = DateTime.Now;
                db.Entry(sach).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaNgonNgu = new SelectList(db.NgonNgus, "MaNgonNgu", "TenNgonNgu", sach.MaNgonNgu);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", sach.MaTG);
            ViewBag.MaTheLoai = new SelectList(db.TheLoais, "MaLoai", "TenLoai", sach.MaTheLoai);
            return View(sach);
        }

        // GET: Admin_64130005/Sach_64130005/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = await db.Saches.FindAsync(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return View(sach);
        }

        // POST: Admin_64130005/Sach_64130005/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Sach sach = await db.Saches.FindAsync(id);
            db.Saches.Remove(sach);
            await db.SaveChangesAsync();
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

        private string LayMaSach(string maLoai)
        {
            var maCuoi = db.Saches
                           .Where(s => s.MaTheLoai == maLoai && s.MaSach.StartsWith(maLoai))
                           .OrderByDescending(s => s.MaSach)
                           .Select(s => s.MaSach)
                           .FirstOrDefault();

            int soThuTu = 0;
            if (maCuoi != null)
            {
                string phanSo = maCuoi.Substring(maLoai.Length);
                soThuTu = int.Parse(phanSo);
            }
            string maSachMoi = $"{maLoai}{(soThuTu + 1).ToString("D3")}";
            return maSachMoi;
        }


    }
}
