using System;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data;
using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Metomarket.Services.Data.Tests
{
    public class CreditCompanyServiceTests
    {
        [Fact]
        public async Task CreateAsyncShouldThrowIfNameIsAlreadyTaken()
        {
            const string name = "name";

            ApplicationDbContext dbContext = this.GetNewDbContext();

            dbContext.CreditCompanies.Add(new CreditCompany
            {
                Name = name,
            });

            dbContext.SaveChanges();

            var creditCompanyRepository = Mock.Of<IRepository<CreditCompany>>();

            ICreditCompanyService creditCompanyService = new CreditCompanyService(creditCompanyRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await creditCompanyService.CreateAsync(name, DateTime.Now.AddDays(-1));
            });
        }

        [Fact]
        public async Task CreateAsyncShouldReturnCorrectId()
        {
            const string name = "name";
            DateTime activeSince = DateTime.UtcNow.AddDays(-123);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            var creditCompanyRepository = new EfRepository<CreditCompany>(dbContext);

            ICreditCompanyService creditCompanyService = new CreditCompanyService(creditCompanyRepository);

            string id = await creditCompanyService.CreateAsync(name, activeSince);

            CreditCompany creditCompany = dbContext.CreditCompanies
                .Where(cc => cc.Id == id)
                .FirstOrDefault();

            Assert.NotNull(creditCompany);
            Assert.Equal(name, creditCompany.Name);
            Assert.Equal(activeSince, creditCompany.ActiveSince);
        }

        [Fact]
        public void ExistsShouldReturnFalseWhenEntityWithTheGivenIdIsNotPresentedInDatabase()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            var creditCompanyRepository = new EfRepository<CreditCompany>(dbContext);

            ICreditCompanyService creditCompanyService = new CreditCompanyService(creditCompanyRepository);

            Assert.False(creditCompanyService.Exists(Guid.NewGuid().ToString()));
        }

        [Fact]
        public void ExistsShouldReturnTrueWhenEntityWithTheGivenIdIsPresentedInDatabase()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            const string id = "123";
            const string name = "name";

            dbContext.CreditCompanies.Add(new CreditCompany
            {
                Id = id,
                Name = name,
            });

            dbContext.SaveChanges();

            var creditCompanyRepository = new EfRepository<CreditCompany>(dbContext);

            ICreditCompanyService creditCompanyService = new CreditCompanyService(creditCompanyRepository);

            Assert.True(creditCompanyService.Exists(id));
        }

        [Fact]
        public void GetCountShouldReturnCorrectCount()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            var creditCompanyRepository = new EfRepository<CreditCompany>(dbContext);

            ICreditCompanyService creditCompanyService = new CreditCompanyService(creditCompanyRepository);

            const int count = 3;

            for (int i = 0; i < count; i++)
            {
                dbContext.CreditCompanies.Add(new CreditCompany());
            }

            dbContext.SaveChanges();

            Assert.Equal(count, creditCompanyService.GetCount());
        }

        private ApplicationDbContext GetNewDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            ApplicationDbContext dbContext = new ApplicationDbContext(options);

            return dbContext;
        }
    }
}