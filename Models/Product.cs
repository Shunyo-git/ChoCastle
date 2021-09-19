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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.ShoppingDetails = new HashSet<ShoppingDetail>();
        }
    
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public string ProductSpec { get; set; }
        public string ProductDisc { get; set; }
        public Nullable<bool> isDisplay { get; set; }
        public Nullable<int> PurchasePrice { get; set; }
        public int RetailPrice { get; set; }
        public int SellingPrice { get; set; }
        public Nullable<int> SalePrice { get; set; }
        public Nullable<int> StockQty { get; set; }
        public Nullable<int> AvailableQty { get; set; }
        public Nullable<int> VendorID { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<int> AddedUserID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedUserID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual Vendor Vendor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingDetail> ShoppingDetails { get; set; }
    }
}
