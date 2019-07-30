using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.Infrastructure.ComponentViewModels.ProductTypes
{
    public class ProductTypeLinkViewModel : IMapFrom<ProductType>
    {
        public string Name { get; set; }
    }
}