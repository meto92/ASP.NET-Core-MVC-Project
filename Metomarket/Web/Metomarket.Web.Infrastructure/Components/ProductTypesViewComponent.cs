using System.Collections.Generic;

using Metomarket.Services.Data;
using Metomarket.Web.Infrastructure.ComponentViewModels.ProductTypes;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Infrastructure.Components
{
    [ViewComponent(Name = "ProductTypeSelectOptions")]
    public class ProductTypesViewComponent : ViewComponent
    {
        private readonly IProductTypeService productTypeService;

        public ProductTypesViewComponent(IProductTypeService productTypeService)
        {
            this.productTypeService = productTypeService;
        }

        public IViewComponentResult Invoke()
        {
            IEnumerable<ProductTypeOptionViewModel> model = this.productTypeService.All<ProductTypeOptionViewModel>();

            return this.View(model);
        }
    }
}