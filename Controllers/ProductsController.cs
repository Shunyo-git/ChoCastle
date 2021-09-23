using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChoCastle.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;


namespace ChoCastle.Controllers
{
    public class ProductsController : Controller
    {
        private ChoCastleDBEntities2 db = new ChoCastleDBEntities2();      
       
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.ProductCategory).Include(p => p.Vendor);
            return View(products.ToList());
        }
        public ActionResult ProductManage()
        {
            var products = db.Products.Include(p => p.ProductCategory).Include(p => p.Vendor);
            return View(products.ToList());
        }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        // 2021/9/16 by sean
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

        //Get
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.ProductCategories.OrderBy(x => x.SortID), "CategoryID", "CategoryName");
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName");

            var model = new Product();

            var user =  UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                model.AddedUserID = user.MemberID;
                model.AddedDate = DateTime.Now;
                model.ModifiedUserID = user.MemberID;
                model.ModifiedDate = DateTime.Now;
            }

            return View(model);
        }

        // POST: Products/Create
        // 2021/9/21 By Rita
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductSpec,ProductDisc,isDisplay,PurchasePrice,RetailPrice,SellingPrice,SalePrice,StockQty,AvailableQty,VendorID,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Product product)
        {
            var userId = User.Identity.GetUserId();
             

            if (ModelState.IsValid)
            {
                var user =  UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    product.AddedUserID = user.MemberID;
                    product.AddedDate = DateTime.Now;
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("ProductManage");
            }

            ViewBag.CategoryID = new SelectList(db.ProductCategories.OrderBy(model => model.SortID), "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.ProductCategories.OrderBy(model => model.SortID), "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);
            return View(product);
        }

        // POST: Products/Edit/5
        // 2021/9/21 By Rita
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductSpec,ProductDisc,isDisplay,PurchasePrice,RetailPrice,SellingPrice,SalePrice,StockQty,AvailableQty,VendorID,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Product product)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user =  UserManager.FindById(User.Identity.GetUserId());
                
                if (user != null)
                {
                    product.AddedUserID = user.MemberID;
                    product.AddedDate = DateTime.Now;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProductManage");
            }
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("ProductManage");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 產品分類-列表
        /// 2021/9/21 By Rita
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductCategory()
        {
            return View(db.ProductCategories.ToList());
        }

        /// <summary>
        /// 產品分類-新增
        /// 2021/9/21 By Rita
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryCreate()
        {
            return View();
        }

        /// <summary>
        /// 產品分類-新增
        /// 2021/9/21 By Rita
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryCreate([Bind(Include = "CategoryID,CategoryName,SortID,isDisplay,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] ProductCategory productCategory)
        {
            var userId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    productCategory.AddedUserID = user.Id;
                    productCategory.AddedDate = DateTime.Now;
                }

                db.ProductCategories.Add(productCategory);
                db.SaveChanges();
                return RedirectToAction("ProductCategory");
            }
            return View(productCategory);
        }

        /// <summary>
        /// 產品分類-編輯
        /// 2021/9/21 By Rita
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory product = db.ProductCategories.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }

        /// <summary>
        /// 產品分類-編輯
        /// 2021/9/21 By Rita
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit([Bind(Include = "CategoryID, CategoryName, SortID, isDisplay, AddedDate, AddedUserID, ModifiedDate, ModifiedUserID")] ProductCategory productCategory)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    productCategory.ModifiedUserID = user.Id;
                    productCategory.ModifiedDate = DateTime.Now;
                }

                db.Entry(productCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("productCategory");
            }
            
            return View(productCategory);
        }

        /// <summary>
        /// 產品分類-刪除
        /// 2021/9/21 By Rita
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryDelete(int id)
        {
            ProductCategory productCategory = db.ProductCategories.Find(id);
            db.ProductCategories.Remove(productCategory);
            db.SaveChanges();

            return RedirectToAction("ProductCategory");
        }
    }
}
