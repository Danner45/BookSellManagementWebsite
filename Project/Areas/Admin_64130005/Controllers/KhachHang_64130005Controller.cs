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

namespace Project_64130005.Areas.Admin_64130005.Controllers
{
    public class KhachHang_64130005Controller : Controller
    {
        private Project_64130005Entities1 db = new Project_64130005Entities1();

        // GET: Admin_64130005/KhachHang_64130005
        public async Task<ActionResult> Index(string sort, int? page, string searchStr)
        {
            NhanVien nv = (NhanVien)Session["NV"];
            if (nv.MaLoaiNV != "QL")
            {
                return RedirectToAction("Index", "Order");
            }
            var dsKhachHang = db.KhachHangs.ToList();

            // Tìm kiếm khách hàng trong quản lí khách hàng bằng email 
            if (!String.IsNullOrEmpty(searchStr))
            {
                searchStr = searchStr.ToLower();
                dsKhachHang = dsKhachHang.Where(s => s.Emai.ToLower().Contains(searchStr)).ToList();
                ViewBag.dsKh = dsKhachHang;
            }
            else
            {
                ViewBag.dsKH = dsKhachHang;
            }

            return View();
        }

        // GET: Admin_64130005/KhachHang_64130005/Details/5
        public async Task<ActionResult> Details(string MaKH)
        {
            if (MaKH == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = await db.KhachHangs.FindAsync(MaKH);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

      

        // GET: Admin_64130005/KhachHang_64130005/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = await db.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: Admin_64130005/KhachHang_64130005/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            KhachHang khachHang = await db.KhachHangs.FindAsync(id);
            db.KhachHangs.Remove(khachHang);
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
    }
}
