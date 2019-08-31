using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ResetPasswordModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string CodeMustBeSuppliedMessage = "A code must be supplied for password reset.";
        private const string SlashResetPasswordConfirmation = "./ResetPasswordConfirmation";

        private readonly UserManager<ApplicationUser> userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return this.BadRequest(CodeMustBeSuppliedMessage);
            }
            else
            {
                this.Input = new InputModel
                {
                    Code = code,
                };

                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.userManager.FindByEmailAsync(this.Input.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToPage(SlashResetPasswordConfirmation);
            }

            var result = await this.userManager.ResetPasswordAsync(user, this.Input.Code, this.Input.Password);

            if (result.Succeeded)
            {
                return this.RedirectToPage(SlashResetPasswordConfirmation);
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.Page();
        }

        public class InputModel
        {
            private const int PasswordMinLength = GlobalConstants.PasswordMinLength;
            private const int PasswordMaxLength = GlobalConstants.PasswordMaxLength;
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;
            private const string ConfirmPasswordDisplayName = "Confirm password";
            private const string ConfirmPasswordErrorMessage = "The password and confirmation password do not match.";

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(PasswordMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = PasswordMinLength)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = ConfirmPasswordDisplayName)]
            [Compare(nameof(Password), ErrorMessage = ConfirmPasswordErrorMessage)]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }
    }
}