using Metomarket.Web.Areas.Market.Controllers;
using Metomarket.Web.ViewModels.ShoppingCarts;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Routing
{
    public class ShoppingCartRouteTests
    {
        [Fact]
        public void IndexShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ShoppingCart/Index")
                    .WithUser())
                .To<ShoppingCartController>(c => c.Index());

        [Fact]
        public void SlashShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ShoppingCart/")
                    .WithUser())
                .To<ShoppingCartController>(c => c.Index());

        [Fact]
        public void DeleteOrderShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ShoppingCart/DeleteOrder/1")
                    .WithUser())
                .To<ShoppingCartController>(c => c.DeleteOrder("1"));

        [Fact]
        public void CompleteOrdersShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithLocation("/Market/ShoppingCart/CompleteOrders")
                    .WithUser())
                .To<ShoppingCartController>(c => c.CompleteOrders());

        [Fact]
        public void PostCompleteOrdersShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Market/ShoppingCart/CompleteOrders")
                    .WithUser()
                    .WithAntiForgeryToken())
                .To<ShoppingCartController>(c =>
                    c.CompleteOrders(With.Any<CompleteOrdersModel>()));

        [Fact]
        public void PostEmptyCartShouldBeRoutedCorrectly()
            => MyMvc
                .Routing()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Market/ShoppingCart/EmptyCart")
                    .WithUser()
                    .WithAntiForgeryToken())
                .To<ShoppingCartController>(c => c.EmptyCart());
    }
}