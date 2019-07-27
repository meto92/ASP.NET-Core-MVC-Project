using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Controllers
{
    public class BaseController : Controller
    {
        private const string HomePageUrl = "/";

        public IActionResult RedirectToHome()
            => this.Redirect(HomePageUrl);
    }
}