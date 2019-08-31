using System.Linq;

using Metomarket.Data.Models;
using Metomarket.Web.Areas.Market.Controllers;
using Metomarket.Web.Controllers;
using Metomarket.Web.ViewModels.ShoppingCarts;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Controllers
{
    public class ShoppingCartControllerTests
    {
        private const string UserId = nameof(UserId);

        [Fact]
        public void ShoppingCartControllerShouldBeOnlyForAuthorizedUsers()
            => MyMvc
                .Controller<ShoppingCartController>()
                .ShouldHave()
                .Attributes(attrs => attrs
                    .RestrictingForAuthorizedRequests());

        [Fact]
        public void UserShouldHaveShoppingCart()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(user => user.WithIdentifier(UserId))
                .WithData(db => db
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId })))
                .Calling(c => c.Index())
                .ShouldHave()
                .Data(db => db
                    .WithSet<ShoppingCart>(set => set
                        .Single(sc => sc.CustomerId == UserId)));

        [Fact]
        public void IndexShouldReturnView()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(user => user.WithIdentifier(UserId))
                .WithData(db => db
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId })))
                .Calling(c => c.Index())
                .ShouldReturn()
                .View(view =>
                    view.WithModelOfType<ShoppingCartViewModel>());

        [Fact]
        public void DeleteOrderShouldRedirectWhenSucceeded()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(UserId, "username")
                .WithData(db => db
                    .WithSet<Order>(set => set
                        .Add(new Order
                        {
                            Id = "OrderId",
                            Product = new Product(),
                            IssuerId = UserId,
                        }))
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId })))
                .Calling(c => c.DeleteOrder("OrderId"))
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ShoppingCartController.Index)));

        [Fact]
        public void CompleteOrdersShouldRedirectWhenShoppingCartIsEmpty()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(UserId, "username")
                .WithData(db => db
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId })))
                .Calling(c => c.CompleteOrders())
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ShoppingCartController.Index)));

        [Fact]
        public void PostCompleteOrdersWithInvalidDataShouldHaveInvalidModelStateAndRedirect()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(UserId, "username")
                .WithData(db => db
                    .WithSet<Order>(set => set
                        .Add(new Order
                        {
                            Id = "OrderId",
                            Product = new Product(),
                            IssuerId = UserId,
                        }))
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId })))
                .Calling(c => c.CompleteOrders(new CompleteOrdersModel
                {
                    CreditCardNumber = string.Empty,
                    PeriodInMonths = -1,
                    CreditCompanyId = string.Empty,
                }))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post))
                .AndAlso()
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ShoppingCartController.CompleteOrders)));

        [Fact]
        public void PostCompleteOrdersWithValidDataShouldHaveValidModelStateCreateContractAndRedirect()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(UserId, "username")
                .WithData(db => db
                    .WithSet<Order>(set => set
                        .Add(new Order
                        {
                            Id = "OrderId",
                            Product = new Product(),
                            IssuerId = UserId,
                        }))
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId }))
                    .WithSet<CreditCompany>(set => set
                        .Add(new CreditCompany { Id = "CompanyId" })))
                .Calling(c => c.CompleteOrders(new CompleteOrdersModel
                {
                    CreditCardNumber = "4879548847880307",
                    PeriodInMonths = 3,
                    CreditCompanyId = "CompanyId",
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<Contract>(set => set
                        .Single(c => c.CustomerId == UserId)))
                .AndAlso()
                .ShouldReturn()
                .Redirect(/*redirect => redirect
                    .To<HomeController>(c => c.Index(With.Any<string>(), With.Any<bool>()))*/);

        [Fact]
        public void EmptyCartShouldRedirect()
            => MyMvc
                .Controller<ShoppingCartController>()
                .WithUser(UserId, "username")
                .WithData(db => db
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = UserId })))
                .Calling(c => c.EmptyCart())
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ShoppingCartController.Index)));
    }
}