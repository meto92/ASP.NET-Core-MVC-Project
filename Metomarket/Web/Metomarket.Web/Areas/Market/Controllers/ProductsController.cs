using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.Orders;
using Metomarket.Web.ViewModels.Products;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    public class ProductsController : MarketController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Create()
        {
            return this.View(new ProductCreateInputModel());
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.productService.CreateAsync(model.Name, model.Price, model.ImageUrl, model.InStock, model.TypeId);

            return this.Redirect("/");
        }

        public IActionResult Details(string id)
        {
            bool exists = this.productService.Exists(id);

            if (!exists)
            {
                return this.RedirectToHome();
            }

            ProductDetailsViewModel model = this.productService
                .FindById<ProductDetailsViewModel>(id);

            return this.View(model);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Edit(string id)
        {
            bool exists = this.productService.Exists(id);

            if (!exists)
            {
                return this.RedirectToHome();
            }

            ProductEditModel model = this.productService.FindById<ProductEditModel>(id);

            return this.View(model);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(ProductEditModel model)
        {
            return this.Content($"{model.Name}, {model.Price}, {model.ImageUrl}, {model.QuantityToAdd}");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Delete(string id)
        {
            return this.Content(id);
        }

        [Authorize]
        [HttpPost]
        public IActionResult InitializeOrder(string productId)
        {
            return this.RedirectToAction(nameof(this.CreateOrder), new { id = productId });
        }

        [Authorize]
        public IActionResult CreateOrder(string id)
        {
            return this.View(new ProductCreateOrderViewModel
            {
                Id = "2",
                Price = 2500.99m,
                Name = "TV name",
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderCreateInputModel model)
        {
            return this.Content($"{model.ProductId}, {model.Quantity}");
        }
    }
}