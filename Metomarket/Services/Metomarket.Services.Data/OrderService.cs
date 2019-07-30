using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;

namespace Metomarket.Services.Data
{
    public class OrderService : IOrderService
    {
        private const string InvalidQuantityMessage = "Quantity must be greater than zero.";
        private const string OrderNotFoundMessage = "Order with id {0} could not be found.";
        private const string UnauthorizedToDeleteOrderMessage = "You don't have permission to delete this order.";

        private readonly IDeletableEntityRepository<Order> orderRepository;
        private readonly IProductService productService;
        private readonly IUserService userService;

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IProductService productService,
            IUserService userService)
        {
            this.orderRepository = orderRepository;
            this.productService = productService;
            this.userService = userService;
        }

        public async Task<bool> CompleteOrdersAsync(IEnumerable<string> ids)
        {
            IEnumerable<Order> orders = this.orderRepository.All()
                .Where(order => ids.Contains(order.Id))
                .ToArray();

            if (orders.Any(order => order.IsCompleted))
            {
                return false;
            }

            foreach (Order order in orders)
            {
                order.IsCompleted = true;
                this.orderRepository.Update(order);
            }

            await this.orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<string> CreateAsync(string productId, string issuerId, int quantity)
        {
            if (quantity < 1)
            {
                throw new ServiceException(InvalidQuantityMessage);
            }

            bool userExists = await this.userService.ExistsAsync(issuerId);

            if (!userExists)
            {
                throw new ServiceException();
            }

            await this.productService.ReduceQuantityAsync(productId, quantity);

            Order order = new Order
            {
                ProductId = productId,
                Quantity = quantity,
                IssuerId = issuerId,
            };

            await this.orderRepository.AddAsync(order);
            await this.orderRepository.SaveChangesAsync();

            return order.Id;
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            Order order = this.orderRepository.All()
                .Where(o => o.Id == id)
                .FirstOrDefault();

            if (order == null)
            {
                throw new ServiceException(string.Format(
                    OrderNotFoundMessage,
                    id));
            }

            bool isAdmin = await this.userService.IsAdminAsync(userId);

            if (order.IssuerId != userId && !isAdmin)
            {
                throw new ServiceException(UnauthorizedToDeleteOrderMessage);
            }

            await this.productService.AddQuantityAsync(order.ProductId, order.Quantity);

            this.orderRepository.Delete(order);
            await this.orderRepository.SaveChangesAsync();

            return true;
        }
    }
}