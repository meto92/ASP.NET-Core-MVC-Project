﻿using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

using Microsoft.EntityFrameworkCore;

namespace Metomarket.Services.Data
{
    public class ShoppingCartService : IShoppingCartService
    {
        private const string OrderNotFoundMessage = "Order not found.";

        private readonly IRepository<ShoppingCart> shoppingCartRepository;
        private readonly IRepository<Order> orderRepository;

        public ShoppingCartService(
            IRepository<ShoppingCart> shoppingCartRepository,
            IRepository<Order> orderRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.orderRepository = orderRepository;
        }

        public async Task<bool> AddOrderAsync(string userId, string orderId)
        {
            ShoppingCart shoppingCart = this.shoppingCartRepository.All()
                .Where(sc => sc.CustomerId == userId)
                .FirstOrDefault();

            if (shoppingCart == null)
            {
                throw new ServiceException();
            }

            Order order = this.orderRepository.All()
                .Where(o => o.Id == orderId)
                .FirstOrDefault();

            if (order == null)
            {
                throw new ServiceException(OrderNotFoundMessage);
            }

            shoppingCart.Orders.Add(order);

            await this.shoppingCartRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EmptyCartAsync(string userId)
        {
            ShoppingCart shoppingCart = this.shoppingCartRepository.All()
                .Include(sc => sc.Orders)
                .Where(sc => sc.CustomerId == userId)
                .FirstOrDefault();

            if (shoppingCart == null)
            {
                throw new ServiceException();
            }

            shoppingCart.Orders.Clear();

            await this.shoppingCartRepository.SaveChangesAsync();

            return true;
        }

        public TModel FindByUserId<TModel>(string userId)
        {
            TModel model = this.shoppingCartRepository.All()
                .Where(shoppingCart => shoppingCart.CustomerId == userId)
                .To<TModel>()
                .FirstOrDefault();

            if (model == null)
            {
                throw new ServiceException();
            }

            return model;
        }
    }
}