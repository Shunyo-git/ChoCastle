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

        public ActionResult OrderDetail(int id)
        {
            OrderDetaillResult result = new OrderDetaillResult();
            if (db.Orders.Find(id)!=null) {
                result.order = db.Orders.Find(id);
                result.Detail = db.OrderDetails.Where(model => model.OrderID == id).ToList();
                result.user = db.AspNetUsers.Where(model => model.MemberID == result.order.MemberID).FirstOrDefault();
                foreach (var item in result.Detail)
                {
                    result.DetailTotal += item.Price * item.Qty;
                }
                DeliveryType dt = (DeliveryType)result.order.Delivery;
                ViewBag.DeliveryType = MyEnumHelper<DeliveryType>.GetDisplayValue(dt);
                InvoiceType Invoice = (InvoiceType)result.order.InvoiceType;
                ViewBag.InvoiceType = MyEnumHelper<InvoiceType>.GetDisplayValue(Invoice);

                PaymentType payment = (PaymentType)result.order.Payment;
                ViewBag.PaymentType = MyEnumHelper<PaymentType>.GetDisplayValue(payment);

                if (result.order.PaymentTime != null)
                {
                    DateTime PaymentTime = (DateTime)result.order.PaymentTime;
                    ViewBag.PaymentTime = String.Format("已付款(付款時間 {0})", PaymentTime.ToString("MM/dd/yyyy H:mm")); 
                }
                else {
                    ViewBag.PaymentTime = "待付款";
                }
               
            }
            

            return View(result);
        }

        public class OrderDetaillResult
        {
            public Order order { get; set; }
            public List<OrderDetail> Detail { get; set; }
            public AspNetUser user { get; set; }
            public int DetailTotal = 0;
        }
    }
}