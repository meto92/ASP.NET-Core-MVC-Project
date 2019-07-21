using System.Collections.Generic;
using System.Linq;

using Metomarket.Web.ViewModels.Orders;

namespace Metomarket.Web.ViewModels.ShoppingCart
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<OrderShoppingCartViewModel> Orders { get; set; }

        public decimal Total => this.Orders
            .Sum(order => order.Quantity * order.Product.Price);
    }
}