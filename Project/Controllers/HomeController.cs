using PagedList;
using Project_64130005.Models;
using Project_64130005.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Project_64130005.Controllers
{
    public class HomeController : Controller
    {
        private readonly Project_64130005Entities1 db = new Project_64130005Entities1();
        public ActionResult Index(string searchStr, string sort, int pageIndex = 1)
        {
            //Catalog
            ViewBag.theloais = (from s in db.TheLoais select s).ToList();
            ViewBag.nhaxuatbans = (from n in db.NhaXuatBans select n).ToList();
            if (String.IsNullOrEmpty(searchStr))
            {
                Sort(sort, pageIndex);
            }
            // Tìm kiếm sản phẩm 
            else
            {
                string searchLower = searchStr.ToLower();
                Debug.WriteLine(searchLower);
                var query = db.Set<Sach>().Where(s => s.TenSach.ToLower().Contains(searchLower));
                /*if (searchLower != null)
                {
                    query.Where(s => s.TenSach.ToLower().Contains(searchLower));
                }*/
                var dsProduct2 = query;
                var totalPage = (double)dsProduct2.Count() / (double)8;
                ViewBag.totalPage = Math.Ceiling(totalPage);
                ViewBag.searchStr = searchStr;
                ViewBag.sachs = dsProduct2.OrderBy(s => s.TenSach).ToPagedList(pageIndex, 5);
                ViewBag.currentPage = pageIndex;
            }
            return View();
        }

        public void Sort(string sort, int pageIndex)
        {
            //Product
            var dsProduct = (from s in db.Set<Sach>() select s).ToList();
            var totalPage = (double)dsProduct.Count() / (double)8;
            ViewBag.totalPage = Math.Ceiling(totalPage);
            ViewBag.currentPage = pageIndex;
            // sắp xếp theo filter
            if (String.IsNullOrEmpty(sort))
            {
                ViewBag.sachs = dsProduct.OrderByDescending(c => c.DaBan).ToPagedList(pageIndex, 8);
            }
            else
            {
                sort = sort.ToLower();
                ViewBag.currentSort = sort;
                switch (sort)
                {
                    case "asc":
                        ViewBag.sachs = ProductSort.Ascending(dsProduct).ToPagedList(pageIndex, 8);
                        break;
                    case "desc":
                        ViewBag.sachs = ProductSort.Descending(dsProduct).ToPagedList(pageIndex, 8);
                        break;
                    case "new":
                        ViewBag.sachs = dsProduct.OrderByDescending(c => c.MaSach).ToPagedList(pageIndex, 8);
                        break;
                    case "hot":
                        ViewBag.sachs = dsProduct.OrderByDescending(c => c.DaBan).ToPagedList(pageIndex, 8);
                        break;
                    case "selling":
                        ViewBag.sachs = dsProduct.Where(c => c.SoLuong > 0).ToPagedList(pageIndex, 8);
                        break;
                    default:
                        var dsProduct2 = dsProduct.Where(s => s.TheLoai.TenLoai.ToLower().Contains(sort) || s.NhaXuatBan.TenNXB.ToLower().Contains(sort));
                        totalPage = (double)dsProduct2.Count() / (double)8;
                        ViewBag.totalPage = Math.Ceiling(totalPage);
                        ViewBag.sachs = dsProduct2.ToPagedList(pageIndex, 8);
                        break;
                }
            }
        }
        public ActionResult Sach_64130005(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound("Invalid ID");
            }

            Sach sach = db.Saches.FirstOrDefault(s => s.MaSach.ToString() == id);
            if (sach == null)
            {
                return HttpNotFound("Book not found");
            }

            return View(sach);
        }

    }
}