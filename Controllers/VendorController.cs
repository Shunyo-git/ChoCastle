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
    public class VendorController : Controller
    {
        ChoCastleDBEntities db = new ChoCastleDBEntities();

        // GET: Vendor
        public ActionResult Index()
        {
            return View(db.Vendors.ToList());
        }

        public ActionResult VendorCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorCreate([Bind(Include = "VendorID,VendorName,Phone,Address,ContactName,ContactEmail,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Vendor vendor)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    vendor.AddedUserID = user.Id;
                    vendor.AddedDate = DateTime.Now;
                }
            }

            db.Vendors.Add(vendor);
            db.SaveChanges();
            return RedirectToAction("Index", vendor);
        }

        public ActionResult VendorEdit(int? id)
        {
            Vendor vendor = db.Vendors.Find(id);
            return View(vendor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorEdit([Bind(Include = "VendorID,VendorName,Phone,Address,ContactName,ContactEmail,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Vendor vendor)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    vendor.ModifiedUserID = user.Id;
                    vendor.ModifiedDate = DateTime.Now;
                }
            }

            db.Entry(vendor).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult VendorDelete(int id)
        {
            Vendor vendor = db.Vendors.Find(id);
            db.Vendors.Remove(vendor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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