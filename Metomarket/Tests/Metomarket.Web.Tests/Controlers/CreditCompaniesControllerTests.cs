using System;
using System.Linq;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Web.Areas.CreditCompanies.Controllers;
using Metomarket.Web.ViewModels.CreditCompanies;

using MyTested.AspNetCore.Mvc;

using Xunit;

namespace Metomarket.Web.Tests.Controlers
{
    public class CreditCompaniesControllerTests
    {
        [Fact]
        public void CreditCompaniesControllerShouldBeOnlyForAdmins()
            => MyMvc
                .Controller<CreditCompaniesController>()
                .ShouldHave()
                .Attributes(attrs => attrs
                    .RestrictingForAuthorizedRequests(GlobalConstants.AdministratorRoleName));

        [Fact]
        public void IndexShouldReturnViewWithMappedEntities()
            => MyMvc
                .Controller<CreditCompaniesController>()
                .WithoutValidation()
                .WithData(db => db
                    .WithSet<CreditCompany>(cc => cc.AddRange(new CreditCompany[]
                    {
                        new CreditCompany
                        {
                            Name = "name1",
                        },
                        new CreditCompany
                        {
                            Name = "name2",
                            Contracts = new Contract[]
                            {
                                new Contract(),
                            },
                        },
                    })))
                .Calling(c => c.Index())
                .ShouldReturn()
                .View(new CreditCompaniesListIndexViewModel
                {
                    CreditCompanies = new CreditCompanyIndexViewModel[]
                    {
                        new CreditCompanyIndexViewModel
                        {
                            Name = "name1",
                            ContractsCount = 0,
                        },
                        new CreditCompanyIndexViewModel
                        {
                            Name = "name2",
                            ContractsCount = 1,
                        },
                    },
                });

        [Fact]
        public void AddShouldReturnView()
            => MyMvc
                .Controller<CreditCompaniesController>()
                .Calling(c => c.Add())
                .ShouldReturn()
                .View();

        [Fact]
        public void PostAddWithInvalidDataShouldHaveModelErrors()
            => MyMvc
                .Controller<CreditCompaniesController>()
                .Calling(c => c.Add(new CreditCompanyCreateInputModel
                {
                    Name = string.Empty,
                    ActiveSincce = DateTime.Now.AddDays(1),
                }))
                .ShouldHave()
                .ModelState(modelState => modelState
                    .For<CreditCompanyCreateInputModel>()
                    .ContainingErrorFor(m => m.Name)
                    .AndAlso()
                    .ContainingErrorFor(m => m.ActiveSincce))
                .AndAlso()
                .ShouldReturn()
                .View();

        [Fact]
        public void PostAddWithValidDataShouldHaveValidModelStateAddNewEntityAndRedirect()
            => MyMvc
                .Controller<CreditCompaniesController>()
                .Calling(c => c.Add(new CreditCompanyCreateInputModel
                {
                    Name = "name",
                    ActiveSincce = DateTime.Now.AddDays(-1),
                }))
                .ShouldHave()
                .ActionAttributes(attrs => attrs
                    .RestrictingForHttpMethod(HttpMethod.Post))
                .AndAlso()
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(db => db
                    .WithSet<CreditCompany>(set => set
                        .Any(c => c.Name == "name")))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .To<CreditCompaniesController>(c => c.Index()));
    }
}