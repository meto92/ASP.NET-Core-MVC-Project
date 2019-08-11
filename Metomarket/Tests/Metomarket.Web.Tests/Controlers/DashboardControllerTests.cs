using System.Collections.Generic;
using System.Linq;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Areas.Administration.Controllers;
using Metomarket.Web.ViewModels.Dashboard;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Controlers
{
    public class DashboardControllerTests
    {
        [Fact]
        public void DashboardControllerShouldInheritAdministrationControllerWhichShouldBeOnlyForAdmins()
        {
            Assert.True(typeof(AdministrationController).IsAssignableFrom(typeof(DashboardController)));

            MyMvc
                .Controller<AdministrationController>()
                .ShouldHave()
                .Attributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName));
        }

        [Fact]
        public void IndexShouldReturnViewWithEntityCounts()
            => MyMvc
                .Controller<DashboardController>()
                .WithData(db => db
                    .WithSet<Contract>(c => c.Add(new Contract()))
                    .WithSet<Order>(o => o.AddRange(this.CreateEntities<Order>(2)))
                    .WithSet<Product>(p => p.AddRange(this.CreateEntities<Product>(3)))
                    .WithSet<ProductType>(pt => pt.AddRange(this.CreateEntities<ProductType>(2)))
                    .WithSet<ApplicationUser>(u => u.Add(new ApplicationUser())))
                .Calling(c => c.Index())
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<DashboardIndexViewModel>()
                    .Passing(model =>
                    {
                        Assert.Equal(1, model.ContractsCount);
                        Assert.Equal(2, model.OrdersCount);
                        Assert.Equal(3, model.ProductsCount);
                        Assert.Equal(2, model.ProductTypesCount);
                        Assert.Equal(1, model.UsersCount);
                    }));

        private IEnumerable<TEntity> CreateEntities<TEntity>(int count)
            where TEntity : class, new()
            => Enumerable.Range(1, count)
                .Select(n => new TEntity());
    }
}