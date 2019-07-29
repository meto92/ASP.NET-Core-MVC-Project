using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Services.Data;
using Metomarket.Web.Controllers;
using Metomarket.Web.ViewModels.CreditCompanies;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.CreditCompanies.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("CreditCompanies")]
    [Route("{area}/{action}/{id?}")]
    public class CreditCompaniesController : BaseController
    {
        private readonly ICreditCompanyService creditCompanyService;

        public CreditCompaniesController(ICreditCompanyService creditCompanyService)
        {
            this.creditCompanyService = creditCompanyService;
        }

        public IActionResult Index()
        {
            CreditCompaniesListIndexViewModel model = new CreditCompaniesListIndexViewModel
            {
                CreditCompanies = this.creditCompanyService
                .All<CreditCompanyIndexViewModel>(),
            };

            return this.View(model);
        }

        public IActionResult Add()
        {
            return this.View(new CreditCompanyCreateInputModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreditCompanyCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.creditCompanyService.CreateAsync(
                model.Name,
                model.ActiveSincce);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}