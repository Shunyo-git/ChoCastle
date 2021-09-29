using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChoCastle.Models
{
    /// <summary>
    /// Image view model class.
    /// </summary>
    public class ImgViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image file.
        /// </summary>
        [Required]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase FileAttach { get; set; }

        /// <summary>
        /// Gets or sets Image file list.
        /// </summary>
        public List<ProductImage> ImgLst { get; set; }


        public int PhotoID { get; set; }

        public int isMain { get; set; }

        public int SortID { get; set; }
        #endregion
    }
}