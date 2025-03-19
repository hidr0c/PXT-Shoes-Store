using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinaShoseShop.Models;
using PagedList;
using System.Data.Entity;
using System.Net;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Web.Helpers;

namespace VinaShoseShop.Areas.Admin.Controllers
{
    public class HomeController : Controller

    {
        private ApplicationDbcontext db = new ApplicationDbcontext();

      //  GET: Admin/
        public ActionResult Index(string searchQuery, decimal? minPrice, decimal? maxPrice, int? page)
        {
            if (page == null) page = 1;

            var sanphams = db.Sanphams.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                sanphams = sanphams.Where(sp => sp.Tensp.Contains(searchQuery));
            }

            if (minPrice.HasValue)
            {
                sanphams = sanphams.Where(sp => sp.Giatien >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                sanphams = sanphams.Where(sp => sp.Giatien <= maxPrice);
            }

            sanphams = sanphams.OrderBy(sp => sp.Masp);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var model = sanphams.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var dt = db.Sanphams.Find(id);
            return View(dt);
        }

      //  Tạo sản phẩm mới phương thức GET: Admin/Home/Create
       // GET: Admin/Home/Create
        public ActionResult Create()
        { Sanpham sanpham = new Sanpham();
            ViewBag.Mahang = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang");
            ViewBag.Makieudang = new SelectList(db.Kieudangs, "Makieudang", "Tenkieudang");
            return View(sanpham);
        }

       // POST: Admin/Home/Create
       [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Tensp,GiaGoc,Giatien,GiaSale,Sale,Soluong,Mota,Kichco,Sanphammoi,Mausac,Anhbia,Mahang,Makieudang")] Sanpham sanpham)
        {

            if (ModelState.IsValid)
            {
                db.Sanphams.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Mahang = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang", sanpham.Mahang);
            ViewBag.Makieudang = new SelectList(db.Kieudangs, "Makieudang", "Tenkieudang", sanpham.Makieudang);
            return View(sanpham);
        }

       // Sửa sản phẩm GET lấy ra ID sản phẩm: Admin/Home/Edit/5
        // GET: /Admin/Sanpham/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sanpham sanpham = db.Sanphams.Find(id);
            if (sanpham == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mahang = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang", sanpham.Mahang);
            ViewBag.Makieudang = new SelectList(db.Kieudangs, "Makieudang", "Tenkieudang", sanpham.Makieudang);
            return View(sanpham);
        }

        // POST: /Admin/Sanpham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Masp,Tensp,GiaGoc,Giatien,GiaSale,Sale,Soluong,Mota,Kichco,Sanphammoi,Mausac,Mahang,Makieudang")] Sanpham sanpham, HttpPostedFileBase Anhbia)
        {
            if (ModelState.IsValid)
            {
                if (Anhbia != null && Anhbia.ContentLength > 0)
                {
                   // Thực hiện xử lý tải và lưu ảnh mới
                   var fileName = System.IO.Path.GetFileName(Anhbia.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Images/"), fileName);
                    Anhbia.SaveAs(path);
                    sanpham.Anhbia = "/Images/" + fileName;
                }
                else
                {
                  //  Nếu người dùng không chọn ảnh mới
                   // Gán lại ảnh cũ cho sanpham.Anhbia
                   var currentImage = db.Sanphams.AsNoTracking().FirstOrDefault(x => x.Masp == sanpham.Masp)?.Anhbia;
                    sanpham.Anhbia = currentImage;
                }

                db.Entry(sanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Mahang = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang", sanpham.Mahang);
            ViewBag.Makieudang = new SelectList(db.Kieudangs, "Makieudang", "Tenkieudang", sanpham.Makieudang);
            return View(sanpham);
        }

        //Xoá sản phẩm phương thức GET: Admin/Home/Delete/5
        public ActionResult Delete(int id)
        {
            var dt = db.Sanphams.Find(id);
            return View(dt);
        }

        //Xoá sản phẩm phương thức POST: Admin/Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
              // Lấy được thông tin sản phẩm theo ID(mã sản phẩm)
                var dt = db.Sanphams.Find(id);
                if (dt == null)
                {
                    return HttpNotFound();
                }

               // Xoá các chi tiết đơn hàng liên quan
                var chitietdonhangs = db.Chitietdonhangs.Where(c => c.Masp == id);
                foreach (var chitiet in chitietdonhangs)
                {
                    db.Chitietdonhangs.Remove(chitiet);
                }
              //  Xoá
                db.Sanphams.Remove(dt);
              //  Lưu lại
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //Phương thức này dùng để giải phóng tài nguyên (như kết nối cơ sở dữ liệu)
        //khi đối tượng không còn sử dụng, đảm bảo tránh rò rỉ bộ nhớ.
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
