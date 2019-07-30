using System.Collections.Generic;

using Metomarket.Services.Data;
using Metomarket.Web.Infrastructure.ComponentViewModels.ProductTypes;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Infrastructure.Components
{
    [ViewComponent(Name = "ProductTypeLinks")]
    public class ProductTypeLinksViewComponent : ViewComponent
    {
        private readonly IProductTypeService productTypeService;

        public ProductTypeLinksViewComponent(IProductTypeService productTypeService)
        {
            this.productTypeService = productTypeService;
        }

        public IViewComponentResult Invoke()
        {
            IEnumerable<ProductTypeLinkViewModel> model = this.productTypeService
                .All<ProductTypeLinkViewModel>();

            return this.View(model);
        }
    }
}