using System.Collections.Generic;
using System.Linq;

using Metomarket.Data.Models;
using Metomarket.Web.Infrastructure.Components;
using Metomarket.Web.Infrastructure.ComponentViewModels.ProductTypes;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Components
{
    public class ProductTypesViewComponentTests
    {
        [Fact]
        public void InvokeShouldReturnViewWithCorrectEntitiesCount()
            => MyViewComponent<ProductTypesViewComponent>
                .Instance()
                .WithData(data => data
                    .WithSet<ProductType>(set => set
                        .AddRange(Enumerable.Range(1, 11)
                            .Select(n => new ProductType()))))
                .InvokedWith(c => c.Invoke())
                .ShouldReturn()
                .View()
                .WithModelOfType<IEnumerable<ProductTypeOptionViewModel>>()
                .Passing(model => model.Count() == 11);
    }
}