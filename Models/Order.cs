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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Invoices = new HashSet<Invoice>();
        }
    
        public int OrderID { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int MemberID { get; set; }
        public string OrderName { get; set; }
        public string ShipName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public int Delivery { get; set; }
        public int ShippingCost { get; set; }
        public int TotalAmount { get; set; }
        public int Payment { get; set; }
        public System.DateTime PaymentTime { get; set; }
        public int OrderStatus { get; set; }
        public System.DateTime RequiredDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CompanyNumber { get; set; }
        public string InvoiceHeading { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> CarriageCompanyID { get; set; }
        public string TrackingNumber { get; set; }
        public Nullable<System.DateTime> DeliverDate { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
    
        public virtual carriage carriage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
