using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChoCastle.Models
{


    public enum OrderStatus
    {
        [Display(Name = "待處理")]
        NewOrder = 0,
        [Display(Name = "處理中")]
        Processing = 1,
        [Display(Name = "已出貨")]
        Shipped = 2,
        [Display(Name = "已取消")]
        Cancelled = 3
    }

    public enum DeliveryType
    {
        [Display(Name = "未設定")]
        Default = 0,
        [Display(Name = "宅配")]
        HomeDelivery = 1,
        [Display(Name = "7-11到店取貨")]
        SevenStore = 2,
        [Display(Name = "全家到店取貨")]
        FamilyStore = 3


    }

    public enum InvoiceType
    {
        [Display(Name = "未設定")]
        NotSet = 0,
        [Display(Name = "二聯式")]
        DuplicateUniformInvoice = 1,
        [Display(Name = "三聯式")]
        TriplicateUniformInvoice = 2,
        [Display(Name = "電子發票")]
        EInvoice = 3
    }
    public enum PaymentType
    {
        [Display(Name = "未設定")]
        NotSet = 0,
        [Display(Name = "ATM轉帳")]
        PayByATM = 1,
        [Display(Name = "超商取貨付款")]
        PayByStore = 2,
      
    }
}