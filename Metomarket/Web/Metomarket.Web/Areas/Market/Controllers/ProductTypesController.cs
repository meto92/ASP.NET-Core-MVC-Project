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
            return this.View();
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