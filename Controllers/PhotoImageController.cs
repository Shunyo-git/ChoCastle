using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChoCastle.Models;

namespace ChoCastle.Controllers
{

    public class PhotoImageController : Controller
    {
        #region Private Properties

        /// <summary>
        /// Gets or sets database manager property.
        /// </summary>
        private db_imgEntities databaseManager = new db_imgEntities();
        ChoCastle.Models.ChoCastleDBEntities db = new Models.ChoCastleDBEntities();
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

        #region Index view method.

        #region Get: /PhotoImage/Index method.

        /// <summary>
        /// Get: /Img/Index method.
        /// </summary>        
        /// <returns>Return index view</returns>
        public ActionResult Index()
        {
            // Initialization.
            ImgViewModel model = new ImgViewModel { FileAttach = null, ImgLst = new List<ProductImage>() };

            ViewBag.ProductID = new SelectList(db.Products.OrderBy(x => x.CategoryID), "ProductID", "ProductName");


            try
            {
                // Settings.
                //model.ImgLst = this.databaseManager.sp_get_all_files().Select(p => new ProductImage
                //{
                //    file_id = p.file_id,
                //    file_name = p.file_name,
                //    file_ext = p.file_ext,
                //    ProductID = p.ProductID,
                //    isMain = p.isMain,
                //    SortID = p.SortID
                //}).ToList();

                model.ImgLst = da.GetAllProductImage();

                //ViewBag.Base64String = "data:image/png;base64," + Convert.ToBase64String(image.Data, 0, image.Data.Length);
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View(model);
        }

        #endregion

        #region POST: /PhotoImage/Index

        /// <summary>
        /// POST: /Img/Index
        /// </summary>
        /// <param name="model">Model parameter</param>
        /// <returns>Return - Response information</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ProductID,isMain,SortID,FileAttach")] ImgViewModel model)
        {


            // Initialization.
            string fileContent = string.Empty;
            string fileContentType = string.Empty;

            try
            {
                // Verification
                if (ModelState.IsValid)
                {
                    // Converting to bytes.
                    byte[] uploadedFile = new byte[model.FileAttach.InputStream.Length];
                    model.FileAttach.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    // Initialization.
                    fileContent = Convert.ToBase64String(uploadedFile);
                    fileContentType = model.FileAttach.ContentType;

                    // Saving info.
                    //int PhotoID = this.databaseManager.sp_insert_file(model.FileAttach.FileName, fileContentType, fileContent, model.PhotoID, model.isMain, model.SortID);


                    int PhotoID = da.AddProductImage(model.FileAttach.FileName, fileContentType, fileContent, model.ProductID, model.isMain, model.SortID);
                    string _FileName = String.Format("{0}.jpeg", PhotoID);
                    string _path = Path.Combine(Server.MapPath("~/PhotoImages"), _FileName);
                    model.FileAttach.SaveAs(_path);
                    if (model.isMain == 1)
                    {
                        _FileName = String.Format("Main_{0}.jpeg", model.ProductID);
                        _path = Path.Combine(Server.MapPath("~/PhotoImages"), _FileName);
                        model.FileAttach.SaveAs(_path);
                    }

                }

                // Settings.
                //model.ImgLst = this.databaseManager.sp_get_all_files().Select(p => new ProductImage
                //{
                //    file_id = p.file_id,
                //    file_name = p.file_name,
                //    file_ext = p.file_ext,
                //    ProductID = p.ProductID,
                //    isMain = p.isMain,
                //    SortID = p.SortID

                //}).ToList();
                model.ImgLst = da.GetAllProductImage();

            }
            catch (Exception ex)
            {
                // Info
                //Console.Write(ex);
                throw ex;
            }

            ViewBag.ProductID = new SelectList(db.Products.OrderBy(x => x.CategoryID), "ProductID", "ProductName");


            // Info
            return this.View(model);
        }

        #endregion


        // GET: PhotoImage/Delete/5
        public ActionResult Delete(int id, int ProductId)
        {
            RemoveProductPhoto(id);
            this.databaseManager.sp_delete_file(id);

            //return RedirectToAction("Index");
            return RedirectToAction("Edit", "Products", new { id = ProductId });

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RemoveProductPhoto(id);
            this.databaseManager.sp_delete_file(id);
            return RedirectToAction("Index");
        }



        public ActionResult SetMain(int id, int ProductId)
        {

            this.databaseManager.sp_set_main_file(id);

            return RedirectToAction("Edit", "Products", new { id = ProductId });
            //return RedirectToAction("Index");
        }
        #endregion


        private void RemoveProductPhoto(int id) {
            ProductImage img = da.GetPhotoImageByID(id);
            if (img != null)
            {
                this.databaseManager.sp_delete_file(id);
                string _FileName = String.Format("{0}.jpeg", id);
                string path = Path.Combine(Server.MapPath("~/PhotoImages"), _FileName);
                DeleteFile(path);

                _FileName = String.Format("Main_{0}.jpeg", img.ProductID);
                path = Path.Combine(Server.MapPath("~/PhotoImages"), _FileName);
                DeleteFile(path);
            }
        }
        //刪除檔案
        private  void DeleteFile(string path)
        {


            //string path = @"c:\temp\MyTest.txt";
            
            try
            {

                // Delete the file if it exists.
                if (System.IO.File.Exists(path))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Download file methods

        #region GET: /Img/DownloadFile

        /// <summary>
        /// GET: /Img/DownloadFile
        /// </summary>
        /// <param name="fileId">File Id parameter</param>
        /// <returns>Return download file</returns>
        public ActionResult DownloadFile(int fileId)
        {
            // Model binding.
            ImgViewModel model = new ImgViewModel { FileAttach = null, ImgLst = new List<ProductImage>() };

            try
            {
                // Loading dile info.
                //var fileInfo = this.databaseManager.sp_get_file_details(fileId).First();

                var fileInfo = da.GetPhotoImageByID(fileId);
                // Info.
                return this.GetFile(fileInfo.file_base6, fileInfo.file_ext);
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View(model);
        }

        #endregion

        #endregion

        #region Helpers

        #region Get file method.

        /// <summary>
        /// Get file method.
        /// </summary>
        /// <param name="fileContent">File content parameter.</param>
        /// <param name="fileContentType">File content type parameter</param>
        /// <returns>Returns - File.</returns>
        private FileResult GetFile(string fileContent, string fileContentType)
        {
            // Initialization.
            FileResult file = null;

            try
            {
                // Get file.

                byte[] byteContent = Convert.FromBase64String(fileContent);
                file = this.File(byteContent, fileContentType);
            }
            catch (Exception ex)
            {
                // Info.
                throw ex;
            }

            // info.
            return file;
        }

        #endregion




        #endregion
    }
}