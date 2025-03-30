using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Areas.Admin.Controllers
{
    public class DonhangsController : Controller
    {
        private ApplicationDbcontext db = new ApplicationDbcontext();

        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;

            var dh = db.Donhangs.OrderBy(x => x.Madon).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var pageOrder = dh.ToPagedList(pageNumber, pageSize);

            var userList = new List<Nguoidung>();
            foreach (var user in dh)
            {
                var nguoiDung = db.Nguoidungs.FirstOrDefault(
                    p => p.Manguoidung == user.Manguoidung);
                userList.Add(nguoiDung);
            }
            ViewBag.UserList = userList;

            return View(pageOrder);
        }

        // Xoá sản phẩm phương thức GET: Admin/Home/Delete/5
        public ActionResult Cancel(int id)
        {
            // Lấy thông tin của các đơn hàng cần hủy
            var dh = db.Donhangs.Find(id);
            if (dh == null)
            {
                return HttpNotFound();
            }

            // Lấy các chi tiết đơn hàng của đơn hàng cần hủy
            var chiTietDonHangs = db.Chitietdonhangs.Where(ct => ct.Madon == id).ToList();

            // Xóa các chi tiết đơn hàng
            foreach (var ct in chiTietDonHangs)
            {
                db.Chitietdonhangs.Remove(ct);
            }

            // Xóa đơn hàng
            db.Donhangs.Remove(dh);
            db.SaveChanges();

            // Chuyển hướng đến trang Index
            return RedirectToAction("Index");
        }

        // Xoá sản phẩm phương thức POST: Admin/Home/Delete/5
        [HttpPost]
        public ActionResult Cancel(int id, FormCollection collection)
        {
            try
            {
                //Lấy được thông tin sản phẩm theo ID(mã sản phẩm)
                var dh = db.Donhangs.Find(id);
                // Xoá
                db.Donhangs.Remove(dh);
                // Lưu lại
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delivery(int id)
        {
            var dh = db.Donhangs.Find(id);
            if (dh != null && dh.Tinhtrang == 1)
            {
                // Cập nhật trạng thái đơn hàng từ 1 thành 2 (đang giao)
                dh.Tinhtrang = 2;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
       

    }
}
