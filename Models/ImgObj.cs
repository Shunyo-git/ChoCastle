using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChoCastle.Models
{
    /// <summary>
    /// Image object class.
    /// </summary>
    public class ImgObj2
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image ID.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets Image name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets Image extension.
        /// </summary>
        public string FileContentType { get; set; }

        public int PhotoID { get; set; }

        public int isMain { get; set; }

        public int SortID { get; set; }
        #endregion
    }
}