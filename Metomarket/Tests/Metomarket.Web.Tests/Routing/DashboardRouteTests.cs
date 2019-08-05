using Metomarket.Common;
using Metomarket.Web.Areas.Administration.Controllers;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class DashboardRouteTests
    {
        [Fact]
        public void IndexShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Administration/Dashboard/Index")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<DashboardController>(c => c.Index());

        [Fact]
        public void SlashShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Administration/Dashboard/")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<DashboardController>(c => c.Index());
    }
}