using System.Collections.Generic;
using System.Linq;

using Metomarket.Data.Models;
using Metomarket.Services.Mapping;
using Metomarket.Web.ViewModels.Orders;

namespace Metomarket.Web.ViewModels.ShoppingCarts
{
    public class ShoppingCartViewModel : IMapFrom<ShoppingCart>
    {
        public IEnumerable<OrderShoppingCartViewModel> Orders { get; set; }

        public decimal Total => this.Orders
            .Sum(order => order.Quantity * order.Product.Price);
    }
}