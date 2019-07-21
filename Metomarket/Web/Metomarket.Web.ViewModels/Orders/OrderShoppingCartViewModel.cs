using Metomarket.Web.ViewModels.Products;

namespace Metomarket.Web.ViewModels.Orders
{
    public class OrderShoppingCartViewModel
    {
        public string Id { get; set; }

        public int Quantity { get; set; }

        public ProductShoppingCartViewModel Product { get; set; }
    }
}