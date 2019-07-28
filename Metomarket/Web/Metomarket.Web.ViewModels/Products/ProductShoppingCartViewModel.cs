using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductShoppingCartViewModel : IMapFrom<Product>
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }
    }
}