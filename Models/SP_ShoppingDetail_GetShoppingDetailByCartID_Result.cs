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
    
    public partial class SP_ShoppingDetail_GetShoppingDetailByCartID_Result
    {
        public int Id { get; set; }
        public int CarID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> UnitPrice { get; set; }
        public Nullable<int> OrderQuantity { get; set; }
        public Nullable<int> Subtotal { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string ModifiedDate { get; set; }
    }
}
