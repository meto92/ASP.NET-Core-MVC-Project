using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Models;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.ShoppingCarts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    [Authorize]
    public class ShoppingCartController : MarketController
    {
        private readonly IShoppingCartService shoppingCartService;
        private readonly IOrderService orderService;
        private readonly IContractService contractService;
        private readonly UserManager<ApplicationUser> userManager;

        public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            IContractService contractService,
            UserManager<ApplicationUser> userManager)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
            this.contractService = contractService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            string userId = this.GetUserId();

            ShoppingCartViewModel model = this.shoppingCartService
                .FindByUserId<ShoppingCartViewModel>(userId);

            return this.View(model);
        }

        public async Task<IActionResult> DeleteOrder(string id)
        {
            string userId = this.GetUserId();

            await this.orderService.DeleteAsync(id, userId);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult CompleteOrders()
        {
            string userId = this.GetUserId();

            ShoppingCartViewModel shoppingCartData = this.shoppingCartService
                .FindByUserId<ShoppingCartViewModel>(userId);

            CompleteOrdersModel model = new CompleteOrdersModel
            {
                Total = shoppingCartData.Total,
                OrdersCount = shoppingCartData.Orders.Count(),
            };

            if (model.OrdersCount == 0)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrders(CompleteOrdersModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.CompleteOrders));
            }

            string userId = this.GetUserId();

            ShoppingCartViewModel shoppingCartData = this.shoppingCartService
                .FindByUserId<ShoppingCartViewModel>(userId);

            IEnumerable<string> orderIds = shoppingCartData.Orders
                .Select(order => order.Id);

            await this.contractService.CreateAsync(
                userId,
                model.CreditCompanyId,
                orderIds,
                shoppingCartData.Total,
                model.CreditCardNumber,
                model.PeriodInMonths);

            await this.orderService.CompleteOrdersAsync(orderIds);
            await this.shoppingCartService.EmptyCartAsync(userId);

            return this.RedirectToHome();
        }

        [HttpPost]
        public async Task<IActionResult> EmptyCart()
        {
            string userId = this.GetUserId();

            await this.shoppingCartService.EmptyCartAsync(userId);

            return this.RedirectToAction(nameof(this.Index));
        }

        private string GetUserId()
        {
            string userId = this.userManager.GetUserId(this.User);

            return userId;
        }
    }
}