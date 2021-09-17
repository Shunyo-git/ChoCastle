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
        private ChoCastleDBEntities db = new ChoCastleDBEntities();

        // GET: Products
        public ActionResult Index()
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

        public async Task<ActionResult> Create()
        {
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "CategoryID", "CategoryName");
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName");

            var model = new Product();
            var userId = User.Identity.GetUserId();

            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                model.AddedUserID = user.MemberID;
                model.AddedDate = DateTime.Now ;
                //model.ModifiedUserID = user.MemberID;
                //model.ModifiedDate = DateTime.Now;

            }

            return View(model);
        }


        // GET: Products/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CategoryID = new SelectList(db.ProductCategories, "CategoryID", "CategoryName");
        //    ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName");
        //    return View();
        //}

        // POST: Products/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductSpec,ProductDisc,isDisplay,PurchasePrice,RetailPrice,SellingPrice,SalePrice,StockQty,AvailableQty,VendorID,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", product.CategoryID);
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
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);
            return View(product);
        }

        // POST: Products/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductSpec,ProductDisc,isDisplay,PurchasePrice,RetailPrice,SellingPrice,SalePrice,StockQty,AvailableQty,VendorID,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
