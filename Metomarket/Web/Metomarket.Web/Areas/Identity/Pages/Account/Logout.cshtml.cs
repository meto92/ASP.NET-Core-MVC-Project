﻿using System.Threading.Tasks;

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
    public class LogoutModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UserLoggedOutMessage = "User logged out.";

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LogoutModel> logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            await this.signInManager.SignOutAsync();
            this.logger.LogInformation(UserLoggedOutMessage);

            if (returnUrl != null)
            {
                return this.LocalRedirect(returnUrl);
            }

            return this.Page();
        }
    }
}