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
    public class HangsanxuatsController : Controller
    {
        private ApplicationDbcontext db = new ApplicationDbcontext();

        // GET: Admin/Hangsanxuats
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;

            var hsx = db.Hangsanxuats.OrderBy(x => x.Mahang);

            int pageSize = 10;

            int pageNumber = (page ?? 1);

            return View(hsx.ToPagedList(pageNumber, pageSize));

        }

        // GET: Admin/Hangsanxuats/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hangsanxuat hangsanxuat = db.Hangsanxuats.Find(id);
            if (hangsanxuat == null)
            {
                return HttpNotFound();
            }
            return View(hangsanxuat);
        }

        // GET: Admin/Hangsanxuats/Create
        public ActionResult Create()
        {
            Hangsanxuat hangsanxuat = new Hangsanxuat();
            return View(hangsanxuat);
        }

        // POST: Admin/Hangsanxuats/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Mahang,Tenhang")] Hangsanxuat hangsanxuat)
        {
            if (ModelState.IsValid)
            {
                db.Hangsanxuats.Add(hangsanxuat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hangsanxuat);
        }

        // GET: Admin/Hangsanxuats/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hangsanxuat hangsanxuat = db.Hangsanxuats.Find(id);
            if (hangsanxuat == null)
            {
                return HttpNotFound();
            }
            return View(hangsanxuat);
        }

        // POST: Admin/Hangsanxuats/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Mahang,Tenhang")] Hangsanxuat hangsanxuat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hangsanxuat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hangsanxuat);
        }

        // GET: Admin/Hangsanxuats/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hangsanxuat hangsanxuat = db.Hangsanxuats.Find(id);
            if (hangsanxuat == null)
            {
                return HttpNotFound();
            }
            return View(hangsanxuat);
        }

        // POST: Admin/Hangsanxuats/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hangsanxuat hangsanxuat = db.Hangsanxuats.Find(id);
            if (hangsanxuat == null)
            {
                return HttpNotFound();
            }

            // Xoá các sản phẩm liên quan
            var sanphams = db.Sanphams.Where(c => c.Masp == id);
            foreach (var sp in sanphams)
            {
                db.Sanphams.Remove(sp);
            }
            db.Hangsanxuats.Remove(hangsanxuat);
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
