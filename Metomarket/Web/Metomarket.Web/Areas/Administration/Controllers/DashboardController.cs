using Metomarket.Web.Areas.Administration.ViewModels.Dashboard;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Administration.Controllers
{
    public class DashboardController : AdministrationController
    {
        public DashboardController()
        {
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = 0, };

            return this.View(viewModel);
        }
    }
}