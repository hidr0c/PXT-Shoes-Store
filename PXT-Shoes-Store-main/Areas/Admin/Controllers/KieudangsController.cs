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
    public class KieudangsController : Controller
    {
        private ApplicationDbcontext db = new ApplicationDbcontext();

        // GET: Admin/Kieudangs
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;

            var kd = db.Kieudangs.OrderBy(x => x.Makieudang);

            int pageSize = 10;

            int pageNumber = (page ?? 1);

            return View(kd.ToPagedList(pageNumber, pageSize));

        }

        // GET: Admin/Kieudangs/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kieudang kieudang = db.Kieudangs.Find(id);
            if (kieudang == null)
            {
                return HttpNotFound();
            }
            return View(kieudang);
        }

        // GET: Admin/Kieudangs/Create
        public ActionResult Create()
        {
            Kieudang kieudang = new Kieudang();
            return View(kieudang);
        }

        // POST: Admin/Kieudangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Makieudang,Tenkieudang")] Kieudang kieudang)
        {
            if (ModelState.IsValid)
            {
                db.Kieudangs.Add(kieudang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kieudang);
        }

        // GET: Admin/Kieudangs/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kieudang kieudang = db.Kieudangs.Find(id);
            if (kieudang == null)
            {
                return HttpNotFound();
            }
            return View(kieudang);
        }

        // POST: Admin/Kieudangs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Makieudang,Tenkieudang")] Kieudang kieudang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kieudang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kieudang);
        }

        // GET: Admin/Kieudangs/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kieudang kieudang = db.Kieudangs.Find(id);
            if (kieudang == null)
            {
                return HttpNotFound();
            }
            return View(kieudang);
        }

        // POST: Admin/Kieudangs/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kieudang kieudang = db.Kieudangs.Find(id);
            if (kieudang == null)
            {
                return HttpNotFound();
            }

            // Xoá các sản phẩm liên quan
            var sanphams = db.Sanphams.Where(c => c.Makieudang == id);
            foreach (var sp in sanphams)
            {
                db.Sanphams.Remove(sp);
            }
            db.Kieudangs.Remove(kieudang);
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
