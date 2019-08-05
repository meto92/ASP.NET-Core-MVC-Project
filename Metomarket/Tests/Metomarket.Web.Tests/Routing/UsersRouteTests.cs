using Metomarket.Common;
using Metomarket.Web.Areas.Administration.Controllers;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class UsersRouteTests
    {
        [Fact]
        public void IndexShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Administration/Users/Index")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<UsersController>(c => c.Index());

        [Fact]
        public void SlashShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Administration/Users/")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<UsersController>(c => c.Index());

        [Fact]
        public void PromoteShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Administration/Users/Promote/1")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<UsersController>(c => c.Promote("1"));

        [Fact]
        public void DemoteShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Administration/Users/Demote/1")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<UsersController>(c => c.Demote("1"));
    }
}