using System;
using System.Threading.Tasks;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
#pragma warning disable SA1649 // File name should match first type name
    public class Disable2faModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string CannotDisable2faMessage = "Cannot disable 2FA for user with ID '{0}' as it's not currently enabled.";
        private const string UnexpectedErrorDisabling2faMessage = "Unexpected error occurred disabling 2FA for user with ID '{0}'.";
        private const string UserHasDisabled2faLogMessage = "User with ID '{UserId}' has disabled 2fa.";
        private const string TwoFactorAuthenticationDisabledMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
        private const string SlashTwoFactorAuthentication = "./TwoFactorAuthentication";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<Disable2faModel> logger;

        public Disable2faModel(
            UserManager<ApplicationUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound(string.Format(
                    UnableToLoadUserMessage,
                    this.userManager.GetUserId(this.User)));
            }

            if (!await this.userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException(string.Format(
                    CannotDisable2faMessage,
                    this.userManager.GetUserId(this.User)));
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound(string.Format(
                    UnableToLoadUserMessage,
                    this.userManager.GetUserId(this.User)));
            }

            var disable2faResult = await this.userManager.SetTwoFactorEnabledAsync(user, false);

            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException(string.Format(
                    UnexpectedErrorDisabling2faMessage,
                    this.userManager.GetUserId(this.User)));
            }

            this.logger.LogInformation(
                UserHasDisabled2faLogMessage,
                this.userManager.GetUserId(this.User));
            this.StatusMessage = TwoFactorAuthenticationDisabledMessage;

            return this.RedirectToPage(SlashTwoFactorAuthentication);
        }
    }
}