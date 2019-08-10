using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.Dashboard;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Administration.Controllers
{
    public class DashboardController : AdministrationController
    {
        private readonly IContractService contractService;
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly IProductTypeService productTypeService;
        private readonly IUserService userService;

        public DashboardController(
            IContractService contractService,
            IOrderService orderService,
            IProductService productService,
            IProductTypeService productTypeService,
            IUserService userService)
        {
            this.contractService = contractService;
            this.orderService = orderService;
            this.productService = productService;
            this.productTypeService = productTypeService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            DashboardIndexViewModel model = new DashboardIndexViewModel
            {
                ContractsCount = this.contractService.GetCount(),
                OrdersCount = this.orderService.GetCount(),
                ProductsCount = this.productService.GetCount(),
                ProductTypesCount = this.productTypeService.GetCount(),
                UsersCount = this.userService.GetCount(),
            };

            return this.View(model);
        }
    }
}