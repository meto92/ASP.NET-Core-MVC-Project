using Metomarket.Common;
using Metomarket.Web.ViewModels.ProductTypes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class ProductTypesController : MarketController
    {
        public IActionResult Index()
        {
            ProductTypesListViewModel model = new ProductTypesListViewModel
            {
                ProductTypes = new ProductTypeViewModel[]
                {
                    new ProductTypeViewModel
                    {
                        Name = "Type 1",
                        ProductsCount = 10,
                    },
                    new ProductTypeViewModel
                    {
                        Name = "Type 2",
                        ProductsCount = 20,
                    },
                    new ProductTypeViewModel
                    {
                        Name = "Type 3",
                        ProductsCount = 30,
                    },
                },
            };

            return this.View(model);
        }

        public IActionResult Create()
        {
            return this.View(new ProductTypeCreateInputModel());
        }

        [HttpPost]
        public IActionResult Create(ProductTypeCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}