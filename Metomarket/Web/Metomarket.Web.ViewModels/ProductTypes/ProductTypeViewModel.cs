using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.ProductTypes
{
    public class ProductTypeViewModel : IMapFrom<ProductType>
    {
        public string Name { get; set; }

        public int ProductsCount { get; set; }
    }
}