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
    
    public partial class ShoppingCar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ShoppingCar()
        {
            this.ShoppingDetails = new HashSet<ShoppingDetail>();
        }
    
        public int CarID { get; set; }
        public int isLogin { get; set; }
        public Nullable<int> MemberID { get; set; }
        public string OrderName { get; set; }
        public string ShipName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public Nullable<int> Delivery { get; set; }
        public Nullable<int> ShippingCost { get; set; }
        public Nullable<int> TotalAmount { get; set; }
        public Nullable<int> Payment { get; set; }
        public Nullable<System.DateTime> RequiredDate { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string CompanyNumber { get; set; }
        public string InvoiceHeading { get; set; }
        public Nullable<int> InvoiceType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingDetail> ShoppingDetails { get; set; }
    }
}
