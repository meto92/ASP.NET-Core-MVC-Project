using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductDetailsViewModel : IMapFrom<Product>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int InStock { get; set; }
    }
}