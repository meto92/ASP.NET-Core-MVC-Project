using System.Collections.Generic;

using Metomarket.Services.Data;
using Metomarket.Web.Infrastructure.ComponentViewModels.CreditCompanies;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Infrastructure.Components
{
    [ViewComponent(Name = "CreditCompanySelectOptions")]
    public class CreditCompaniesViewComponent : ViewComponent
    {
        private readonly ICreditCompanyService creditCompanyService;

        public CreditCompaniesViewComponent(ICreditCompanyService creditCompanyService)
        {
            this.creditCompanyService = creditCompanyService;
        }

        public IViewComponentResult Invoke()
        {
            IEnumerable<CreditCompanyOptionViewModel> model = this.creditCompanyService.All<CreditCompanyOptionViewModel>();

            return this.View(model);
        }
    }
}