using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Controllers
{
    public class SanphamController : Controller
    {
		ApplicationDbcontext db = new ApplicationDbcontext();

        public ActionResult Index(string searchQuery1, decimal? minPrice1, decimal? maxPrice1)
        {
            var sanphams1 = db.Sanphams.AsQueryable();

            // Áp dụng bộ lọc dựa trên các tiêu chí tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery1))
            {
                sanphams1 = sanphams1.Where(sp => sp.Tensp.Contains(searchQuery1) || sp.Mota.Contains(searchQuery1));
            }
            if (minPrice1.HasValue)
            {
                sanphams1 = sanphams1.Where(sp => sp.Giatien >= minPrice1);
            }
            if (maxPrice1.HasValue)
            {
                sanphams1 = sanphams1.Where(sp => sp.Giatien <= maxPrice1);
            }

            sanphams1 = sanphams1.OrderBy(sp => sp.Masp);

            var filteredSanphams = sanphams1.ToList(); // Lấy danh sách sản phẩm đã lọc

            return View(filteredSanphams);
        }


        // GET: Sanpham
        public ActionResult Nike()
        {
            var nike = db.Sanphams.Where(n => n.Mahang == 1).Take(20).ToList();
            return PartialView(nike);
        }
        public ActionResult Adidas()
        {
            var adidas = db.Sanphams.Where(n => n.Mahang == 2).Take(20).ToList();
            return PartialView(adidas);
        }
        public ActionResult Puma()
        {
            var puma = db.Sanphams.Where(n => n.Mahang == 3).Take(20).ToList();
            return PartialView(puma);
        }
        //public ActionResult Vans()
        //{
        //    var vans = db.Sanphams.Where(n => n.Mahang == 104).Take(4).ToList();
        //    return PartialView(vans);
        //}
        public ActionResult xemchitiet(int Masp = 0)
        {
            var chitiet = db.Sanphams.SingleOrDefault(n => n.Masp == Masp);
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }

        public ActionResult Sneaker()
        {
            var sneaker = db.Sanphams.Where(n => n.Makieudang == 1).Take(30).ToList();
            return PartialView(sneaker);
        }

        public ActionResult Loafer()
        {
            var loafer = db.Sanphams.Where(n => n.Makieudang == 5).Take(30).ToList();
            return PartialView(loafer);
        }

        public ActionResult Brogues()
        {
            var brogues = db.Sanphams.Where(n => n.Makieudang == 11).Take(30).ToList();
            return PartialView(brogues);
        }
    }

}