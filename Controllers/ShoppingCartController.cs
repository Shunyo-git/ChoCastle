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

namespace ChoCastle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ChoCastleDBEntities1 db = new ChoCastleDBEntities1();

        // GET: ShoppingCart
        public async Task<ActionResult> Index()
        {
            var shoppingDetails = db.ShoppingDetails.Include(s => s.Product).Include(s => s.ShoppingCar);
            return View(await shoppingDetails.ToListAsync());
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SoppingCarOrderDetail()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SoppingCarOrderDetail(int id,string a)
        {

            return View();
        }
    }
}
