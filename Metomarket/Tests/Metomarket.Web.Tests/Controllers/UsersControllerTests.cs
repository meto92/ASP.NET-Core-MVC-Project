using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Areas.Administration.Controllers;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Controllers
{
    public class UsersControllerTests
    {
        private const string User1Id = nameof(User1Id);
        private const string User2Id = nameof(User2Id);
        private const string AdminRoleId = nameof(AdminRoleId);
        private const string AdminRoleName = GlobalConstants.AdministratorRoleName;

        [Fact]
        public void UsersControllerShouldInheritAdministrationControllerWhichShouldBeOnlyForAdmins()
        {
            Assert.True(typeof(AdministrationController).IsAssignableFrom(typeof(UsersController)));

            MyMvc
                .Controller<AdministrationController>()
                .ShouldHave()
                .Attributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(AdminRoleName));
        }

        [Fact]
        public void IndexShouldReturnView()
            => MyMvc
                .Controller<UsersController>()
                .Calling(c => c.Index())
                .ShouldReturn()
                .View();

        [Fact]
        public void PromoteShouldRedirect()
            => MyMvc
                .Controller<UsersController>()
                .WithData(db => db
                    .WithSet<ApplicationRole>(set => set
                        .Add(this.CreateAdminRole()))
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = User1Id })))
                .Calling(c => c.Promote(User1Id))
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(UsersController.Index)));

        [Fact]
        public void DemoteShouldRedirect()
            => MyMvc
                .Controller<UsersController>()
                .WithData(db => db
                    .WithSet<ApplicationRole>(set => set
                        .Add(this.CreateAdminRole()))
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = User2Id })))
                .Calling(c => c.Demote(User2Id))
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(UsersController.Index)));

        private ApplicationRole CreateAdminRole()
        {
            ApplicationRole adminRole = new ApplicationRole
            {
                Id = AdminRoleId,
                Name = AdminRoleName,
            };

            return adminRole;
        }
    }
}