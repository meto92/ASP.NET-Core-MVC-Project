using Metomarket.Data.Models;
using Metomarket.Services.Mapping;
using Metomarket.Web.ViewModels.Products;

namespace Metomarket.Web.ViewModels.Orders
{
    public class OrderShoppingCartViewModel : IMapFrom<Order>
    {
        public string Id { get; set; }

        public int Quantity { get; set; }

        public ProductShoppingCartViewModel Product { get; set; }
    }
}