using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Metomarket.Common;
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
    public class LoginWith2faModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string Slash = "~/";
        private const string SlashLockout = "./Lockout";
        private const string UnableToLoad2faUserMessage = "Unable to load two-factor authentication user.";
        private const string Space = " ";
        private const string Dash = "-";
        private const string UserLoggedInWith2faLogMessage = "User with ID '{UserId}' logged in with 2fa.";
        private const string AccountLockedOutMessage = "User with ID '{UserId}' account locked out.";
        private const string InvalidAuthenticatorCodeEnteredMessage = "Invalid authenticator code entered for user with ID '{UserId}'.";
        private const string InvalidAuthenticatorCodeMessage = "Invalid authenticator code.";

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LoginWith2faModel> logger;

        public LoginWith2faModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginWith2faModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(UnableToLoad2faUserMessage);
            }

            this.ReturnUrl = returnUrl;
            this.RememberMe = rememberMe;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            returnUrl = returnUrl ?? this.Url.Content(Slash);

            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(UnableToLoad2faUserMessage);
            }

            var authenticatorCode = this.Input.TwoFactorCode
                .Replace(Space, string.Empty)
                .Replace(Dash, string.Empty);

            var result = await this.signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, this.Input.RememberMachine);

            if (result.Succeeded)
            {
                this.logger.LogInformation(UserLoggedInWith2faLogMessage, user.Id);

                return this.LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                this.logger.LogWarning(AccountLockedOutMessage, user.Id);

                return this.RedirectToPage(SlashLockout);
            }
            else
            {
                this.logger.LogWarning(InvalidAuthenticatorCodeEnteredMessage, user.Id);
                this.ModelState.AddModelError(string.Empty, InvalidAuthenticatorCodeMessage);

                return this.Page();
            }
        }

        public class InputModel
        {
            private const int TwoFactorCodeMinLength = 6;
            private const int TwoFactorCodeMaxLength = 7;
            private const string TwoFactorCodeDisplayName = "Authenticator code";
            private const string RememberMachineDisplayName = "Remember this machine";
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

            [Required]
            [StringLength(TwoFactorCodeMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = TwoFactorCodeMinLength)]
            [DataType(DataType.Text)]
            [Display(Name = TwoFactorCodeDisplayName)]
            public string TwoFactorCode { get; set; }

            [Display(Name = RememberMachineDisplayName)]
            public bool RememberMachine { get; set; }
        }
    }
}