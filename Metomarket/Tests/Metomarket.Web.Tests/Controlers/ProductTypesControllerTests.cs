using System.Linq;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Areas.Market.Controllers;
using Metomarket.Web.ViewModels.ProductTypes;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Controlers
{
    public class ProductTypesControllerTests
    {
        private const string Id = nameof(Id);
        private const string Name = "Laptop";

        [Fact]
        public void ProductTypesControllerShouldBeOnlyForAdmins()
            => MyMvc
                .Controller<ProductTypesController>()
                .ShouldHave()
                .Attributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName));

        [Fact]
        public void IndexShouldReturnViewWithMappedEntities()
            => MyMvc
                .Controller<ProductTypesController>()
                .WithData(db => db
                    .WithSet<ProductType>(set => set
                        .AddRange(Enumerable.Range(1, 3).Select(n => new ProductType
                        {
                            Id = n.ToString(),
                            Name = $"name{n}",
                            Products = Enumerable.Range(1, n)
                                .Select(x => new Product())
                                .ToList(),
                        }))))
                .Calling(c => c.Index())
                .ShouldReturn()
                .View(new ProductTypesListViewModel
                {
                    ProductTypes = Enumerable.Range(1, 3).Select(n => new ProductTypeViewModel
                    {
                        Id = n.ToString(),
                        Name = $"name{n}",
                        ProductsCount = n,
                    }),
                });

        [Fact]
        public void CreateShouldReturnView()
            => MyMvc
                .Controller<ProductTypesController>()
                .Calling(c => c.Create())
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductTypeCreateInputModel>());

        [Fact]
        public void PostCreateWithInvalidDataShouldHaveInvalidModelStateAndReturnViewWithOldValues()
            => MyMvc
                .Controller<ProductTypesController>()
                .Calling(c => c.Create(new ProductTypeCreateInputModel
                {
                    Name = "N",
                }))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post))
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductTypeCreateInputModel>()
                    .Passing(model =>
                    {
                        Assert.Equal("N", model.Name);
                    }));

        [Fact]
        public void PostCreateWithValidDataShouldHaveValidModelStateAddEntityAndRedirect()
            => MyMvc
                .Controller<ProductTypesController>()
                .Calling(c => c.Create(new ProductTypeCreateInputModel
                {
                    Name = Name,
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<ProductType>(set => set
                        .Single(pt => pt.Name == Name)))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ProductTypesController.Index)));

        [Fact]
        public void EditShouldReturnViewWithCorrectValues()
            => MyMvc
                .Controller<ProductTypesController>()
                .WithData(db => db
                    .WithSet<ProductType>(set => set.Add(this.CreateTestProductType())))
                .Calling(c => c.Edit(Id))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductTypeEditModel>()
                    .Passing(model =>
                    {
                        Assert.Equal(Id, model.Id);
                        Assert.Equal(Name, model.Name);
                    }));

        [Fact]
        public void PostEditWithInvalidDataShouldHaveInvalidModelStateAndReturnViewWithOldValues()
            => MyMvc
                .Controller<ProductTypesController>()
                .WithData(db => db
                    .WithSet<ProductType>(set => set.Add(this.CreateTestProductType())))
                .Calling(c => c.Edit(new ProductTypeEditModel
                {
                    Id = Id,
                    Name = "N",
                }))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post))
                .AndAlso()
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<ProductTypeEditModel>()
                    .Passing(model => model.Id == Id && model.Name == "N"));

        [Fact]
        public void PostEditWithValidDataShouldHaveValidModelStateAndShouldUpdateEntityAndRedirect()
            => MyMvc
                .Controller<ProductTypesController>()
                .WithData(db => db
                    .WithSet<ProductType>(set => set.Add(this.CreateTestProductType())))
                .Calling(c => c.Edit(new ProductTypeEditModel
                {
                    Id = Id,
                    Name = "New name",
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<ProductType>(set => set
                        .Single(pt => pt.Id == Id && pt.Name == "New name")))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .ToAction(nameof(ProductTypesController.Index)));

        private ProductType CreateTestProductType()
        {
            ProductType productType = new ProductType
            {
                Id = Id,
                Name = Name,
            };

            return productType;
        }
    }
}