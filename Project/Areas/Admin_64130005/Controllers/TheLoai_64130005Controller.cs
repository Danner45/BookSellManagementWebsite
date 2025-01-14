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
    public class TheLoai_64130005Controller : Controller
    {
        private Project_64130005Entities1 db = new Project_64130005Entities1();

        // GET: Admin_64130005/TheLoai_64130005
        public async Task<ActionResult> Index()
        {
            return View(await db.TheLoais.ToListAsync());
        }

        // GET: Admin_64130005/TheLoai_64130005/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TheLoai theLoai = await db.TheLoais.FindAsync(id);
            if (theLoai == null)
            {
                return HttpNotFound();
            }
            return View(theLoai);
        }

        // GET: Admin_64130005/TheLoai_64130005/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin_64130005/TheLoai_64130005/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MaLoai,TenLoai")] TheLoai theLoai)
        {
            if (ModelState.IsValid)
            {
                db.TheLoais.Add(theLoai);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(theLoai);
        }

        // GET: Admin_64130005/TheLoai_64130005/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TheLoai theLoai = await db.TheLoais.FindAsync(id);
            if (theLoai == null)
            {
                return HttpNotFound();
            }
            return View(theLoai);
        }

        // POST: Admin_64130005/TheLoai_64130005/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MaLoai,TenLoai")] TheLoai theLoai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(theLoai).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(theLoai);
        }

        // GET: Admin_64130005/TheLoai_64130005/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TheLoai theLoai = await db.TheLoais.FindAsync(id);
            if (theLoai == null)
            {
                return HttpNotFound();
            }
            return View(theLoai);
        }

        // POST: Admin_64130005/TheLoai_64130005/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            TheLoai theLoai = await db.TheLoais.FindAsync(id);
            db.TheLoais.Remove(theLoai);
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
