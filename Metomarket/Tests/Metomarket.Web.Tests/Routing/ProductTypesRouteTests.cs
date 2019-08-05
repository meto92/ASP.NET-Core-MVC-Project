using Metomarket.Common;
using Metomarket.Web.Areas.Market.Controllers;
using Metomarket.Web.ViewModels.ProductTypes;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class ProductTypesRouteTests
    {
        [Fact]
        public void IndexShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ProductTypes/Index")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<ProductTypesController>(c => c.Index());

        [Fact]
        public void SlashShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ProductTypes/")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<ProductTypesController>(c => c.Index());

        [Fact]
        public void CreateShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ProductTypes/Create")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<ProductTypesController>(c => c.Create());

        [Fact]
        public void PostCreateShouldBeRoutedCorrectly()
           => MyMvc
               .Routing()
               .ShouldMap(request => request
                   .WithMethod(HttpMethod.Post)
                   .WithLocation("/Market/ProductTypes/Create")
                   .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName))
                   .WithAntiForgeryToken())
               .To<ProductTypesController>(c =>
                   c.Create(With.Any<ProductTypeCreateInputModel>()));

        [Fact]
        public void EditShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ProductTypes/Edit/1")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<ProductTypesController>(c => c.Edit("1"));

        [Fact]
        public void PostEditShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Market/ProductTypes/Edit")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName))
                    .WithAntiForgeryToken())
                .To<ProductTypesController>(c =>
                    c.Edit(With.Any<ProductTypeEditModel>()));
    }
}