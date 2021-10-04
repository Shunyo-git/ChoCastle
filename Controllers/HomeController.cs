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
        ChoCastle.Models.ChoCastleDBEntities db = new Models.ChoCastleDBEntities();

        public ActionResult Index()
        {
            var result = (from pd in db.Products
                          join img in db.ProductImages on pd.ProductID equals img.ProductID
                          where img.isMain == 1
                          select new imgResult
                          {
                              Product = pd,
                              Image = img
                          }).OrderByDescending(x => x.Product.AddedDate.Value).Take(5).ToList();

            return View(result);
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
        /// <summary>
        /// GET: /Home/ProductDescription?ProductID=3
        /// </summary>
        /// <param name="ProductID">ProductID parameter</param>
        /// <returns>Return download file</returns>
        public ActionResult ProductDescription(int? ProductID)
        {
            imgResultDes imgResult = new imgResultDes();
            Product product = db.Products.Find(ProductID);
            List<ProductImage> lst_images = db.ProductImages.Where(x => x.ProductID == ProductID.Value).ToList();
            if (product == null)
            {
                return HttpNotFound();
            }

            imgResult.Product = product;
            imgResult.lst_Image = lst_images;

            ViewBag.CategoryID = new SelectList(db.ProductCategories.OrderBy(model => model.SortID), "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);
            return View(imgResult);
            //return View(product);

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
                    shoppingCart.isLogin = User.Identity.IsAuthenticated ? 1 : 0;
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
                    int OrderQty = Int32.Parse(Request.Form["OrderQty"]);
                    da.UpdateShoppingDetail(CartID, product.ProductID, shoppingDetail.ProductName, product.SellingPrice, (int)shoppingDetail.OrderQuantity + OrderQty, 0, DateTime.Now);

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
            var result = from product in db.Products
                         join image in db.ProductImages on product.ProductID equals image.ProductID
                         where product.CategoryID == CategoryID && product.isDisplay.Value == true && image.isMain == 1
                         select new imgResult
                         {
                             Product = product,
                             Image = image
                         };
            String imgurl = "/Template/images/";
            switch (CategoryID)
            {
                case 1:
                    imgurl += "choco/生巧克力7.jpg";
                    break;
                case 2:
                    imgurl += "cookies.jpg";
                    break;
                case 3:
                    imgurl += "cakes.jpg";
                    break;
                case 4:
                    imgurl += "gift.jpg";
                    break;
                default:
                    break;

            }
            ViewBag.img = imgurl;
            return View(result.ToList());
        }

        public ActionResult ShoppingFAQ()
        {
            return View();
        }

        public class imgResult
        {
            public Product Product { get; set; }
            public ProductImage Image { get; set; }
        }

        public class imgResultDes
        {
            public Product Product { get; set; }
            public List<ProductImage> lst_Image { get; set; }
        }
    }
}
