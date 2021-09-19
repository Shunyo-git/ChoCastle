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

    public partial class ShoppingDetail
    {
        public int CarID { get; set; }
        public Nullable<int> ProductID { get; set; }

        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "單價")]
        public Nullable<int> UnitPrice { get; set; }

        [Display(Name = "數量")]
        public Nullable<int> OrderQuantity { get; set; }

        [Display(Name = "小計")]
        public Nullable<int> Subtotal { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string ModifiedDate { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ShoppingCar ShoppingCar { get; set; }
    }
}
