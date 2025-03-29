using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbcontext db = new ApplicationDbcontext();
        public ActionResult Index()
        {
            var newestProducts = db.Sanphams
                .OrderByDescending(p => p.Masp)
                .Take(3)
                .ToList();

            // Pass the newest products to the view using ViewBag
            ViewBag.NewestProducts = newestProducts;

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult SlidePartial()
        {
            return PartialView();

        }

    }
}