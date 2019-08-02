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
    public class OrderServiceTests
    {
        [Fact]
        public async Task CompleteOrdersAsyncShouldReturnFalseAndDoNothingIfAnyOfTheOrdersIsCompleted()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            Order order1 = new Order
            {
                IsCompleted = false,
            };

            Order order2 = new Order
            {
                IsCompleted = true,
            };

            dbContext.Orders.AddRange(order1, order2);

            await dbContext.SaveChangesAsync();

            var orderRepository = new EfDeletableEntityRepository<Order>(dbContext);
            var productService = Mock.Of<IProductService>();
            var userService = Mock.Of<IUserService>();

            IOrderService orderService = new OrderService(
                orderRepository,
                productService,
                userService);

            Assert.False(await orderService.CompleteOrdersAsync(new[] { order1.Id, order2.Id }));
            Assert.False(order1.IsCompleted);
        }

        [Fact]
        public async Task CompleteOrdersAsyncShouldReturnTrueAndSetIsCompletedWhenNoneOtTheOrdersIsCompleted()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            Order order1 = new Order
            {
                IsCompleted = false,
            };

            Order order2 = new Order
            {
                IsCompleted = false,
            };

            dbContext.Orders.AddRange(order1, order2);

            await dbContext.SaveChangesAsync();

            var orderRepository = new EfDeletableEntityRepository<Order>(dbContext);
            var productService = Mock.Of<IProductService>();
            var userService = Mock.Of<IUserService>();

            IOrderService orderService = new OrderService(
                orderRepository,
                productService,
                userService);

            Assert.True(await orderService.CompleteOrdersAsync(new[] { order1.Id, order2.Id }));
            Assert.True(order1.IsCompleted);
            Assert.True(order2.IsCompleted);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task CreateAsyncShouldThrowWhenQuantityIsLessThanOne(int quantity)
        {
            var orderRepository = Mock.Of<IDeletableEntityRepository<Order>>();
            var productService = new Mock<IProductService>();
            var userService = new Mock<IUserService>();

            userService.Setup(us => us.ExistsAsync(string.Empty))
                .Returns(Task.FromResult(true));

            IOrderService orderService = new OrderService(
                orderRepository,
                productService.Object,
                userService.Object);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await orderService.CreateAsync(string.Empty, string.Empty, quantity);
            });
        }

        [Fact]
        public async Task CreateAsyncShouldThrowWhenUserDoestExist()
        {
            var orderRepository = Mock.Of<IDeletableEntityRepository<Order>>();
            var productService = new Mock<IProductService>();
            var userService = new Mock<IUserService>();

            userService.Setup(us => us.ExistsAsync(string.Empty))
                .Returns(Task.FromResult(false));

            IOrderService orderService = new OrderService(
                orderRepository,
                productService.Object,
                userService.Object);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await orderService.CreateAsync(string.Empty, string.Empty, 1);
            });
        }

        [Fact]
        public async Task CreateAsyncShouldReturnCorrectId()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            var orderRepository = new EfDeletableEntityRepository<Order>(dbContext);
            var productService = new Mock<IProductService>();
            var userService = new Mock<IUserService>();

            userService.Setup(us => us.ExistsAsync(nameof(Order.IssuerId)))
                .Returns(Task.FromResult(true));

            IOrderService orderService = new OrderService(
                orderRepository,
                productService.Object,
                userService.Object);

            string id = await orderService.CreateAsync(nameof(Order.ProductId), nameof(Order.IssuerId), 1);

            Order order = dbContext.Orders
                .Where(o => o.Id == id)
                .FirstOrDefault();

            Assert.NotNull(order);
            Assert.Equal(nameof(Order.ProductId), order.ProductId);
            Assert.Equal(nameof(Order.IssuerId), order.IssuerId);
            Assert.Equal(1, order.Quantity);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowIfOrderWithGivenIdDoesntExist()
        {
            var orderRepository = new Mock<IDeletableEntityRepository<Order>>();
            var productService = Mock.Of<IProductService>();
            var userService = Mock.Of<IUserService>();

            orderRepository.Setup(op => op.All())
                .Returns(new Order[0].AsQueryable);

            IOrderService orderService = new OrderService(
                orderRepository.Object,
                productService,
                userService);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await orderService.DeleteAsync(string.Empty, string.Empty);
            });
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowIfUserIsNotAdminAndIssuer()
        {
            var orderRepository = new Mock<IDeletableEntityRepository<Order>>();
            var productService = Mock.Of<IProductService>();
            var userService = new Mock<IUserService>();

            orderRepository.Setup(op => op.All())
                .Returns(new Order[]
                {
                    new Order
                    {
                        Id = nameof(Order.Id),
                        IssuerId = string.Empty,
                    },
                }
                .AsQueryable);
            userService.Setup(us => us.IsAdminAsync(nameof(Order.IssuerId)))
                .Returns(Task.FromResult(false));

            IOrderService orderService = new OrderService(
                orderRepository.Object,
                productService,
                userService.Object);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await orderService.DeleteAsync(nameof(Order.Id), nameof(Order.IssuerId));
            });
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task DeleteAsyncShouldNotThrowIfUserIsAdminOrIssuer(bool isIssuer, bool isAdmin)
        {
            var orderRepository = new Mock<IDeletableEntityRepository<Order>>();
            var productService = Mock.Of<IProductService>();
            var userService = new Mock<IUserService>();

            orderRepository.Setup(op => op.All())
                .Returns(new Order[]
                {
                    new Order
                    {
                        Id = nameof(Order.Id),
                        IssuerId = isIssuer ? nameof(Order.IssuerId) : string.Empty,
                    },
                }
                .AsQueryable);
            userService.Setup(us => us.IsAdminAsync(nameof(Order.IssuerId)))
                .Returns(Task.FromResult(isAdmin));

            IOrderService orderService = new OrderService(
                orderRepository.Object,
                productService,
                userService.Object);

            await orderService.DeleteAsync(nameof(Order.Id), nameof(Order.IssuerId));
        }

        [Fact]
        public async Task DeleteAsyncShouldSetIsDeleted()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            var orderRepository = new EfDeletableEntityRepository<Order>(dbContext);
            var productService = Mock.Of<IProductService>();
            var userService = Mock.Of<IUserService>();

            dbContext.Orders.Add(new Order
            {
                Id = nameof(Order.Id),
                IssuerId = nameof(Order.IssuerId),
            });

            await dbContext.SaveChangesAsync();

            IOrderService orderService = new OrderService(
                orderRepository,
                productService,
                userService);

            await orderService.DeleteAsync(nameof(Order.Id), nameof(Order.IssuerId));

            Order order = dbContext.Orders
                .IgnoreQueryFilters()
                .Where(o => o.Id == nameof(Order.Id))
                .FirstOrDefault();

            Assert.NotNull(order);
            Assert.True(order.IsDeleted);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectCount()
        {
            ApplicationDbContext dbContext = this.GetNewDbContext();

            var orderRepository = new EfDeletableEntityRepository<Order>(dbContext);
            var productService = Mock.Of<IProductService>();
            var userService = Mock.Of<IUserService>();

            IOrderService orderService = new OrderService(
                orderRepository,
                productService,
                userService);

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                dbContext.Orders.Add(new Order());
            }

            await dbContext.SaveChangesAsync();

            Assert.Equal(count, orderService.GetCount());
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