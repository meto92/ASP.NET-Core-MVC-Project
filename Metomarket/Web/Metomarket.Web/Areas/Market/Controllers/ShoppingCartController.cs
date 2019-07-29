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
        private readonly UserManager<ApplicationUser> userManager;

        public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            string userId = this.userManager.GetUserId(this.User);

            ShoppingCartViewModel model = this.shoppingCartService
                .FindByUserId<ShoppingCartViewModel>(userId);

            return this.View(model);
        }

        public async Task<IActionResult> DeleteOrder(string id)
        {
            string userId = this.userManager.GetUserId(this.User);

            await this.orderService.DeleteAsync(id, userId);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult CompleteOrders()
        {
            CompleteOrdersModel model = new CompleteOrdersModel
            {
                Total = 1234.567m,
                OrdersCount = 4,
            };

            if (model.OrdersCount == 0)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(model);
        }

        [HttpPost]
        public IActionResult CompleteOrders(CompleteOrdersModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.CompleteOrders));
            }

            return this.Content($"id: {model.CreditCompanyId}, card number:{model.CreditCardNumber}, period: {model.PeriodInMonths}");
        }

        [HttpPost]
        public IActionResult EmptyCart()
        {
            throw new System.Exception();
        }
    }
}