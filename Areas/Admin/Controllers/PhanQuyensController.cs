using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Areas.Admin.Controllers
{
    public class PhanQuyensController : Controller
    {
        private ApplicationDbcontext db = new ApplicationDbcontext();

        // GET: Admin/PhanQuyens
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;

            var pq = db.Phanquyens.OrderBy(x => x.IDQuyen);

            int pageSize = 10;

            int pageNumber = (page ?? 1);

            return View(pq.ToPagedList(pageNumber, pageSize));

        }

        // GET: Admin/PhanQuyens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phanquyen phanQuyen = db.Phanquyens.Find(id);
            if (phanQuyen == null)
            {
                return HttpNotFound();
            }
            return View(phanQuyen);
        }

        // GET: Admin/PhanQuyens/Create
        public ActionResult Create()
        {   Phanquyen phanquyen = new Phanquyen();
            return View(phanquyen);
        }

        // POST: Admin/PhanQuyens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDQuyen,TenQuyen")] Phanquyen phanquyen)
        {
            if (ModelState.IsValid)
            {
                db.Phanquyens.Add(phanquyen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phanquyen);
        }

        // GET: Admin/PhanQuyens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phanquyen phanQuyen = db.Phanquyens.Find(id);
            if (phanQuyen == null)
            {
                return HttpNotFound();
            }
            return View(phanQuyen);
        }

        // POST: Admin/PhanQuyens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDQuyen,TenQuyen")] Phanquyen phanQuyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phanQuyen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phanQuyen);
        }

        // GET: Admin/PhanQuyens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phanquyen phanQuyen = db.Phanquyens.Find(id);
            if (phanQuyen == null)
            {
                return HttpNotFound();
            }
            return View(phanQuyen);
        }

        // POST: Admin/PhanQuyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Phanquyen phanQuyen = db.Phanquyens.Find(id);
            if (phanQuyen == null)
            {
                return HttpNotFound();
            }

            // Xoá các người dùng liên quan
            var nguoidungs = db.Nguoidungs.Where(c => c.Manguoidung == id);
            foreach (var nd in nguoidungs)
            {
                db.Nguoidungs.Remove(nd);
            }
            db.Phanquyens.Remove(phanQuyen);
            db.SaveChanges();
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
