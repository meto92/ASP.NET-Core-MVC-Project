using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class RegisterModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string Slash = "~/";
        private const string UserCreatedNewAccountLogMessage = "User created a new account with password.";
        private const string SlashAccountSlashConfirmEmail = "/Account/ConfirmEmail";
        private const string ConfirmYourEmailMessage = "Confirm your email";
        private const string ConfirmAccountHtmlMessage = "Please confirm your account by <a href='{0}'>clicking here</a>.";

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IHubContext<DashboardHub> dashboardHubContext;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IHubContext<DashboardHub> dashboardHubContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.dashboardHubContext = dashboardHubContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content(Slash);

            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = this.Input.Username,
                    Email = this.Input.Email,
                    FirstName = this.Input.FirstName,
                    LastName = this.Input.LastName,
                    Address = this.Input.Address,
                    PhoneNumber = this.Input.PhoneNumber,
                };

                var result = await this.userManager.CreateAsync(user, this.Input.Password);

                if (result.Succeeded)
                {
                    this.logger.LogInformation(UserCreatedNewAccountLogMessage);

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        SlashAccountSlashConfirmEmail,
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        ConfirmYourEmailMessage,
                        string.Format(
                            ConfirmAccountHtmlMessage,
                            HtmlEncoder.Default.Encode(callbackUrl)));

                    await this.signInManager.SignInAsync(user, isPersistent: false);

                    await this.dashboardHubContext.Clients.All
                        .SendAsync(DashboardHub.UserRegisteredMethodName);

                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        public class InputModel
        {
            private const int UsernameMinLength = GlobalConstants.UsernameMinLength;
            private const int UsernameMaxLength = GlobalConstants.UsernameMaxLength;
            private const int AddressMinLength = GlobalConstants.UserAddressMinLength;
            private const int AddressMaxLength = GlobalConstants.UserAddressMaxLength;
            private const int FirstNameMaxLength = GlobalConstants.UserFirstNameMaxLength;
            private const string FirstNameDisplayName = "First name";
            private const int LastNameMaxLength = GlobalConstants.UserLastNameMaxLength;
            private const string LastNameDisplayName = "Last name";
            private const int PasswordMinLength = GlobalConstants.PasswordMinLength;
            private const int PasswordMaxLength = GlobalConstants.PasswordMaxLength;
            private const string ConfirmPasswordDisplayName = "Confirm password";
            private const string PasswordsDoNotMatchMessage = "The password and confirmation password do not match.";
            private const string StringLengthErrorMessage = GlobalConstants.StringLengthErrorMessage;

            [Required]
            [StringLength(UsernameMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = UsernameMinLength)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(AddressMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = AddressMinLength)]
            public string Address { get; set; }

            [StringLength(FirstNameMaxLength)]
            [Display(Name = FirstNameDisplayName)]
            public string FirstName { get; set; }

            [StringLength(LastNameMaxLength)]
            [Display(Name = LastNameDisplayName)]
            public string LastName { get; set; }

            [Phone]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(PasswordMaxLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = PasswordMinLength)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = ConfirmPasswordDisplayName)]
            [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatchMessage)]
            public string ConfirmPassword { get; set; }
        }
    }
}