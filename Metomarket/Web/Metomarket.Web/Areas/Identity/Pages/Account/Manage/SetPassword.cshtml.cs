using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
#pragma warning disable SA1649 // File name should match first type name
    public class SetPasswordModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string SlashChangePassword = "./ChangePassword";
        private const string PasswordSetMessage = "Your password has been set.";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public SetPasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

            if (hasPassword)
            {
                return this.RedirectToPage(SlashChangePassword);
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

            var addPasswordResult = await this.userManager.AddPasswordAsync(user, this.Input.NewPassword);

            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.Page();
            }

            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = PasswordSetMessage;

            return this.RedirectToPage();
        }

        public class InputModel
        {
            private const int PasswordMinLength = GlobalConstants.PasswordMinLength;
            private const int PasswordMaxLength = GlobalConstants.PasswordMaxLength;
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;
            private const string NewPasswordDisplayName = "New password";
            private const string ConfirmPasswordDisplayName = "Confirm new password";
            private const string ConfirmPasswordErrorMEssage = "The new password and confirmation password do not match.";

            [Required]
            [StringLength(PasswordMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = PasswordMinLength)]
            [DataType(DataType.Password)]
            [Display(Name = NewPasswordDisplayName)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = ConfirmPasswordDisplayName)]
            [Compare(nameof(NewPassword), ErrorMessage = ConfirmPasswordErrorMEssage)]
            public string ConfirmPassword { get; set; }
        }
    }
}