using System.ComponentModel.DataAnnotations;
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
    public class ChangePasswordModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string SLashSetPassword = "./SetPassword";
        private const string YouCannotChangeYourPasswordMessage = "You cannot change your password.";
        private const string UserChangedPasswordSuccessfullyMessage = "User changed their password successfully.";
        private const string PasswordHasBeenChangedMessage = "Your password has been changed.";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ChangePasswordModel> logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

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

            var hasPassword = await this.userManager.HasPasswordAsync(user);

            if (!hasPassword)
            {
                return this.RedirectToPage(SLashSetPassword);
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            if (user.UserName == GlobalConstants.RootAdministratorUsername
                && user.Email == GlobalConstants.RootAdministratorEmail)
            {
                this.ModelState.AddModelError(
                    string.Empty,
                    YouCannotChangeYourPasswordMessage);

                return this.Page();
            }

            var changePasswordResult = await this.userManager.ChangePasswordAsync(user, this.Input.OldPassword, this.Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.Page();
            }

            await this.signInManager.RefreshSignInAsync(user);
            this.logger.LogInformation(UserChangedPasswordSuccessfullyMessage);
            this.StatusMessage = PasswordHasBeenChangedMessage;

            return this.RedirectToPage();
        }

        public class InputModel
        {
            private const string OldPasswordDisplayName = "Current password";
            private const int PasswordMinLength = GlobalConstants.PasswordMinLength;
            private const int PasswordMaxLength = GlobalConstants.PasswordMaxLength;
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessageFormat;
            private const string NewPasswordDisplayName = "New password";
            private const string ConfirmPasswordDisplayName = "Confirm new password";
            private const string ConfirmPasswordErrorMessage = "The new password and confirmation password do not match.";

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = OldPasswordDisplayName)]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(PasswordMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = PasswordMinLength)]
            [DataType(DataType.Password)]
            [Display(Name = NewPasswordDisplayName)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = ConfirmPasswordDisplayName)]
            [Compare(nameof(NewPassword), ErrorMessage = ConfirmPasswordErrorMessage)]
            public string ConfirmPassword { get; set; }
        }
    }
}