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
using System.Data.SqlClient;

namespace ChoCastle.Controllers
{



    [RequireHttps]
    public class HomeController : Controller
    {
        ChoCastle.Models.ChoCastleDBEntities1 db = new Models.ChoCastleDBEntities1();

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
        //2021/9/24 DataAccessFactory.CreateDefaultDataAccess()
        //SQL 資料庫存取提供者
        private SQLDataAccessProvider _da;
        public SQLDataAccessProvider da
        {
            get
            {
                return _da ?? DataAccessFactory.CreateDefaultDataAccess();
            }
            private set
            {
                _da = value;
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
                int CartID = 0;
                int MemberID = 0;
                ShoppingCar shoppingCart;

                //是否有購物車
                if (Session["CartID"] != null)
                {
                    CartID = Int32.Parse(Session["CartID"].ToString());
                }
 
                if (CartID == 0)
                {
                    //無購物車
                    //判斷是否為會員
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    if (user != null)
                    {
                        MemberID = user.MemberID;
                        //如為會員 取得購物車紀錄
                        CartID = da.GetCartID(MemberID);
                        //如有舊購物車取回購物車
                        if (CartID > 0)
                        {
                            Session["CartID"] = CartID;
                        }
                    }

                }

                //新增購物車
                if (CartID == 0)
                {
                    //新增購物車
                    //CartID = db.ShoppingCars.Count() + 1; 
                    CartID = db.Database.SqlQuery<int>("SELECT CASE WHEN MAX(CarID) IS NULL THEN 1 ELSE MAX(CarID) + 1 END AS CartID FROM ShoppingCar").ToList()[0];
                    shoppingCart = new ShoppingCar();
                    shoppingCart.CarID = CartID;
                    shoppingCart.MemberID = MemberID;
                    shoppingCart.isLogin = User.Identity.IsAuthenticated ? 1:0 ;
                    shoppingCart.AddedDate = DateTime.Now;
                    shoppingCart.ModifiedDate = DateTime.Now;
                    db.ShoppingCars.Add(shoppingCart);
                    db.SaveChanges();
                    Session["CartID"] = CartID;
                }


                //取得商品清單
                ShoppingDetail shoppingDetail = da.GetCartShoppingDetailByProductID(product.ProductID, CartID);

                if (shoppingDetail == null)
                {
                    //新商品
                    ShoppingDetail newItem = new ShoppingDetail();
                    newItem.CarID = CartID;
                    newItem.OrderQuantity = Int32.Parse(Request.Form["OrderQty"]);
                    newItem.ProductID = product.ProductID;
                    newItem.ProductName = product.ProductName;
                    newItem.UnitPrice = product.SellingPrice;
                    newItem.Subtotal = newItem.UnitPrice * newItem.OrderQuantity;
                    newItem.AddedDate = DateTime.Now;
                    //shoppingDetail.ModifiedDate = DateTime.Now;
                    db.ShoppingDetails.Add(newItem);
                    db.SaveChanges();
                }
                else
                {
                    //已有商品更新數量
                    da.UpdateShoppingDetail(CartID, product.ProductID, shoppingDetail.ProductName, product.SellingPrice, (int)shoppingDetail.OrderQuantity + 1,0,DateTime.Now);
                    
                    //shoppingDetail.OrderQuantity += 1;
                    //shoppingDetail.Subtotal = shoppingDetail.UnitPrice * shoppingDetail.OrderQuantity;
                    //db.Entry(shoppingDetail).State = EntityState.Modified;
                    //db.SaveChanges();
                }


                
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
