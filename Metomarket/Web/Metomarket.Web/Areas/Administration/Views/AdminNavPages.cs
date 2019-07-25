using System;
using System.IO;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metomarket.Web.Areas.Administration.Views
{
    public static class AdminNavPages
    {
        private const string ViewDataActivePageKey = "ActivePage";
        private const string ActiveClassName = "active";

        public static string Users => nameof(Users);

        public static string Dashboard => nameof(Dashboard);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string DashboardNavClass(ViewContext viewContext) => PageNavClass(viewContext, Dashboard);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData[ViewDataActivePageKey] as string
                ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase)
                ? ActiveClassName
                : null;
        }
    }
}