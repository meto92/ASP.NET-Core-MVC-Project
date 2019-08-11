using System.Collections.Generic;
using System.Linq;

using Metomarket.Data.Models;
using Metomarket.Web.Infrastructure.Components;
using Metomarket.Web.Infrastructure.ComponentViewModels.CreditCompanies;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Components
{
    public class CreditCompaniesViewComponentTests
    {
        [Fact]
        public void InvokeShouldReturnViewWithCorrectEntitiesCount()
            => MyViewComponent<CreditCompaniesViewComponent>
                .Instance()
                .WithData(data => data
                    .WithSet<CreditCompany>(set => set
                        .AddRange(Enumerable.Range(1, 5)
                            .Select(n => new CreditCompany()))))
                .InvokedWith(c => c.Invoke())
                .ShouldReturn()
                .View()
                .WithModelOfType<IEnumerable<CreditCompanyOptionViewModel>>()
                .Passing(model => model.Count() == 5);
    }
}