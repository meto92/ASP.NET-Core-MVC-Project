using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.Infrastructure.ComponentViewModels.ProductTypes
{
    public class ProductTypeOptionViewModel : IMapFrom<ProductType>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}