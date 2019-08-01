using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ForgotPasswordModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string SlashForgotPasswordConfirmation = "./ForgotPasswordConfirmation";
        private const string SlashAccountSlashResetPassword = "/Account/ResetPassword";
        private const string ResetPasswordMessage = "Reset Password";
        private const string ResetPasswordHtmlMessage = "Please reset your password by <a href='{0}'>clicking here</a>.";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(this.Input.Email);

                if (user == null || !(await this.userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed

                    return this.RedirectToPage(SlashForgotPasswordConfirmation);
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = this.Url.Page(
                    SlashAccountSlashResetPassword,
                    pageHandler: null,
                    values: new { code },
                    protocol: this.Request.Scheme);

                await this.emailSender.SendEmailAsync(
                    this.Input.Email,
                    ResetPasswordMessage,
                    string.Format(
                        ResetPasswordHtmlMessage,
                        HtmlEncoder.Default.Encode(callbackUrl)));

                return this.RedirectToPage(SlashForgotPasswordConfirmation);
            }

            return this.Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}