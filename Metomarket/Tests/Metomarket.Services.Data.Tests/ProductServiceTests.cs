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
    public class ProductServiceTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public async Task AddQuantityAsyncShouldThrowWhenQuantityIsNegative(int quantity)
        {
            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[0].AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.AddQuantityAsync(string.Empty, quantity);
            });
        }

        [Fact]
        public async Task AddQuantityAsyncShouldThrowIfProductWithGivenIdDoesntExist()
        {
            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[0].AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.AddQuantityAsync(nameof(Product.Id), 1);
            });
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(1, 999, 1000)]
        [InlineData(1, int.MaxValue, int.MaxValue)]
        [InlineData(int.MaxValue, int.MaxValue, int.MaxValue)]
        public async Task AddQuantityAsyncShouldWorkCorrectly(int inStock, int quantityToAdd, int expected)
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            Product product = new Product
            {
                Id = nameof(Product.Id),
                InStock = inStock,
            };

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository);

            await productService.AddQuantityAsync(nameof(Product.Id), quantityToAdd);

            Assert.Equal(expected, product.InStock);
        }

        [Fact]
        public async Task CreateAsyncShouldThrowIfProductTypeWithGivenIdDoesntExist()
        {
            var productRepository = Mock.Of<IDeletableEntityRepository<Product>>();
            var productTypeRepository = new Mock<IRepository<ProductType>>();

            productTypeRepository.Setup(ptr => ptr.All())
                .Returns(new ProductType[0].AsQueryable);

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository.Object);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.CreateAsync(string.Empty, 1, string.Empty, 1, nameof(ProductType.Id));
            });
        }

        [Fact]
        public async Task CreateAsyncShouldReturnCorrectId()
        {
            const string productName = nameof(Product.Name);
            const decimal price = 100;
            const string imageUrl = nameof(Product.ImageUrl);
            const int inStock = 15;
            const string typeId = nameof(ProductType.Id);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = new Mock<IRepository<ProductType>>();

            productTypeRepository.Setup(ptr => ptr.All())
                .Returns(new ProductType[]
                {
                    new ProductType
                    {
                        Id = typeId,
                    },
                }
                .AsQueryable);

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository.Object);

            string id = await productService.CreateAsync(productName, price, imageUrl, inStock, typeId);

            Product product = dbContext.Products
                .Where(p => p.Id == id)
                .FirstOrDefault();

            Assert.NotNull(product);
            Assert.Equal(productName, product.Name);
            Assert.Equal(price, product.Price);
            Assert.Equal(imageUrl, product.ImageUrl);
            Assert.Equal(inStock, product.InStock);
            Assert.Equal(typeId, product.TypeId);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowIfProductWithGivenIdDoesntExist()
        {
            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[0].AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.DeleteAsync(nameof(Product.Id));
            });
        }

        [Fact]
        public async Task DeleteAsyncShouldSetIsDeletedAndReturnTrue()
        {
            const string id = nameof(Product.Id);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            Product product = new Product
            {
                Id = id,
            };

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = new Mock<IRepository<ProductType>>();

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository.Object);

            bool result = await productService.DeleteAsync(id);

            Assert.True(product.IsDeleted);
            Assert.True(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ExistsShouldCheckWhetherProductWithGivenIdExists(bool exists)
        {
            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            Product[] products = new Product[0];

            if (exists)
            {
                products = new Product[]
                {
                    new Product
                    {
                        Id = nameof(Product.Id),
                    },
                };
            }

            productRepository.Setup(pr => pr.All())
                .Returns(products.AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            bool result = productService.Exists(nameof(Product.Id));

            Assert.Equal(exists, result);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectCount()
        {
            const int count = 7;

            ApplicationDbContext dbContext = this.GetNewDbContext();

            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = new Mock<IRepository<ProductType>>();

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository.Object);

            for (int i = 0; i < count; i++)
            {
                dbContext.Products.Add(new Product());
            }

            await dbContext.SaveChangesAsync();

            Assert.Equal(count, productService.GetCount());
        }

        [Fact]
        public async Task ReduceQuantityAsyncShouldThrowIfProductWithGivenIdDoesntExist()
        {
            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[0].AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.ReduceQuantityAsync(nameof(Product.Id), 1);
            });
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(10, 100)]
        public async Task ReduceQuantityAsyncShouldThrowWhenQuantityIsMoreThanProductQuantityInStock(int inStock, int quantity)
        {
            const string id = nameof(Product.Id);

            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            Product product = new Product
            {
                Id = id,
                InStock = inStock,
            };

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[] { product }.AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.ReduceQuantityAsync(id, quantity);
            });
        }

        [Fact]
        public async Task ReduceQuantityAsyncShouldWorkCorrectly()
        {
            const string id = nameof(Product.Id);
            const int inStock = 50;
            const int quantity = 22;

            ApplicationDbContext dbContext = this.GetNewDbContext();

            Product product = new Product
            {
                Id = id,
                InStock = inStock,
            };

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository);

            await productService.ReduceQuantityAsync(id, quantity);

            Assert.Equal(inStock - quantity, product.InStock);
        }

        [Fact]
        public async Task UpdateAsyncShouldThrowIfProductWithGivenIdDoesntExist()
        {
            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[0].AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await productService.UpdateAsync(
                    nameof(Product.Id),
                    string.Empty,
                    1,
                    string.Empty,
                    1);
            });
        }

        [Fact]
        public async Task UpdateAsyncShouldReturnFalseIfNothingIsChanged()
        {
            const string id = nameof(Product.Id);
            const string name = nameof(Product.Name);
            const decimal price = 100;
            const string imageUrl = nameof(Product.ImageUrl);
            const int inStock = 15;

            var productRepository = new Mock<IDeletableEntityRepository<Product>>();
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            Product product = new Product
            {
                Id = id,
                Name = name,
                Price = price,
                ImageUrl = imageUrl,
                InStock = inStock,
            };

            productRepository.Setup(pr => pr.All())
                .Returns(new Product[] { product }.AsQueryable);

            IProductService productService = new ProductService(
                productRepository.Object,
                productTypeRepository);

            bool result = await productService.UpdateAsync(id, name, price, imageUrl, 0);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsyncShouldSetGivenValuesAndReturnTrue()
        {
            const string id = nameof(Product.Id);
            const string name = nameof(Product.Name);
            const decimal price = 100;
            const string imageUrl = nameof(Product.ImageUrl);
            const int inStock = 1;
            const int quantityToAdd = 15;

            ApplicationDbContext dbContext = this.GetNewDbContext();

            Product product = new Product
            {
                Id = id,
                InStock = inStock,
            };

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();

            IProductService productService = new ProductService(
                productRepository,
                productTypeRepository);

            bool result = await productService.UpdateAsync(id, name, price, imageUrl, quantityToAdd);

            Assert.Equal(name, product.Name);
            Assert.Equal(price, product.Price);
            Assert.Equal(imageUrl, product.ImageUrl);
            Assert.Equal(inStock + quantityToAdd, product.InStock);
            Assert.True(result);
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