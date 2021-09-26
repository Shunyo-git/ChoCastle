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
using System.ComponentModel.DataAnnotations;
namespace ChoCastle.Controllers
{
    public enum OrderStatus
    {
        [Display(Name = "待處理")]
        NewOrder = 0,
        [Display(Name = "處理中")]
        Processing = 1,
        [Display(Name = "已出貨")]
        Shipped = 2,
        [Display(Name = "已取消")]
        Cancelled = 3
    }

    public class OrdersController : Controller
    {
        private ChoCastleDBEntities1 db = new ChoCastleDBEntities1();

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Include(o => o.Carriage).Include(o => o.OrderDetail);
            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.Carriages, "CarriageCompanyID", "CarriageName");
            ViewBag.OrderID = new SelectList(db.OrderDetails, "OrderID", "ProductName");
            return View();
        }

        // POST: Orders/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderID,OrderDate,MemberID,OrderName,ShipName,PhoneNumber,ShippingAddress,Delivery,ShippingCost,TotalAmount,Payment,PaymentTime,OrderStatus,RequiredDate,InvoiceNo,CompanyNumber,InvoiceHeading,InvoiceType,CarriageCompanyID,TrackingNumber,DeliverDate,ReceiveDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.Carriages, "CarriageCompanyID", "CarriageName", order.OrderID);
            ViewBag.OrderID = new SelectList(db.OrderDetails, "OrderID", "ProductName", order.OrderID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Carriages, "CarriageCompanyID", "CarriageName", order.OrderID);
            ViewBag.OrderID = new SelectList(db.OrderDetails, "OrderID", "ProductName", order.OrderID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderID,OrderDate,MemberID,OrderName,ShipName,PhoneNumber,ShippingAddress,Delivery,ShippingCost,TotalAmount,Payment,PaymentTime,OrderStatus,RequiredDate,InvoiceNo,CompanyNumber,InvoiceHeading,InvoiceType,CarriageCompanyID,TrackingNumber,DeliverDate,ReceiveDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Carriages, "CarriageCompanyID", "CarriageName", order.OrderID);
            ViewBag.OrderID = new SelectList(db.OrderDetails, "OrderID", "ProductName", order.OrderID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
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
    }
}
