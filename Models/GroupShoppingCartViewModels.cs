using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChoCastle.Models
{
    public class GroupShoppingCartViewModels
    {
        public ShoppingCar ShoppingCart { get; set; }
        public List<ShoppingDetail> ShoppingDetails { get; set; }
    }
}