using Metomarket.Services.Data;
using Metomarket.Web.Areas.Administration.ViewModels.Dashboard;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Administration.Controllers
{
    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
            return this.View(viewModel);
        }
    }
}