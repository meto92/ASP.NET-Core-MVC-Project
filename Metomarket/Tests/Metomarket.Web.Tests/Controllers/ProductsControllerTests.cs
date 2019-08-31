using System.Linq;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Areas.Market.Controllers;
using Metomarket.Web.Controllers;
using Metomarket.Web.ViewModels.Orders;
using Metomarket.Web.ViewModels.Products;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private const string Id = nameof(Id);
        private const string Name = nameof(Name);
        private const string ImageUrl = "https://media.4rgos.it/i/Argos/8729493_R_Z001A?w=750&h=440&qlt=70";
        private const decimal Price = 123;
        private const int InStock = 19;
        private const string Type = "Laptop";

        [Fact]
        public void CreateShouldBeOnlyForAdminsAndShouldReturnView()
            => MyMvc
                .Controller<ProductsController>()
                .Calling(c => c.Create())
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductCreateInputModel>());

        [Fact]
        public void PostCreateShouldBeOnlyForAdmins()
            => MyMvc
                .Controller<ProductsController>()
                .Calling(c => c.Create(With.Default<ProductCreateInputModel>()))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName));

        [Fact]
        public void PostCreateWithInvalidDataShouldHaveInvalidModelStateAndReturnViewWithOldValues()
            => MyMvc
                .Controller<ProductsController>()
                .Calling(c => c.Create(new ProductCreateInputModel
                {
                    Name = string.Empty,
                    ImageUrl = "url",
                    InStock = -1,
                    Price = 10,
                    TypeId = "1",
                }))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductCreateInputModel>()
                    .Passing(model =>
                    {
                        Assert.Equal(string.Empty, model.Name);
                        Assert.Equal("url", model.ImageUrl);
                        Assert.Equal(-1, model.InStock);
                        Assert.Equal(10, model.Price);
                        Assert.Equal("1", model.TypeId);
                    }));

        [Fact]
        public void PostCreateWithValidDataShouldHaveValidModelStateAddEntityAndRedirect()
        {
            const string typeId = nameof(typeId);
            const string name = nameof(name);
            const string imageUrl = "https://media.4rgos.it/i/Argos/8729493_R_Z001A?w=750&h=440&qlt=70";
            const decimal price = 123;
            const int inStock = 19;

            MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<ProductType>(pt => pt
                        .Add(new ProductType { Id = typeId })))
                .Calling(c => c.Create(new ProductCreateInputModel
                {
                    Name = name,
                    ImageUrl = imageUrl,
                    InStock = inStock,
                    Price = price,
                    TypeId = typeId,
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<Product>(set => set
                        .Any(p => p.Name == name
                            && p.ImageUrl == imageUrl
                            && p.InStock == inStock
                            && p.Price == price
                            && p.TypeId == typeId)))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ProductsController.Details)));
        }

        [Fact]
        public void DetailsShouldReturnViewWithCorrectValues()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.Details(Id))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductDetailsViewModel>()
                    .Passing(model =>
                    {
                        Assert.Equal(Name, model.Name);
                        Assert.Equal(ImageUrl, model.ImageUrl);
                        Assert.Equal(Price, model.Price);
                        Assert.Equal(InStock, model.InStock);
                        Assert.Equal(Type, model.TypeName);
                    }));

        [Fact]
        public void EditShouldBeOnlyForAdminsAndShouldReturnViewWithCorrectValues()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.Edit(Id))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductEditModel>()
                    .Passing(model =>
                    {
                        Assert.Equal(Name, model.Name);
                        Assert.Equal(ImageUrl, model.ImageUrl);
                        Assert.Equal(Price, model.Price);
                        Assert.Equal(InStock, model.InStock);
                        Assert.Equal(Type, model.TypeName);
                    }));

        [Fact]
        public void PostEditShouldBeOnlyForAdminsAndShouldHaveInvalidModelStateAndRedirectWhenDataIsInvalid()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.Edit(new ProductEditModel
                {
                    Id = Id,
                    ImageUrl = ImageUrl,
                    Name = string.Empty,
                    Price = 100,
                    QuantityToAdd = -1,
                }))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect =>
                    redirect.ToAction(nameof(ProductsController.Edit)));

        [Fact]
        public void PostEditWithValidDataShouldHaveValidModelStateAndShouldUpdateEntityAndRedirect()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.Edit(new ProductEditModel
                {
                    Id = Id,
                    ImageUrl = ImageUrl,
                    Name = "new name",
                    Price = 100,
                    QuantityToAdd = 3,
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<Product>(set => set
                        .Any(p => p.Id == Id
                            && p.Name == "new name"
                            && p.Price == 100
                            && p.InStock == InStock + 3)))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect =>
                    redirect.ToAction(nameof(ProductsController.Details)));

        [Fact]
        public void DeleteShouldBeOnlyForAdminsAndShouldRedirectToHomePageWhenSucceeded()
             => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.Delete(Id))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<Product>(set => set.Count() == 0))
                .AndAlso()
                .ShouldReturn()
                .Redirect(/*redirect => redirect
                    .To<HomeController>(c => c.Index(With.Any<string>(), With.Any<bool>()))*/);

        [Fact]
        public void InitializeOrderShouldBeOnlyForAuthorizedUsers()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.InitializeOrder(Id))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ProductsController.CreateOrder)));

        [Fact]
        public void CreateOrderShouldBeOnlyForAuthorizedUsersAndShouldReturnView()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.CreateOrder(Id))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductCreateOrderViewModel>()
                    .Passing(model =>
                    {
                        Assert.Equal(Id, model.Id);
                        Assert.Equal(Name, model.Name);
                        Assert.Equal(Price, model.Price);
                        Assert.Equal(ImageUrl, model.ImageUrl);
                    }));

        [Fact]
        public void PostCreateOrderShouldBeOnlyForAuthorizedUsersAndShouldHaveInvalidModelStateAndRedirectWhenDataIsInvalid()
            => MyMvc
                .Controller<ProductsController>()
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct())))
                .Calling(c => c.CreateOrder(new OrderCreateInputModel
                {
                    ProductId = Id,
                    Quantity = -1,
                }))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ProductsController.CreateOrder)));

        [Fact]
        public void PostCreateOrderWithValidDataShouldHaveValidModelStateAndShouldCreateNewOrderAndShouldRedirect()
            => MyMvc
                .Controller<ProductsController>()
                .WithUser(user => user.WithIdentifier("1"))
                .WithData(db => db
                    .WithSet<Product>(set => set.Add(this.CreateTestProduct()))
                    .WithSet<ApplicationUser>(set => set
                        .Add(new ApplicationUser { Id = "1" })))
                .Calling(c => c.CreateOrder(new OrderCreateInputModel
                {
                    ProductId = Id,
                    Quantity = 2,
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<Order>(set => set
                        .Any(order => order.ProductId == Id && order.Quantity == 2)))
                .AndAlso()
                .ShouldReturn()
                .Redirect(/*redirect => redirect
                    .To<HomeController>(c => c.Index(With.Any<string>(), With.Any<bool>()))*/);

        private Product CreateTestProduct()
        {
            Product product = new Product
            {
                Id = Id,
                Name = Name,
                ImageUrl = ImageUrl,
                InStock = InStock,
                Price = Price,
                Type = new ProductType
                {
                    Name = Type,
                },
            };

            return product;
        }
    }
}