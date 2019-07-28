using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Services.Data
{
    public class ShoppingCartService : IShoppingCartService
    {
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
                return false;

                // TODO: throw exception
            }

            Order order = this.orderRepository.All()
                .Where(o => o.Id == orderId)
                .FirstOrDefault();

            if (order == null)
            {
                return false;

                // TODO: throw exception
            }

            shoppingCart.Orders.Add(order);

            await this.shoppingCartRepository.SaveChangesAsync();

            return true;
        }

        public TModel FindByUserId<TModel>(string userId)
        {
            TModel model = this.shoppingCartRepository.All()
                .Where(shoppingCart => shoppingCart.CustomerId == userId)
                .To<TModel>()
                .FirstOrDefault();

            return model;
        }
    }
}