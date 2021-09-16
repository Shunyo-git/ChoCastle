//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChoCastle.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class ProductCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Display(Name = "分類編號")]
        public int CategoryID { get; set; }
        [Display(Name = "分類名稱")]
        public string CategoryName { get; set; }
        [Display(Name = "排序值")]
        public int SortID { get; set; }
        [Display(Name = "上架")]
        public bool isDisplay { get; set; }
        [Display(Name = "新增日期")]
        public System.DateTime AddedDate { get; set; }
        [Display(Name = "新增者編號")]
        public int AddedUserID { get; set; }
        [Display(Name = "更新日期")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [Display(Name = "更新者編號")]
        public Nullable<int> ModifiedUserID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
