using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
#pragma warning disable SA1649 // File name should match first type name
    public class EnableAuthenticatorModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string Space = " ";
        private const string Dash = "-";
        private const string InputDotCode = "Input.Code";
        private const string InvalidVerificationCodeMessage = "Verification code is invalid.";
        private const string UserHasEnabled2faLogMessage = "User with ID '{UserId}' has enabled 2FA with an authenticator app.";
        private const int NumberOfGenerated2faRecoveryCodes = 10;
        private const string AuthenticationAppVerifiedMessage = "Your authenticator app has been verified.";
        private const string SlashShowRecoveryCodes = "./ShowRecoveryCodes";
        private const string SlashTwoFactorAuthentication = "./TwoFactorAuthentication";
        private const string MetomarketDotWeb = "Metomarket.Web";
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<EnableAuthenticatorModel> logger;
        private readonly UrlEncoder urlEncoder;

        public EnableAuthenticatorModel(
            UserManager<ApplicationUser> userManager,
            ILogger<EnableAuthenticatorModel> logger,
            UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.urlEncoder = urlEncoder;
        }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound(string.Format(
                    UnableToLoadUserMessage,
                    this.userManager.GetUserId(this.User)));
            }

            await this.LoadSharedKeyAndQrCodeUriAsync(user);

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

            if (!this.ModelState.IsValid)
            {
                await this.LoadSharedKeyAndQrCodeUriAsync(user);

                return this.Page();
            }

            // Strip spaces and hypens
            var verificationCode = this.Input.Code
                .Replace(Space, string.Empty)
                .Replace(Dash, string.Empty);

            var is2faTokenValid = await this.userManager.VerifyTwoFactorTokenAsync(
                user, this.userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                this.ModelState.AddModelError(InputDotCode, InvalidVerificationCodeMessage);
                await this.LoadSharedKeyAndQrCodeUriAsync(user);

                return this.Page();
            }

            await this.userManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await this.userManager.GetUserIdAsync(user);
            this.logger.LogInformation(UserHasEnabled2faLogMessage, userId);

            this.StatusMessage = AuthenticationAppVerifiedMessage;

            if (await this.userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await this.userManager.GenerateNewTwoFactorRecoveryCodesAsync(
                    user,
                    NumberOfGenerated2faRecoveryCodes);
                this.RecoveryCodes = recoveryCodes.ToArray();

                return this.RedirectToPage(SlashShowRecoveryCodes);
            }
            else
            {
                return this.RedirectToPage(SlashTwoFactorAuthentication);
            }
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await this.userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(unformattedKey))
            {
                await this.userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await this.userManager.GetAuthenticatorKeyAsync(user);
            }

            this.SharedKey = this.FormatKey(unformattedKey);

            var email = await this.userManager.GetEmailAsync(user);
            this.AuthenticatorUri = this.GenerateQrCodeUri(email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            const int four = 4;
            int currentPosition = 0;
            var result = new StringBuilder();

            while (currentPosition + four < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, four)).Append(Space);
                currentPosition += four;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                this.urlEncoder.Encode(MetomarketDotWeb),
                this.urlEncoder.Encode(email),
                unformattedKey);
        }

        public class InputModel
        {
            private const int CodeMinLength = 6;
            private const int CodeMaxLength = 7;
            private const string CodeDisplayName = "Verification Code";
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

            [Required]
            [StringLength(CodeMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = CodeMinLength)]
            [DataType(DataType.Text)]
            [Display(Name = CodeDisplayName)]
            public string Code { get; set; }
        }
    }
}