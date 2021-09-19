using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChoCastle.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ChoCastle.Models.ChoCastleDBEntities db = new Models.ChoCastleDBEntities();

        public ActionResult Index()
        {
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

        public ActionResult ProductDescription()
        {
            db.Products = db.Products;
            return View(db);
        }

        public ActionResult ProductCategory()
        {
            db.Products = db.Products;
            return View(db);
        }

        public ActionResult ShoppingFAQ()
        {
            return View();
        }
    }
}
