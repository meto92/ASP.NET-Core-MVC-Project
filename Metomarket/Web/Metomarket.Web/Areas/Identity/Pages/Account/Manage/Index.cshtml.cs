using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
#pragma warning disable SA1649 // File name should match first type name
    public class IndexModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string ProfileUpdatedMessage = "Your profile has been updated";
        private const string SlashAccountSlashConfirmEmail = "/Account/ConfirmEmail";
        private const string ConfirmEmailMessage = "Confirm your email";
        private const string ConfirmEmailHtmlMessage = "Please confirm your account by <a href='{0}'>clicking here</a>.";
        private const string VerificationEmailSentMessage = "Verification email sent. Please check your email.";
        private const string UnexpectedErrorSettingUsernaneMessage = "Unexpected error occurred setting username for user with ID '{0}'.";
        private const string UnexpectedErrorSettingPhoneNumberMessage = "Unexpected error occurred setting phone number for user with ID '{0}'.";
        private const string YouCannotChangeYourUsernameMessage = "You cannot change your username.";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

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

            var userName = await this.userManager.GetUserNameAsync(user);
            var email = await this.userManager.GetEmailAsync(user);
            var phoneNumber = await this.userManager.GetPhoneNumberAsync(user);

            this.Email = email;

            this.Input = new InputModel
            {
                Username = userName,
                PhoneNumber = phoneNumber,
            };

            this.IsEmailConfirmed = await this.userManager.IsEmailConfirmedAsync(user);

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return await this.OnGetAsync();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound(string.Format(
                    UnableToLoadUserMessage,
                    this.userManager.GetUserId(this.User)));
            }

            var username = await this.userManager.GetUserNameAsync(user);

            if (this.Input.Username != username)
            {
                await this.TryChangeUsernameAsync(user);
            }

            var phoneNumber = await this.userManager.GetPhoneNumberAsync(user);

            if (this.Input.PhoneNumber != phoneNumber)
            {
                await this.TryChangePhoneNumberAsync(user);
            }

            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = ProfileUpdatedMessage;

            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                return this.NotFound(string.Format(
                    UnableToLoadUserMessage,
                    this.userManager.GetUserId(this.User)));
            }

            var userId = await this.userManager.GetUserIdAsync(user);
            var email = await this.userManager.GetEmailAsync(user);
            var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = this.Url.Page(
                SlashAccountSlashConfirmEmail,
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: this.Request.Scheme);

            await this.emailSender.SendEmailAsync(
                email,
                ConfirmEmailMessage,
                string.Format(
                    ConfirmEmailHtmlMessage,
                    HtmlEncoder.Default.Encode(callbackUrl)));

            this.StatusMessage = VerificationEmailSentMessage;

            return this.RedirectToPage();
        }

        private async Task TryChangeUsernameAsync(ApplicationUser user)
        {
            if (user.Email == GlobalConstants.RootAdministratorEmail
                && user.UserName == GlobalConstants.RootAdministratorUsername)
            {
                this.ModelState.AddModelError(
                    string.Empty,
                    YouCannotChangeYourUsernameMessage);

                return;
            }

            var setUsernameResult = await this.userManager.SetUserNameAsync(user, this.Input.Username);

            if (!setUsernameResult.Succeeded)
            {
                var userId = await this.userManager.GetUserIdAsync(user);

                throw new InvalidOperationException(string.Format(
                    UnexpectedErrorSettingUsernaneMessage,
                    userId));
            }
        }

        private async Task TryChangePhoneNumberAsync(ApplicationUser user)
        {
            var setPhoneResult = await this.userManager.SetPhoneNumberAsync(user, this.Input.PhoneNumber);

            if (!setPhoneResult.Succeeded)
            {
                var userId = await this.userManager.GetUserIdAsync(user);

                throw new InvalidOperationException(string.Format(
                    UnexpectedErrorSettingPhoneNumberMessage,
                    userId));
            }
        }

        public class InputModel
        {
            private const int UsernameMinLength = GlobalConstants.UsernameMinLength;
            private const int UsernameMaxLength = GlobalConstants.UsernameMaxLength;
            private const string PhoneNumberDisplayName = "Phone number";
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

            [Required]
            [StringLength(UsernameMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = UsernameMinLength)]
            public string Username { get; set; }

            [Phone]
            [Display(Name = PhoneNumberDisplayName)]
            public string PhoneNumber { get; set; }
        }
    }
}