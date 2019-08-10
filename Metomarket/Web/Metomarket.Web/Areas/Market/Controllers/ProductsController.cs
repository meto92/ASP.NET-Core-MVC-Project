using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.Orders;
using Metomarket.Web.ViewModels.Products;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    public class ProductsController : MarketController
    {
        private readonly IProductService productService;
        private readonly IOrderService orderService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductsController(
            IProductService productService,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            UserManager<ApplicationUser> userManager)
        {
            this.productService = productService;
            this.orderService = orderService;
            this.shoppingCartService = shoppingCartService;
            this.userManager = userManager;
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

            string id = await this.productService.CreateAsync(
                model.Name,
                model.Price,
                model.ImageUrl,
                model.InStock,
                model.TypeId);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public IActionResult Details(string id)
        {
            ProductDetailsViewModel model = this.productService
                .FindById<ProductDetailsViewModel>(id);

            return this.View(model);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Edit(string id)
        {
            ProductEditModel model = this.productService.FindById<ProductEditModel>(id);

            return this.View(model);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Edit), new { id = model.Id });
            }

            await this.productService.UpdateAsync(
                model.Id,
                model.Name,
                model.Price,
                model.ImageUrl,
                model.QuantityToAdd);

            return this.RedirectToAction(nameof(this.Details), new { id = model.Id });
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            await this.productService.DeleteAsync(id);

            return this.RedirectToHome();
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
            ProductCreateOrderViewModel model = this.productService.FindById<ProductCreateOrderViewModel>(id);

            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(
                    nameof(this.CreateOrder),
                    new { id = model.ProductId });
            }

            string userId = this.userManager.GetUserId(this.User);
            string orderId = await this.orderService.CreateAsync(model.ProductId, userId, model.Quantity);

            await this.shoppingCartService.AddOrderAsync(userId, orderId);

            return this.RedirectToHome();
        }
    }
}