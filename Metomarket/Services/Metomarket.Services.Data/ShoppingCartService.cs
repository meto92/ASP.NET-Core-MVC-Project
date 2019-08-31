using System.Linq;
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
        private const string CustomerShoppingCartNotFound = "User with ID '{0}' does not have a shopping cart.";

        private readonly IRepository<ShoppingCart> shoppingCartRepository;
        private readonly IRepository<Order> orderRepository;
        private readonly IProductService productService;

        public ShoppingCartService(
            IRepository<ShoppingCart> shoppingCartRepository,
            IRepository<Order> orderRepository,
            IProductService productService)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.orderRepository = orderRepository;
            this.productService = productService;
        }

        public async Task<bool> AddOrderAsync(string userId, string orderId)
        {
            ShoppingCart shoppingCart = this.shoppingCartRepository.All()
                .Where(cart => cart.CustomerId == userId)
                .FirstOrDefault();

            if (shoppingCart == null)
            {
                this.ThrowShoppingCartNotFound(userId);
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

        public async Task<bool> EmptyCartAsync(string userId, bool restoreProductQuantities = false)
        {
            ShoppingCart shoppingCart = this.shoppingCartRepository.All()
                .Include(cart => cart.Orders)
                .Where(cart => cart.CustomerId == userId)
                .FirstOrDefault();

            if (shoppingCart == null)
            {
                this.ThrowShoppingCartNotFound(userId);
            }

            if (restoreProductQuantities)
            {
                foreach (Order order in shoppingCart.Orders)
                {
                    await this.productService
                        .AddQuantityAsync(order.ProductId, order.Quantity);
                }
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
                this.ThrowShoppingCartNotFound(userId);
            }

            return model;
        }

        private void ThrowShoppingCartNotFound(string userId)
        {
            throw new ServiceException(string.Format(
                    CustomerShoppingCartNotFound,
                    userId));
        }
    }
}