using System;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data;
using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Metomarket.Services.Data.Tests
{
    public class ProductTypeServiceTests
    {
        [Fact]
        public async Task CreateAsyncShouldThrowIfNameIsTaken()
        {
            const string name = nameof(name);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            dbContext.ProductTypes.Add(new ProductType
            {
                Name = name,
            });

            await dbContext.SaveChangesAsync();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await service.CreateAsync(name);
            });
        }

        [Fact]
        public async Task CreateAsyncShouldReturnCorrectId()
        {
            const string name = nameof(name);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            string id = await service.CreateAsync(name);

            ProductType productType = dbContext.ProductTypes
                .Where(pt => pt.Id == id)
                .FirstOrDefault();

            Assert.NotNull(productType);
            Assert.Equal(name, productType.Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExistsShouldCheckWhetherProducTypetWithGivenNameExists(bool exists)
        {
            const string name = nameof(name);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            if (exists)
            {
                dbContext.ProductTypes.Add(new ProductType
                {
                    Name = name,
                });

                await dbContext.SaveChangesAsync();
            }

            bool result = service.Exists(name);

            Assert.Equal(exists, result);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectCount()
        {
            const int count = 11;

            ApplicationDbContext dbContext = this.GetNewDbContext();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            for (int i = 0; i < count; i++)
            {
                dbContext.ProductTypes.Add(new ProductType());
            }

            await dbContext.SaveChangesAsync();

            Assert.Equal(count, service.GetCount());
        }

        [Fact]
        public async Task UpdateAsyncShouldThrowIfProductTypeWithGivenIdDoesntExist()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await service.UpdateAsync(nameof(ProductType.Id), nameof(ProductType.Name));
            });
        }

        [Fact]
        public async Task UpdateAsyncShouldReturnFalseIfNewNameEqualsCurrent()
        {
            const string id = nameof(id);
            const string name = nameof(name);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            dbContext.ProductTypes.Add(new ProductType
            {
                Id = id,
                Name = name,
            });

            await dbContext.SaveChangesAsync();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            bool result = await service.UpdateAsync(id, name);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsyncShouldUpdateName()
        {
            const string id = nameof(id);
            const string name = nameof(name);
            const string newName = nameof(newName);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            ProductType productType = new ProductType
            {
                Id = id,
                Name = name,
            };

            dbContext.ProductTypes.Add(productType);

            await dbContext.SaveChangesAsync();

            IRepository<ProductType> repository = new EfRepository<ProductType>(dbContext);

            IProductTypeService service = new ProductTypeService(repository);

            bool result = await service.UpdateAsync(id, newName);

            Assert.True(result);
            Assert.Equal(newName, productType.Name);
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