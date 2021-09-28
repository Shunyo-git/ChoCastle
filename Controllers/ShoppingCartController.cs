using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ChoCastle.Models;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;
using System.Reflection;

namespace ChoCastle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ChoCastleDBEntities db = new ChoCastleDBEntities();

        //2021/9/24 DataAccessFactory.CreateDefaultDataAccess()
        //資料庫存取提供者
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

        // GET: ShoppingCart購物車首頁
        public ActionResult Index()
        {
            int CartID = 0;
            int isRestored = 0;

            //20210921 by sean
            //讀取Session購物車編號
            if (Session["CartID"] != null)
            {
                CartID = Int32.Parse(Session["CartID"].ToString());
            }
            if (Session["isRestored"] != null)
            {
                isRestored = Int32.Parse(Session["isRestored"].ToString());
            }

            //2021/9/24 by sean
            //如為會員 取得購物車紀錄
            if (User.Identity.IsAuthenticated)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    //如為會員 取得購物車紀錄
                    if (isRestored == 0 && user.MemberID > 0)
                    {
                        int PrevCartID = da.GetCartID(user.MemberID);

                        //如有舊購物車取回購物車
                        if (PrevCartID > 0)
                        {
                            if (CartID == 0)
                            {
                                CartID = PrevCartID;
                            }
                            else
                            {
                                da.RemovePreviousCart(user.MemberID, CartID);
                            }
                        }

                        Session["isRestored"] = 1;
                        Session["CartID"] = CartID;
                        Session["MemberID"] = user.MemberID;
                    }
                }
            }

            //var p0 = new SqlParameter("p0", CartID);
            //var parameters = new SqlParameter[] { p0 };

            //var shoppingDetails = db.ShoppingDetails.SqlQuery("SELECT * FROM ShoppingDetail WHERE CarID = @p0", parameters).ToList();

            var shoppingDetails = da.GetShoppingDetailsByCart(CartID);
            return View(shoppingDetails);
            //return View(  db.ShoppingDetails.Include(s => s.Product).Include(s => s.ShoppingCar).ToListAsync());


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
        public ActionResult Delete(int ProductID, int CartID)
        {
            //20210924 by sean
            da.RemoveShoppingDetail(CartID, ProductID);
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
        // ShoppingCart/ShoppingCartDetail
        public ActionResult ShoppingCartDetail()
        {

            int CartID = 0;
            ShoppingCar shoppingCart;
            if (Session["CartID"] != null)
            {
                CartID = Int32.Parse(Session["CartID"].ToString());
                shoppingCart = db.ShoppingCars.Find(CartID);
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null && shoppingCart != null)
                {
                    shoppingCart.MemberID = user.MemberID;
                    shoppingCart.isLogin = 1;
                    if (shoppingCart.InvoiceType is null)
                    {
                        shoppingCart.InvoiceType = 1;
                    }
                    db.Entry(shoppingCart).State = EntityState.Modified;
                    db.SaveChanges();
                    return View(shoppingCart);
                }
                return RedirectToAction("Login", "Account", new { returnUrl = "/ShoppingCart" });

            }

            return RedirectToAction("Index");
        }

        //儲存購物相關資料
        // ShoppingCart/ShoppingCartDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShoppingCartDetail([Bind(Include = "CarID, isLogin, MemberID, OrderName, ShipName, PhoneNumber, ShippingAddress, Delivery,Payment,  ShippingCost, TotalAmount, Payment, RequiredDate, AddedDate, ModifiedDate, CompanyNumber, InvoiceHeading,  InvoiceType")] ShoppingCar shoppingCart)
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


                //轉換寄送方式,發票種類說明,付款方式
                //ViewBag.DeliveryType = Enum.GetName(typeof(DeliveryType), shoppingCart.Delivery);
                DeliveryType delivery = (DeliveryType)shoppingCart.Delivery;
                ViewBag.DeliveryType = MyEnumHelper<DeliveryType>.GetDisplayValue(delivery);

                InvoiceType Invoice = (InvoiceType)shoppingCart.InvoiceType;
                ViewBag.InvoiceType = MyEnumHelper<InvoiceType>.GetDisplayValue(Invoice);

                PaymentType payment = (PaymentType)shoppingCart.Payment;
                ViewBag.PaymentType = MyEnumHelper<PaymentType>.GetDisplayValue(payment);

                //計算運費
                int ShippingCost = 100;
                switch (delivery)
                {
                    case DeliveryType.FamilyStore:
                    case DeliveryType.SevenStore:
                        ShippingCost = 60;
                        break;
                    default:
                        ShippingCost = 120;
                        break;
                }
                shoppingCart.ShippingCost = ShippingCost;
                shoppingCart.Payment = Int32.Parse(Request.Form["Payment"]);

                //計算購物總價
                var p0 = new SqlParameter("CarID", shoppingCart.CarID);
                var parameters = new SqlParameter[] { p0 };
                int ItemAmount = db.Database.SqlQuery<int>("SELECT SUM (OrderQuantity*UnitPrice) AS TotalAmount FROM ShoppingDetail WHERE CarID =@CarID", parameters).ToList()[0];

                //更新購物車總金額
                shoppingCart.TotalAmount = ItemAmount + ShippingCost;
                db.Entry(shoppingCart).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("OrderConfirmation", "ShoppingCart");
            }

            //ModelState.AddModelError("", "訂單已完成。");

            return RedirectToAction("Login", "Account", "ShoppingCart");

            //return View(shoppingCart);
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

        // 2021/9/21 by sean
        // GET: /ShoppingCart/OrderConfirmation
        [HttpGet]
        public ActionResult OrderConfirmation()
        {


            //ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            //ViewBag.CarID = new SelectList(db.ShoppingCars, "CarID", "OrderName");

            GroupShoppingCartViewModels CartModel = new GroupShoppingCartViewModels();
            int CartID = 0;
            ShoppingCar shoppingCart;
            if (Session["CartID"] != null)
            {
                //取得購物車
                CartID = Int32.Parse(Session["CartID"].ToString());
                shoppingCart = db.ShoppingCars.Find(CartID);
                if (shoppingCart != null)
                {

                    //轉換寄送方式與發票種類說明
                    //ViewBag.DeliveryType = Enum.GetName(typeof(DeliveryType), shoppingCart.Delivery);
                    DeliveryType dt = (DeliveryType)shoppingCart.Delivery;
                    ViewBag.DeliveryType = MyEnumHelper<DeliveryType>.GetDisplayValue(dt);

                    InvoiceType Invoice = (InvoiceType)shoppingCart.InvoiceType;
                    ViewBag.InvoiceType = MyEnumHelper<InvoiceType>.GetDisplayValue(Invoice);

                    PaymentType payment = (PaymentType)shoppingCart.Payment;
                    ViewBag.PaymentType = MyEnumHelper<PaymentType>.GetDisplayValue(payment);

                    DateTime requiredDate = (DateTime)shoppingCart.RequiredDate;
                    ViewBag.RrequiredDate = requiredDate.ToString("yyyy/MM/dd");
                    var shoppingDetails = da.GetShoppingDetailsByCart(shoppingCart.CarID);
                    CartModel.ShoppingDetails = shoppingDetails;
                }

                CartModel.ShoppingCart = shoppingCart;
                return View(CartModel);
            }
            return RedirectToAction("Index");

        }

        // ShoppingCart/OrderConfirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderConfirmation([Bind(Include = "CarID")] ShoppingCar shoppingCart)
        {

            if (Session["CartID"] != null)
            {
                int CartID = Int32.Parse(Session["CartID"].ToString());


                if (db.ShoppingCars.Find(CartID) != null)
                {
                    int OrderID = da.AddOrder(CartID);

                    if (OrderID > 0)
                    {
                        Session["CartID"] = null;
                        ViewBag.OrderID = OrderID;
                    }


                }

            }

            return View();
        }

    }
}
