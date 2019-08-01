using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class LoginWithRecoveryCodeModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string Space = " ";
        private const string Slash = "~/";
        private const string UnableToLoad2faUserMessage = "Unable to load two-factor authentication user.";
        private const string UserLoggedInWithRecoveryCodeLogMessage = "User with ID '{UserId}' logged in with a recovery code.";
        private const string UserAccountLockedOutLogMessage = "User with ID '{UserId}' account locked out.";
        private const string SlashLockout = "./Lockout";
        private const string InvalidRecoveryCodeEnteredLogMessage = "Invalid recovery code entered for user with ID '{UserId}' ";
        private const string InvalidRecoveryCodeEnteredMessage = "Invalid recovery code entered.";

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> logger;

        public LoginWithRecoveryCodeModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginWithRecoveryCodeModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(UnableToLoad2faUserMessage);
            }

            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(UnableToLoad2faUserMessage);
            }

            var recoveryCode = this.Input.RecoveryCode.Replace(Space, string.Empty);

            var result = await this.signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                this.logger.LogInformation(UserLoggedInWithRecoveryCodeLogMessage, user.Id);
                return this.LocalRedirect(returnUrl ?? this.Url.Content(Slash));
            }

            if (result.IsLockedOut)
            {
                this.logger.LogWarning(UserAccountLockedOutLogMessage, user.Id);

                return this.RedirectToPage(SlashLockout);
            }
            else
            {
                this.logger.LogWarning(InvalidRecoveryCodeEnteredLogMessage, user.Id);
                this.ModelState.AddModelError(string.Empty, InvalidRecoveryCodeEnteredMessage);

                return this.Page();
            }
        }

        public class InputModel
        {
            private const string RecoveryCodeDisplayName = "Recovery Code";

            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = RecoveryCodeDisplayName)]
            public string RecoveryCode { get; set; }
        }
    }
}