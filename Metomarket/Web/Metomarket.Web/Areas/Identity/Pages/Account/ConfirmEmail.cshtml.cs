using System;
using System.Threading.Tasks;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ConfirmEmailModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string SlashIndex = "/Index";
        private const string UserNotFoundMessage = "Unable to load user with ID '{0}'.";
        private const string ErrorConfirmingEmailMessage = "Error confirming email for user with ID '{0}'.";

        private readonly UserManager<ApplicationUser> userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.RedirectToPage(SlashIndex);
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound(string.Format(UserNotFoundMessage, userId));
            }

            var result = await this.userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Format(
                    ErrorConfirmingEmailMessage,
                    userId));
            }

            return this.Page();
        }
    }
}