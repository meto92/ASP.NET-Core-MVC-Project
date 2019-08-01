using System;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
#pragma warning disable SA1649 // File name should match first type name
    public class GenerateRecoveryCodesModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string CannotGenerateRecoveryCodeMessage = "Cannot generate recovery codes for user with ID '{0}' because they do not have 2FA enabled.";
        private const int NumberOfGenerated2faRecoveryCodes = 10;
        private const string UserHasGeneratedNew2faRecoveryCodesLogMessage = "User with ID '{UserId}' has generated new 2FA recovery codes.";
        private const string YouHaveGEneratedNewRecoveryCodeMessage = "You have generated new recovery codes.";
        private const string SlashShowRecoveryCodes = "./ShowRecoveryCodes";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<GenerateRecoveryCodesModel> logger;

        public GenerateRecoveryCodesModel(
            UserManager<ApplicationUser> userManager,
            ILogger<GenerateRecoveryCodesModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound(string.Format(
                    UnableToLoadUserMessage,
                    this.userManager.GetUserId(this.User)));
            }

            var isTwoFactorEnabled = await this.userManager.GetTwoFactorEnabledAsync(user);

            if (!isTwoFactorEnabled)
            {
                var userId = await this.userManager.GetUserIdAsync(user);

                throw new InvalidOperationException(string.Format(
                    CannotGenerateRecoveryCodeMessage,
                    userId));
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

            var isTwoFactorEnabled = await this.userManager.GetTwoFactorEnabledAsync(user);
            var userId = await this.userManager.GetUserIdAsync(user);

            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException(string.Format(
                    CannotGenerateRecoveryCodeMessage,
                    userId));
            }

            var recoveryCodes = await this.userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, NumberOfGenerated2faRecoveryCodes);
            this.RecoveryCodes = recoveryCodes.ToArray();

            this.logger.LogInformation(
                UserHasGeneratedNew2faRecoveryCodesLogMessage,
                userId);
            this.StatusMessage = YouHaveGEneratedNewRecoveryCodeMessage;

            return this.RedirectToPage(SlashShowRecoveryCodes);
        }
    }
}