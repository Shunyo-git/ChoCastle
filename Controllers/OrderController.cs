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
    public class OrderController : Controller
    {
        ChoCastleDBEntities1 db = new ChoCastleDBEntities1();
        // GET: Order
        public ActionResult Index()
        {   
            return View(db.Orders.ToList());
        }
    }
}