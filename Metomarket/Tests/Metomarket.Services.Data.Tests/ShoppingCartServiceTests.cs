﻿using System;
using System.Collections.Generic;
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
    public class ShoppingCartServiceTests
    {
        [Fact]
        public async Task AddOrderAsyncShouldThrownIfShoppingCartDoesntExist()
        {
            const string orderId = nameof(orderId);

            var shoppingCartRepository = new Mock<IRepository<ShoppingCart>>();
            var orderRepository = new Mock<IRepository<Order>>();
            var productService = Mock.Of<IProductService>();

            shoppingCartRepository.Setup(scr => scr.All())
                .Returns(new ShoppingCart[0].AsQueryable);
            orderRepository.Setup(or => or.All())
                .Returns(new Order[]
                {
                    new Order
                    {
                        Id = orderId,
                    },
                }
                .AsQueryable);

            IShoppingCartService service = new ShoppingCartService(
                shoppingCartRepository.Object,
                orderRepository.Object,
                productService);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await service.AddOrderAsync(nameof(ShoppingCart.CustomerId), orderId);
            });
        }

        [Fact]
        public async Task AddOrderAsyncShouldThrownIfOrderDoesntExist()
        {
            const string customerId = nameof(customerId);

            var shoppingCartRepository = new Mock<IRepository<ShoppingCart>>();
            var orderRepository = new Mock<IRepository<Order>>();
            var productService = Mock.Of<IProductService>();

            shoppingCartRepository.Setup(scr => scr.All())
                .Returns(new ShoppingCart[]
                {
                    new ShoppingCart
                    {
                        CustomerId = customerId,
                    },
                }
                .AsQueryable);
            orderRepository.Setup(or => or.All())
                .Returns(new Order[0].AsQueryable);

            IShoppingCartService service = new ShoppingCartService(
                shoppingCartRepository.Object,
                orderRepository.Object,
                productService);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await service.AddOrderAsync(customerId, nameof(Order.Id));
            });
        }

        [Fact]
        public async Task AddOrderAsyncShouldWorkCorrectly()
        {
            const string customerId = nameof(customerId);
            const string orderId = nameof(orderId);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            ShoppingCart shoppingCart = new ShoppingCart
            {
                CustomerId = customerId,
            };

            dbContext.ShoppingCarts.Add(shoppingCart);

            await dbContext.SaveChangesAsync();

            var shoppingCartRepository = new EfRepository<ShoppingCart>(dbContext);
            var orderRepository = new Mock<IRepository<Order>>();
            var productService = Mock.Of<IProductService>();

            orderRepository.Setup(or => or.All())
                .Returns(new Order[]
                {
                    new Order
                    {
                        Id = orderId,
                    },
                }
                .AsQueryable);

            IShoppingCartService service = new ShoppingCartService(
                shoppingCartRepository,
                orderRepository.Object,
                productService);

            await service.AddOrderAsync(customerId, orderId);

            Assert.Single(shoppingCart.Orders);
            Assert.Equal(orderId, shoppingCart.Orders.First().Id);
        }

        [Fact]
        public async Task EmptyCartAsyncShouldThrownIfShoppingCartDoesntExist()
        {
            var shoppingCartRepository = new Mock<IRepository<ShoppingCart>>();
            var orderRepository = Mock.Of<IRepository<Order>>();
            var productService = Mock.Of<IProductService>();

            shoppingCartRepository.Setup(scr => scr.All())
                .Returns(new ShoppingCart[0].AsQueryable);

            IShoppingCartService service = new ShoppingCartService(
                shoppingCartRepository.Object,
                orderRepository,
                productService);

            await Assert.ThrowsAsync<ServiceException>(async () =>
            {
                await service.EmptyCartAsync(nameof(ShoppingCart.CustomerId));
            });
        }

        [Fact]
        public async Task EmptyCartAsyncShouldClearOrders()
        {
            const string customerId = nameof(customerId);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            ShoppingCart shoppingCart = new ShoppingCart
            {
                CustomerId = customerId,
                Orders = new List<Order>
                {
                    new Order(),
                    new Order(),
                    new Order(),
                },
            };

            dbContext.ShoppingCarts.Add(shoppingCart);

            await dbContext.SaveChangesAsync();

            var shoppingCartRepository = new EfRepository<ShoppingCart>(dbContext);
            var orderRepository = Mock.Of<IRepository<Order>>();
            var productService = Mock.Of<IProductService>();

            IShoppingCartService service = new ShoppingCartService(
                shoppingCartRepository,
                orderRepository,
                productService);

            await service.EmptyCartAsync(customerId);

            Assert.Empty(shoppingCart.Orders);
        }

        [Fact]
        public async Task EmptyCartAsyncShouldRestoreProductQuantitiesWhenRequested()
        {
            const int ordersCount = 3;
            const string customerId = nameof(customerId);

            ApplicationDbContext dbContext = this.GetNewDbContext();

            List<Order> orders = Enumerable.Range(1, ordersCount)
                .Select(n =>
                new Order
                {
                    Product = new Product
                    {
                        Id = n.ToString(),
                        InStock = 0,
                    },
                    Quantity = n,
                })
                .ToList();

            ShoppingCart shoppingCart = new ShoppingCart
            {
                CustomerId = customerId,
                Orders = orders,
            };

            dbContext.ShoppingCarts.Add(shoppingCart);

            await dbContext.SaveChangesAsync();

            var shoppingCartRepository = new EfRepository<ShoppingCart>(dbContext);
            var orderRepository = Mock.Of<IRepository<Order>>();
            var productRepository = new EfDeletableEntityRepository<Product>(dbContext);
            var productTypeRepository = Mock.Of<IRepository<ProductType>>();
            var productService = new ProductService(productRepository, productTypeRepository);

            IShoppingCartService service = new ShoppingCartService(
                shoppingCartRepository,
                orderRepository,
                productService);

            await service.EmptyCartAsync(customerId, true);

            for (int i = 0; i < ordersCount; i++)
            {
                Product product = dbContext.Products
                    .Where(p => p.Id == (i + 1).ToString())
                    .FirstOrDefault();

                Assert.Equal(i + 1, product?.InStock);
            }
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