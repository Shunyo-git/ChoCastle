using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChoCastle.Models;
using System.Data.SqlClient;


using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace ChoCastle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ChoCastleDBEntities1 db = new ChoCastleDBEntities1();

        // GET: ShoppingCart
        public ActionResult Index()
        {

            //20210921 by sean
            int CartID = 0;


            if (Session["CartID"] != null)
            {
                CartID = Int32.Parse(Session["CartID"].ToString());
            }
            var p0 = new SqlParameter("p0", CartID);
            var parameters = new SqlParameter[] { p0 };
            
            // return View(await db.ShoppingDetails.Include(s => s.Product).Include(s => s.ShoppingCar).ToListAsync());
            var shoppingDetails =  db.ShoppingDetails.SqlQuery("SELECT * FROM ShoppingDetail WHERE CarID = @p0", parameters).ToList();
            return View(shoppingDetails);




        }

        // GET: ShoppingCart/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingDetail shoppingDetail = await db.ShoppingDetails.FindAsync(id);
            if (shoppingDetail == null)
            {
                return HttpNotFound();
            }
            return View(shoppingDetail);
        }

        // GET: ShoppingCart/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.CarID = new SelectList(db.ShoppingCars, "CarID", "OrderName");
            return View();
        }

        // POST: ShoppingCart/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CarID,ProductID,ProductName,UnitPrice,OrderQuantity,Subtotal,AddedDate,ModifiedDate")] ShoppingDetail shoppingDetail)
        {
            if (ModelState.IsValid)
            {
                db.ShoppingDetails.Add(shoppingDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", shoppingDetail.ProductID);
            ViewBag.CarID = new SelectList(db.ShoppingCars, "CarID", "OrderName", shoppingDetail.CarID);
            return View(shoppingDetail);
        }

        // GET: ShoppingCart/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingDetail shoppingDetail = await db.ShoppingDetails.FindAsync(id);
            if (shoppingDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", shoppingDetail.ProductID);
            ViewBag.CarID = new SelectList(db.ShoppingCars, "CarID", "OrderName", shoppingDetail.CarID);
            return View(shoppingDetail);
        }

        // POST: ShoppingCart/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CarID,ProductID,ProductName,UnitPrice,OrderQuantity,Subtotal,AddedDate,ModifiedDate")] ShoppingDetail shoppingDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shoppingDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", shoppingDetail.ProductID);
            ViewBag.CarID = new SelectList(db.ShoppingCars, "CarID", "OrderName", shoppingDetail.CarID);
            return View(shoppingDetail);
        }

        // GET: ShoppingCart/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingDetail shoppingDetail = await db.ShoppingDetails.FindAsync(id);
            if (shoppingDetail == null)
            {
                return HttpNotFound();
            }
            return View(shoppingDetail);
        }

        // POST: ShoppingCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShoppingDetail shoppingDetail = await db.ShoppingDetails.FindAsync(id);
            db.ShoppingDetails.Remove(shoppingDetail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            //20210921 by sean
            int CartID = 0;
            if (Session["CartID"] != null)
            {
                CartID = Int32.Parse(Session["CartID"].ToString());
            }
            var p0 = new SqlParameter("p0", CartID);
            var p1 = new SqlParameter("p1", id);
            var parameters = new SqlParameter[] { p0, p1 };

            // return View(await db.ShoppingDetails.Include(s => s.Product).Include(s => s.ShoppingCar).ToListAsync());
            var shoppingDetails = db.ShoppingDetails.SqlQuery("SELECT * FROM ShoppingDetail WHERE CarID = @p0 and ProductID = @p1", parameters).ToList();

            db.ShoppingDetails.RemoveRange(shoppingDetails);
            await db.SaveChangesAsync();
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

        //GET
        public ActionResult ShoppingCartDetail()
        {

            int CartID = 0;
            ShoppingCar shoppingCart;
            if (Session["CartID"] != null)
            {
                CartID = Int32.Parse(Session["CartID"].ToString());
                shoppingCart = db.ShoppingCars.Find(CartID);
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    shoppingCart.MemberID = user.MemberID;
                    shoppingCart.isLogin = 1;
                    db.Entry(shoppingCart).State = EntityState.Modified;
                }
                db.SaveChanges();
                return View(shoppingCart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShoppingCartDetail([Bind(Include = "CarID, isLogin, MemberID, OrderName, ShipName, PhoneNumber, ShippingAddress, Delivery,  ShippingCost, TotalAmount, Payment, RequiredDate, AddedDate, ModifiedDate, CompanyNumber, InvoiceHeading,  InvoiceType")] ShoppingCar shoppingCart)
        {

            //20210921 by sean
             
            if (ModelState.IsValid)
            { 
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    shoppingCart.MemberID = user.MemberID;
                    shoppingCart.isLogin = 1;                   
                    shoppingCart.ModifiedDate = DateTime.Now;
                }

                //ShoppingDetail shoppingDetail = db.ShoppingDetails.Find(CartID);

                //if (shoppingDetail == null)
                //{
                //    shoppingDetail = new ShoppingDetail();
                //    shoppingDetail.CarID = CartID;
                //    shoppingDetail.OrderQuantity = Int32.Parse(Request.Form["OrderQty"]);
                //    shoppingDetail.ProductID = product.ProductID;
                //    shoppingDetail.ProductName = product.ProductName;
                //    shoppingDetail.UnitPrice = product.SellingPrice;
                //    shoppingDetail.Subtotal = shoppingDetail.UnitPrice * shoppingDetail.OrderQuantity;
                //    shoppingDetail.AddedDate = DateTime.Now;

                //    db.ShoppingDetails.Add(shoppingDetail);
                //}
                //else
                //{
                //    shoppingDetail.OrderQuantity += 1;
                //    shoppingDetail.Subtotal = shoppingDetail.UnitPrice * shoppingDetail.OrderQuantity;

                //    db.Entry(shoppingDetail).State = EntityState.Modified;
                //    shoppingCart.TotalAmount = 0;
                //}

                db.Entry(shoppingCart).State = EntityState.Modified;
                db.SaveChanges();


                Order newOrder = new Order();
                newOrder.CompanyNumber = shoppingCart.CompanyNumber;
               // newOrder.Delivery = (int)shoppingCart.Delivery;
                newOrder.InvoiceHeading = shoppingCart.InvoiceHeading;
                newOrder.InvoiceType = shoppingCart.InvoiceType;
                newOrder.MemberID = (int)shoppingCart.MemberID;
                newOrder.OrderDate = DateTime.Now;
                newOrder.OrderName = shoppingCart.OrderName;
                newOrder.OrderStatus = 0;
                newOrder.PhoneNumber = shoppingCart.PhoneNumber;
                newOrder.RequiredDate = (DateTime)shoppingCart.RequiredDate;
                newOrder.ShipName = shoppingCart.ShipName;
                newOrder.ShippingAddress = shoppingCart.ShippingAddress;
                // newOrder.TotalAmount = ;
                db.Orders.Add(newOrder);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "ShoppingCart");

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
    }
}
