using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChoCastle.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ChoCastle.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ChoCastle.Models.ChoCastleDBEntities2 db = new Models.ChoCastleDBEntities2();

        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.ProductCategory).Include(p => p.Vendor);
            return View(products.Take(5).ToList());
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

        // 2021/9/21 by sean
        // GET: /Products/Create
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET
        public ActionResult ProductDescription(int? ProductID)
        {

            Product product = db.Products.Find(ProductID);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.ProductCategories.OrderBy(model => model.SortID), "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);
            return View(product);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDescription([Bind(Include = "ProductID,ProductName,SellingPrice")] Product product)
        {

            //20210921 by sean

            if (ModelState.IsValid)
            {
                int CartID;
                ShoppingCar shoppingCart;

                if (Session["CartID"] == null)
                {
                    CartID = db.ShoppingCars.Count() + 1;
                    Session["CartID"] = CartID;

                    shoppingCart = new ShoppingCar();

                    shoppingCart.CarID = CartID;
                    shoppingCart.AddedDate = DateTime.Now;

                    var user = UserManager.FindById(User.Identity.GetUserId());
                    if (user != null)
                    {
                        shoppingCart.MemberID = user.MemberID;
                        shoppingCart.isLogin = 1;
                        shoppingCart.OrderName = user.ChineseName;
                    }
                    else
                    {
                        shoppingCart.MemberID = 0;
                        shoppingCart.isLogin = 0;
                    }
                    shoppingCart.ModifiedDate = DateTime.Now;
                    db.ShoppingCars.Add(shoppingCart);
                }
                else
                {
                    CartID = Int32.Parse(Session["CartID"].ToString());
                    shoppingCart = db.ShoppingCars.Find(CartID);
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    if (user != null)
                    {
                        shoppingCart.MemberID = user.MemberID;
                        shoppingCart.isLogin = 1;
                        shoppingCart.OrderName = user.ChineseName;
                        db.Entry(shoppingCart).State = EntityState.Modified;
                    }
                }
                ShoppingDetail shoppingDetail = db.ShoppingDetails.Find(CartID);

                if (shoppingDetail == null)
                {
                    shoppingDetail = new ShoppingDetail();
                    shoppingDetail.CarID = CartID;
                    shoppingDetail.OrderQuantity = Int32.Parse(Request.Form["OrderQty"]);
                    shoppingDetail.ProductID = product.ProductID;
                    shoppingDetail.ProductName = product.ProductName;
                    shoppingDetail.UnitPrice = product.SellingPrice;
                    shoppingDetail.Subtotal = shoppingDetail.UnitPrice * shoppingDetail.OrderQuantity;
                    shoppingDetail.AddedDate = DateTime.Now;

                    db.ShoppingDetails.Add(shoppingDetail);
                }
                else
                {
                    shoppingDetail.OrderQuantity += 1;
                    shoppingDetail.Subtotal = shoppingDetail.UnitPrice * shoppingDetail.OrderQuantity;

                    db.Entry(shoppingDetail).State = EntityState.Modified;
                    shoppingCart.TotalAmount = 0;
                }


                db.SaveChanges();
            }
            return RedirectToAction("Index", "ShoppingCart");
            //return View(db.Products.Find(product.ProductID));
        }

        public ActionResult ProductCategory(int? CategoryID)
        {
            CategoryID = CategoryID is null ? 1 : CategoryID;
            ProductCategory productCategory = db.ProductCategories.Find(CategoryID);
            ViewBag.CategoryID = productCategory.CategoryName;
            return View(db.Products.Where(model => model.isDisplay.Value == true && model.CategoryID == CategoryID).ToList());
        }

        public ActionResult ShoppingFAQ()
        {
            return View();
        }
    }
}
