using Metomarket.Common;
using Metomarket.Web.Areas.Market.Controllers;
using Metomarket.Web.ViewModels.Orders;
using Metomarket.Web.ViewModels.Products;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class ProductsRouteTests
    {
        [Fact]
        public void CreateShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/Products/Create")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<ProductsController>(c => c.Create());

        [Fact]
        public void PostCreateShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Market/Products/Create")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName))
                    .WithAntiForgeryToken())
                .To<ProductsController>(c =>
                    c.Create(With.Any<ProductCreateInputModel>()));

        [Fact]
        public void DetailsShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap("/Market/Products/Details/1")
                .To<ProductsController>(c => c.Details("1"));

        [Fact]
        public void EditShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/Products/Edit/1")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
                .To<ProductsController>(c => c.Edit("1"));

        [Fact]
        public void PostEditShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Market/Products/Edit/1")
                    .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName))
                    .WithAntiForgeryToken())
                .To<ProductsController>(c =>
                    c.Edit(With.Any<ProductEditModel>()));

        [Fact]
        public void DeleteShouldBeRoutedCorrectly()
           => MyMvc
               .Routing()
               .ShouldMap(request => request
                   .WithLocation("/Market/Products/Delete/1")
                   .WithUser(user => user.InRole(GlobalConstants.AdministratorRoleName)))
               .To<ProductsController>(c => c.Delete("1"));

        [Fact]
        public void CreateOrderShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/Products/CreateOrder/1")
                    .WithUser())
                .To<ProductsController>(c =>
                    c.CreateOrder("1"));

        [Fact]
        public void PostCreateOrderShouldBeRoutedCorrectly()
           => MyMvc
               .Routing()
               .ShouldMap(request => request
                   .WithMethod(HttpMethod.Post)
                   .WithLocation("/Market/Products/CreateOrder")
                   .WithUser()
                   .WithAntiForgeryToken())
               .To<ProductsController>(c =>
                   c.CreateOrder(With.Any<OrderCreateInputModel>()));
    }
}