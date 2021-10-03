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
    public class CarriageController : Controller
    {
        ChoCastleDBEntities db = new ChoCastleDBEntities();
        // GET: Carriage
        public ActionResult Index()
        {
            
            return View(db.Carriages.ToList());
        }

        public ActionResult CarriageCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CarriageCreate([Bind(Include = "CarriageCompanyID,CarriageName,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Carriage carriage)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    carriage.AddedUserID = user.Id;
                    carriage.AddedDate = DateTime.Now;
                }
            }

            db.Carriages.Add(carriage);
            db.SaveChanges();
            return RedirectToAction("Index", carriage);
        }

        public ActionResult CarriageEdit(int? id)
        {
            Carriage carriage = db.Carriages.Find(id);
            return View(carriage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CarriageEdit([Bind(Include = "CarriageCompanyID,CarriageName,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Carriage carriage)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    carriage.ModifiedUserID = user.Id;
                    carriage.ModifiedDate = DateTime.Now;
                }
            }

            db.Entry(carriage).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CarriageDelete(int id)
        {
            Carriage carriage = db.Carriages.Find(id);
            db.Carriages.Remove(carriage);
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