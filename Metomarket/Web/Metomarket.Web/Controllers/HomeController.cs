using System;

using Metomarket.Common;
using Metomarket.Common.Extensions;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.Products;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Metomarket.Web.Controllers
{
    public class HomeController : BaseController
    {
        private const string CacheKey = nameof(ProductsListViewModel);

        private readonly IProductService productService;
        private readonly IMemoryCache memoryCache;

        public HomeController(IProductService productService, IMemoryCache memoryCache)
        {
            this.productService = productService;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index(
            string orderBy = nameof(ProductHomeViewModel.Type),
            bool ascending = true)
        {
            if (!this.memoryCache.TryGetValue(CacheKey, out ProductsListViewModel model))
            {
                model = new ProductsListViewModel
                {
                    Products = this.productService
                    .All<ProductHomeViewModel>(),
                };

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(GlobalConstants.CacheExpirationMinutes));

                this.memoryCache.Set(CacheKey, model, cacheEntryOptions);
            }

            model.Products = model.Products.Order(orderBy, ascending);

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