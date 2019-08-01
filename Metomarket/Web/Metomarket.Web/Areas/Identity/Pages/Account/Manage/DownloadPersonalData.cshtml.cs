using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
#pragma warning disable SA1649 // File name should match first type name
    public class DownloadPersonalDataModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private const string UnableToLoadUserMessage = "Unable to load user with ID '{0}'.";
        private const string UserAskedForPersonalDataLogMessage = "User with ID '{UserId}' asked for their personal data.";
        private const string NullStr = "null";
        private const string ContentDispositionHeaderKey = "Content-Disposition";
        private const string ContentDispositionHeaderValue = "attachment; filename=PersonalData.json";
        private const string FileContentType = "text/json";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<DownloadPersonalDataModel> logger;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
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

            this.logger.LogInformation(
                UserAskedForPersonalDataLogMessage,
                this.userManager.GetUserId(this.User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? NullStr);
            }

            this.Response.Headers.Add(
                ContentDispositionHeaderKey,
                ContentDispositionHeaderValue);

            return new FileContentResult(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)),
                FileContentType);
        }
    }
}