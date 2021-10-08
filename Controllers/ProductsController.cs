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
using System.IO;
using System.Web.Http.Cors;


namespace ChoCastle.Controllers
{


    public class ProductsController : Controller
    {
        private ChoCastleDBEntities db = new ChoCastleDBEntities();

        #region Private Properties

        /// <summary>
        /// Gets or sets database manager property.
        /// </summary>
        private db_imgEntities databaseManager = new db_imgEntities();
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
        #endregion



        // GET: Index
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public ActionResult Index()
        {
            //http://localhost:11775/Products
            //Response.AppendHeader("Access-Control-Allow-Origin", "*");
            //ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //var products = db.Products.Include(p => p.ProductCategory).Include(p => p.Vendor);
            //return View(products.ToList());

            var date = DateTime.Now;
            var orders = (from od in db.Orders 
                          where DbFunctions.DiffDays( od.OrderDate, date) < 30
                          select od);
            string strAmount = string.Empty;
            int TotalAmount = 0;
            foreach (var od in orders)
            {
                if (strAmount != string.Empty)
                {
                    strAmount += ",";
                }
                strAmount += od.TotalAmount;
                TotalAmount += (int)od.TotalAmount;
            }

            ViewBag.LastOrders = strAmount;
            ViewBag.LastAmount = TotalAmount;
            return View(orders.ToList());
        }

        public ActionResult ProductManage()
        {
            //下拉式選單
            viewModel model = new viewModel();
            List<ProductCategory> productCategories = db.ProductCategories.ToList();
            List<SelectListItem> lst_item = new List<SelectListItem>();
            lst_item.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (productCategories.Count > 0)
            {
                foreach (var item in productCategories)
                {
                    lst_item.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryID.ToString() });
                }
            }

            List<SelectListItem> lst_display = new List<SelectListItem>();
            lst_display.Add(new SelectListItem { Text = "請選擇", Value = "" });
            lst_display.Add(new SelectListItem { Text = "上架", Value = "true" });
            lst_display.Add(new SelectListItem { Text = "下架", Value = "false" });

            //列表
            var result = (from pd in db.Products
                          join ca in db.ProductCategories on pd.CategoryID equals ca.CategoryID
                          where
                          (
                          (pd.ProductID == model.ProductID || model.ProductID == null) &&
                          (pd.CategoryID == model.CategoryID || model.CategoryID == null) &&
                          (pd.ProductName.Contains(model.ProductName) || string.IsNullOrEmpty(model.ProductName)) &&
                          (pd.isDisplay == model.isDisplay || model.isDisplay == null)
                          )
                          select pd).ToList();

