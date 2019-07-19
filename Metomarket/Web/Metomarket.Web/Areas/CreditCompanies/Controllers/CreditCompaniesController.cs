using Metomarket.Common;
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
        public IActionResult Create()
        {
            return this.View(new CreditCompanyCreateInputModel());
        }

        [HttpPost]
        public IActionResult Create(CreditCompanyCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            return this.View();
        }
    }
}