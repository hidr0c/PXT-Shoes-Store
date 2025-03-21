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
    public class NguoidungsController : Controller
    {
        private ApplicationDbcontext db = new ApplicationDbcontext();
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageNumber = (page ?? 1);
            int pageSize = 10;

            var nd = db.Nguoidungs
                .OrderBy(x => x.Manguoidung).ToList();
            var pageUser = nd.ToPagedList(pageNumber, pageSize);

            var phanQuyenList = new List<Phanquyen>();
            foreach (var user in pageUser)
            {
                var phanQuyen = db.Phanquyens.FirstOrDefault(
                    p => p.IDQuyen == user.IDQuyen);
                if (phanQuyen != null)
                {
                    phanQuyenList.Add(phanQuyen);
                }
            }
            ViewBag.PhanQuyenList = phanQuyenList;

            return View(pageUser);

        }

        //Xem chi tiết người dùng theo Mã người dùng
        // GET: Admin/Nguoidungs/Details
        public ActionResult Details(int? id)
        {
            // Nếu không có người dùng có mã được truyền vào thì trả về trang báo lỗi
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Khai báo một người dùng theo mã
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            ViewBag.Quyen = db.Phanquyens.FirstOrDefault(
                p => p.IDQuyen == nguoidung.IDQuyen);
            // trả về trang chi tiết người dùng
            return View(nguoidung);
        }

        // GET: Admin/Nguoidungs/Create
        public ActionResult Create()
        {
            Nguoidung nguoidung = new Nguoidung();
            ViewBag.IDQuyen = new SelectList(db.Phanquyens, "IDQuyen", "TenQuyen");
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Manguoidung,Hoten,Email,Dienthoai,Matkhau,IDQuyen, Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Nguoidungs.Add(nguoidung);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDQuyen = new SelectList(db.Phanquyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }


        // Chỉnh sửa người dùng
        // GET: Admin/Nguoidungs/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDQuyen = new SelectList(db.Phanquyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen,Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDQuyen = new SelectList(db.Phanquyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        // Xoá người dùng 
        // GET: Admin/Nguoidungs/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            ViewBag.Quyen = db.Phanquyens.FirstOrDefault(
                p => p.IDQuyen == nguoidung.IDQuyen);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }

            // Xoá các đơn hàng liên quan
            var donhangs = db.Donhangs.Where(c => c.Manguoidung == id);
            foreach (var dh in donhangs)
            {
                db.Donhangs.Remove(dh);
            }
            db.Nguoidungs.Remove(nguoidung);
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
