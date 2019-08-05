using Metomarket.Common;
using Metomarket.Web.Areas.CreditCompanies.Controllers;
using Metomarket.Web.ViewModels.CreditCompanies;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class CreditCompaniesRouteTests
    {
        private const string Username = nameof(Username);

        [Fact]
        public void IndexShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/CreditCompanies/Index")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<CreditCompaniesController>(c => c.Index());

        [Fact]
        public void AddShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/CreditCompanies/Add")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<CreditCompaniesController>(c => c.Add());

        [Fact]
        public void PostAddShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/CreditCompanies/Add")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName))
                    .WithAntiForgeryToken())
                .To<CreditCompaniesController>(c =>
                    c.Add(With.Any<CreditCompanyCreateInputModel>()));
    }
}