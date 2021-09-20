using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChoCastle.Models;

namespace ChoCastle.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ChoCastle.Models.ChoCastleDBEntities1 db = new Models.ChoCastleDBEntities1();

        public ActionResult Index()
        {
            return View(db.Products.OrderByDescending(model => model.AddedDate).Take(5).ToList());
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

        public ActionResult ProductDescription(int ProductID)
        {
            return View(db.Products.Find(ProductID));
        }

        public ActionResult ProductCategory(int? CategoryID)
        {
            CategoryID = CategoryID is null ? 1 : CategoryID;
            return View(db.Products.Where(model => model.isDisplay.Value == true && model.CategoryID == CategoryID).ToList());
        }

        public ActionResult ShoppingFAQ()
        {
            return View();
        }
    }
}