            model.lst_Product = result;
            model.lst_Selectitem_CID = lst_item;
            model.lst_Selectitem_Display = lst_display;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductManage(viewModel rita)
        {
            List<ProductCategory> productCategories = db.ProductCategories.ToList();
            List<SelectListItem> lst_item = new List<SelectListItem>();
            List<SelectListItem> lst_display = new List<SelectListItem>();
            if (productCategories.Count > 0)
            {
                lst_item.Add(new SelectListItem { Text = "請選擇", Value = "" });
                foreach (var item in productCategories)
                {
                    lst_item.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryID.ToString() });
                }
            }

            lst_display.Add(new SelectListItem { Text = "請選擇", Value = "" });
            lst_display.Add(new SelectListItem { Text = "上架", Value = "true" });
            lst_display.Add(new SelectListItem { Text = "下架", Value = "false" });

            var result = (from pd in db.Products
                          join ca in db.ProductCategories on pd.CategoryID equals ca.CategoryID
                          where
                          (
                          (pd.ProductID == rita.ProductID || rita.ProductID == null) &&
                          (pd.CategoryID == rita.CategoryID || rita.CategoryID == null) &&
                          (pd.ProductName.Contains(rita.ProductName) || string.IsNullOrEmpty(rita.ProductName)) &&
                          (pd.isDisplay == rita.isDisplay || rita.isDisplay == null)
                          )
                          select pd).ToList();

            rita.lst_Product = result;
            rita.lst_Selectitem_CID = lst_item;
            rita.lst_Selectitem_Display = lst_display;
            return View(rita);
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

            var user = UserManager.FindById(User.Identity.GetUserId());
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
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductSpec,ProductDisc,isDisplay,PurchasePrice,RetailPrice,SellingPrice,SalePrice,StockQty,AvailableQty,VendorID,AddedDate,AddedUserID,ModifiedDate,ModifiedUserID")] Product product, List<HttpPostedFileBase> FileAttach)
        {
            var userId = User.Identity.GetUserId();


            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    product.AddedUserID = user.MemberID;
                    product.AddedDate = DateTime.Now;
                }

                db.Products.Add(product);
                db.SaveChanges();

                #region 檔案上傳
                if (FileAttach.Count > 0 && FileAttach.FirstOrDefault() != null)
                {
                    // Initialization.
                    string fileContent = string.Empty;
                    string fileContentType = string.Empty;
                    foreach (var pic in FileAttach)
                    {
                        // Converting to bytes.
                        byte[] uploadedFile = new byte[pic.InputStream.Length];
                        pic.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                        // Initialization.
                        fileContent = Convert.ToBase64String(uploadedFile);
                        fileContentType = pic.ContentType;

                        // Saving info.
                        //int PhotoID = this.databaseManager.sp_insert_file(model.FileAttach.FileName, fileContentType, fileContent, model.PhotoID, model.isMain, model.SortID);


                        int PhotoID = da.AddProductImage(pic.FileName, fileContentType, fileContent, product.ProductID, 0, 0); //產品KEY、是否為主要圖片、排序
                        string _FileName = String.Format("{0}.jpeg", PhotoID);
                        string _path = Path.Combine(Server.MapPath("~/PhotoImages"), _FileName);
                        pic.SaveAs(_path);
                    }
                }

                #endregion

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

            Product product = new Product();
            product = db.Products.Find(id);
            List<ProductImage> lst_image = new List<ProductImage>();
            lst_image = db.ProductImages.Where(model => model.ProductID == id).ToList();



            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.ProductCategories.OrderBy(model => model.SortID), "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", product.VendorID);

            ImgResult imgResult = new ImgResult();
            imgResult.Product = product;
            imgResult.ProductImage = lst_image;

            return View(imgResult);
        }

        // POST: Products/Edit/5
        // 2021/9/21 By Rita
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int CategoryID, int VendorID, ImgResult data, List<HttpPostedFileBase> FileAttach)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                data.Product.CategoryID = CategoryID;
                data.Product.VendorID = VendorID;
                if (user != null)
                {
                    data.Product.AddedUserID = user.MemberID;
                    data.Product.AddedDate = DateTime.Now;
                }
                db.Entry(data.Product).State = EntityState.Modified;
                db.SaveChanges();

                #region 檔案上傳
                if (FileAttach.Count > 0 && FileAttach.FirstOrDefault() != null)
                {
                    // Initialization.
                    string fileContent = string.Empty;
                    string fileContentType = string.Empty;
                    foreach (var pic in FileAttach)
                    {
                        // Converting to bytes.
                        byte[] uploadedFile = new byte[pic.InputStream.Length];
                        pic.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                        // Initialization.
                        fileContent = Convert.ToBase64String(uploadedFile);
                        fileContentType = pic.ContentType;

                        // Saving info.
                        //int PhotoID = this.databaseManager.sp_insert_file(model.FileAttach.FileName, fileContentType, fileContent, model.PhotoID, model.isMain, model.SortID);


                        int PhotoID = da.AddProductImage(pic.FileName, fileContentType, fileContent, data.Product.ProductID, 0, 0); //產品KEY、是否為主要圖片、排序
                        string _FileName = String.Format("{0}.jpeg", PhotoID);
                        string _path = Path.Combine(Server.MapPath("~/PhotoImages"), _FileName);
                        pic.SaveAs(_path);
                    }
                }
                #endregion
                return RedirectToAction("ProductManage");
            }
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "CategoryID", "CategoryName", data.Product.CategoryID);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", data.Product.VendorID);
            return View(data);
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
            viewModel model = new viewModel();
            List<ProductCategory> productCategories = db.ProductCategories.ToList();
            List<SelectListItem> lst_item = new List<SelectListItem>();
            lst_item.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (productCategories.Count > 0)
            {
                foreach (var item in productCategories)
                {
                    lst_item.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryName.ToString() });
                }
            }

            List<SelectListItem> lst_display = new List<SelectListItem>();
            lst_display.Add(new SelectListItem { Text = "請選擇", Value = "" });
            lst_display.Add(new SelectListItem { Text = "上架", Value = "true" });
            lst_display.Add(new SelectListItem { Text = "下架", Value = "false" });

            var result = (from ca in db.ProductCategories
                          where
                          (
                          (ca.CategoryID == model.CategoryID || model.CategoryID == null) &&
                          (ca.CategoryName.Contains(model.CategoryName) || string.IsNullOrEmpty(model.CategoryName)) &&
                          (ca.isDisplay == model.isDisplay || model.isDisplay == null)
                          )
                          select ca).ToList();

            model.lst_Category = result;
            model.lst_Selectitem_CID = lst_item;
            model.lst_Selectitem_Display = lst_display;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductCategory(viewModel rita)
        {
            viewModel model = rita;
            List<ProductCategory> productCategories = db.ProductCategories.ToList();
            List<SelectListItem> lst_item = new List<SelectListItem>();
            lst_item.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (productCategories.Count > 0)
            {
                foreach (var item in productCategories)
                {
                    lst_item.Add(new SelectListItem { Text = item.CategoryName, Value = item.CategoryName.ToString() });
                }
            }

            List<SelectListItem> lst_display = new List<SelectListItem>();
            lst_display.Add(new SelectListItem { Text = "請選擇", Value = "" });
            lst_display.Add(new SelectListItem { Text = "上架", Value = "true" });
            lst_display.Add(new SelectListItem { Text = "下架", Value = "false" });

            var result = (from ca in db.ProductCategories
                          where
                          (
                          (ca.CategoryID == model.CategoryID || model.CategoryID == null) &&
                          (ca.CategoryName.Contains(model.CategoryName) || string.IsNullOrEmpty(model.CategoryName)) &&
                          (ca.isDisplay == model.isDisplay || model.isDisplay == null)
                          )
                          select ca).ToList();

            rita.lst_Category = result;
            rita.lst_Selectitem_CID = lst_item;
            rita.lst_Selectitem_Display = lst_display;

            return View(rita);
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

        public class ImgResult
        {
            public Product Product { get; set; }
            public List<ProductImage> ProductImage { get; set; }
        }

        public class viewModel
        {
            public Int32? ProductID { get; set; }
            public Int32? CategoryID { get; set; }
            public String ProductName { get; set; }
            public String CategoryName { get; set; }
            public Boolean? isDisplay { get; set; }
            public List<Product> lst_Product { get; set; }
            public List<ProductCategory> lst_Category { get; set; }
            public List<SelectListItem> lst_Selectitem_CID { get; set; }
            public List<SelectListItem> lst_Selectitem_Display { get; set; }
        }
    }
}
