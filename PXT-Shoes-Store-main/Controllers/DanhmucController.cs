using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VinaShoseShop.Models;

namespace VinaShoseShop.Controllers
{
    public class DanhmucController : Controller
    {
        ApplicationDbcontext db = new ApplicationDbcontext();
        // GET: Danhmuc
        public ActionResult DanhmucPartial()
        {
            var danhmuc = db.Hangsanxuats.ToList();
            return PartialView(danhmuc);
        }
    }
}