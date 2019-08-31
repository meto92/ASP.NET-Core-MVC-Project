using Metomarket.Common.Extensions;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.Products;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductService productService;

        public HomeController(IProductService productService)
        {
            this.productService = productService;
        }

        public IActionResult Index(
            string orderBy = nameof(ProductHomeViewModel.Type),
            bool ascending = true)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = this.productService
                    .All<ProductHomeViewModel>()
                    .Order(orderBy, ascending),
            };

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}