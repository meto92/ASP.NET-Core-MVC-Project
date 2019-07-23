using System;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metomarket.Web.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        private const string ViewDataActivePageKey = "ActivePage";
        private const string ActiveClassName = "active";

        public static string Index => nameof(Index);

        public static string ChangePassword => nameof(ChangePassword);

        public static string ExternalLogins => nameof(ExternalLogins);

        public static string PersonalData => nameof(PersonalData);

        public static string TwoFactorAuthentication => nameof(TwoFactorAuthentication);

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData[ViewDataActivePageKey] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? ActiveClassName : null;
        }
    }
}