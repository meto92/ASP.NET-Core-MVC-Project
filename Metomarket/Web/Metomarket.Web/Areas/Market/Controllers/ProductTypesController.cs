using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.ProductTypes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Market.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class ProductTypesController : MarketController
    {
        private readonly IProductTypeService productTypeService;

        public ProductTypesController(IProductTypeService productTypeService)
        {
            this.productTypeService = productTypeService;
        }

        public IActionResult Index()
        {
            ProductTypesListViewModel model = new ProductTypesListViewModel
            {
                ProductTypes = this.productTypeService.All<ProductTypeViewModel>(),
            };

            return this.View(model);
        }

        public IActionResult Create()
        {
            return this.View(new ProductTypeCreateInputModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductTypeCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.productTypeService.CreateAsync(model.Name);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}