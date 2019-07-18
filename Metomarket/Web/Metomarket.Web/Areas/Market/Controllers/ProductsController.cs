using Metomarket.Common;
using Metomarket.Web.Controllers;
using Metomarket.Web.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    public class ProductsController : MarketController
    {
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Create()
        {
            return this.View(new ProductCreateInputModel());
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public IActionResult Create(ProductCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            return this.Redirect("/");
        }

        public IActionResult Details(string id)
        {
            ProductDetailsViewModel model = new ProductDetailsViewModel
            {
                Id = "2",
                ImageUrl = "https://i5.walmartimages.ca/images/Enlarge/010/121/6000199010121.jpg",
                Price = 2500.99m,
                Name = "TV name",
                Type = "TV",
                InStock = 0,
            };

            return this.View(model);
        }
    }
}