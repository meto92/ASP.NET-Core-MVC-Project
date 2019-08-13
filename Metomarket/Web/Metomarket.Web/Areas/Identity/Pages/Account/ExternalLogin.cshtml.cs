using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ExternalLoginModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string Slash = "~/";
        private const string SlashLogin = "./Login";
        private const string SlashLockout = "./Lockout";
        private const string SlashExternalLogin = "./ExternalLogin";
        private const string Callback = "Callback";
        private const string ErrorFromExternalProviderMessage = "Error from external provider: {0}";
        private const string ErrorLoadingExternalLoginInfoMessage = "Error loading external login information.";
        private const string SuccessfulLoginLogMessage = "{Name} logged in with {LoginProvider} provider.";
        private const string ErrorLoadingExternalLoginInfoDuringConfirmationMessage = "Error loading external login information during confirmation.";
        private const string UserCreatedAccountLogMessage = "User created an account using {Name} provider.";

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ExternalLoginModel> logger;
        private readonly IHubContext<DashboardHub> dashboardHubContext;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IHubContext<DashboardHub> dashboardHubContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.dashboardHubContext = dashboardHubContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGetAsync()
        {
            return this.RedirectToPage(SlashLogin);
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Page(SlashExternalLogin, pageHandler: Callback, values: new { returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? this.Url.Content(Slash);

            if (remoteError != null)
            {
                this.ErrorMessage = string.Format(
                    ErrorFromExternalProviderMessage,
                    remoteError);

                return this.RedirectToPage(SlashLogin, new { ReturnUrl = returnUrl });
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                this.ErrorMessage = ErrorLoadingExternalLoginInfoMessage;

                return this.RedirectToPage(SlashLogin, new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                this.logger.LogInformation(
                    SuccessfulLoginLogMessage,
                    info.Principal.Identity.Name,
                    info.LoginProvider);

                return this.LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return this.RedirectToPage(SlashLockout);
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                this.ReturnUrl = returnUrl;
                this.LoginProvider = info.LoginProvider;

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    this.Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    };
                }

                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content(Slash);

            // Get the information about the user from the external login provider
            var info = await this.signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                this.ErrorMessage = ErrorLoadingExternalLoginInfoDuringConfirmationMessage;

                return this.RedirectToPage(SlashLogin, new { ReturnUrl = returnUrl });
            }

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

                var result = await this.userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await this.userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        this.logger.LogInformation(
                            UserCreatedAccountLogMessage,
                            info.LoginProvider);

                        await this.dashboardHubContext.Clients.All
                            .SendAsync(DashboardHub.UserRegisteredMethodName);

                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            this.LoginProvider = info.LoginProvider;
            this.ReturnUrl = returnUrl;

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

            [Required]
            [StringLength(UsernameMaxLength, ErrorMessage = GlobalConstants.StringLengthErrorMessageFormat, MinimumLength = UsernameMinLength)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(AddressMaxLength, ErrorMessage = GlobalConstants.StringLengthErrorMessageFormat, MinimumLength = AddressMinLength)]
            public string Address { get; set; }

            [StringLength(FirstNameMaxLength)]
            [Display(Name = FirstNameDisplayName)]
            public string FirstName { get; set; }

            [StringLength(LastNameMaxLength)]
            [Display(Name = LastNameDisplayName)]
            public string LastName { get; set; }

            [Phone]
            public string PhoneNumber { get; set; }
        }
    }
}