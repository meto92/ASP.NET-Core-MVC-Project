﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Metomarket.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class LockoutModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        public void OnGet()
        {
        }
    }
}