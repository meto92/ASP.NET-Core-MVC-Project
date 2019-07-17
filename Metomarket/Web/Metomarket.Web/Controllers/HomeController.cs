using Metomarket.Web.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = new ProductHomeViewModel[]
                {
                    new ProductHomeViewModel
                    {
                        Id = "1",
                        ImageUrl = "https://media.wired.com/photos/5b32da5e1027fe1d7ddd1249/191:100/pass/lgtvthing.jpg",
                        Price = 2222,
                        Name = "TV name",
                        Type = "TV",
                    },
                    new ProductHomeViewModel
                    {
                        Id = "2",
                        ImageUrl = "https://i5.walmartimages.ca/images/Enlarge/010/121/6000199010121.jpg",
                        Price = 2500.99m,
                        Name = "TV name",
                        Type = "TV",
                    },
                },
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