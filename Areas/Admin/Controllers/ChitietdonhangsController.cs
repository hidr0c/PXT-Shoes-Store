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
    public class ChitietdonhangsController : Controller
    {
        private ApplicationDbcontext db = new ApplicationDbcontext();
		//Database

		public ActionResult Index(int? page, int? Madon)
		{
			if (page == null) page = 1;

			IQueryable<Chitietdonhang> ctDonhangsQuery = db.Chitietdonhangs.OrderBy(x => x.Madon);

			// Kiểm tra nếu Madon được truyền từ DonhangsController
			if (Madon != null)
			{
				ctDonhangsQuery = ctDonhangsQuery.Where(x => x.Madon == Madon);
			}

			// Materialize the query result to a list
			var ctDonhangs = ctDonhangsQuery.ToList();

			var products = new List<Sanpham>();
			foreach (var sp in ctDonhangs)
			{
				var sanPham = db.Sanphams.FirstOrDefault(p => p.Masp == sp.Masp);
				if (sanPham != null)
				{
					products.Add(sanPham);
				}
			}

			ViewBag.SanPham = products;
			int pageSize = 10;
			int pageNumber = (page ?? 1);

			return View(ctDonhangs.ToPagedList(pageNumber, pageSize));
		}

	}
}
