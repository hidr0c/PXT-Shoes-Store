using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Areas.Admin.Controllers
{
    public class ThongKeDoanhThuController : Controller
	{
		private ApplicationDbcontext db = new ApplicationDbcontext();

		// GET: Admin/ThongKeDoanhThu
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult GetDuLieu(string fromDate, string toDate)
		{
			try
			{
				var query = from o in db.Donhangs
							join od in db.Chitietdonhangs
							on o.Madon equals od.Madon
							join sp in db.Sanphams
							on od.Masp equals sp.Masp	
							select new
							{
								CreatedDate = o.Ngaydat,
								Quantity = od.Soluong,
								Price = od.Dongia,
								OriginalPrice = od.GiaGocSp
							};
				if (!string.IsNullOrEmpty(fromDate))
				{
					DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
					query = query.Where(x => x.CreatedDate >= startDate);
				}
				if (!string.IsNullOrEmpty(toDate))
				{
					DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
					query = query.Where(x => x.CreatedDate < endDate);
				}
				var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
				{
					Date = x.Key.Value,
					TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice), 
					TotalSell = x.Sum(y => y.Quantity * y.Price),
				}).Select(x => new
				{
					Date = x.Date,
					DoanhThu = x.TotalSell,   // doanh thu sẽ bằng tổng cái số lượng nhân giá 
					LoiNhuan = x.TotalSell - x.TotalBuy // lợi nhuận bằng tổng số lượng * giá - tổng số lượng * giá gốc (giá này tùy thuộc vào sale hay không sale)
                });
				return Json(new { Data = result, ketqua = true }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { ketqua = false, error = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}
	}
}