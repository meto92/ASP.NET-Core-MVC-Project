using System.Collections.Generic;

namespace Metomarket.Web.ViewModels.Products
{
    public class ProductsListViewModel
    {
        public IEnumerable<ProductHomeViewModel> Products { get; set; }
    }
}