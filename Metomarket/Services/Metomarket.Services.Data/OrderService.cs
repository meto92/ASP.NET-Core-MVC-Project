using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;

namespace Metomarket.Services.Data
{
    public class OrderService : IOrderService
    {
        private const string InvalidQuantityMessage = "Quantity must be greater than zero.";
        private const string ProductNotFoundMessage = "Product not found.";
        private const string InsufficientProductQuantityMessage = "Sorry. We don't have the requested quantity.";

        private readonly IDeletableEntityRepository<Order> orderRepository;
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IUserService userService;

        public OrderService(
            IDeletableEntityRepository<Order> orderRepository,
            IDeletableEntityRepository<Product> poductRepository,
            IUserService userService)
        {
            this.orderRepository = orderRepository;
            this.productRepository = poductRepository;
            this.userService = userService;
        }

        public async Task<string> CreateAsync(string productId, string issuerId, int quantity)
        {
            if (quantity < 1)
            {
                throw new ServiceException(InvalidQuantityMessage);
            }

            Product product = this.productRepository.All()
                .Where(p => p.Id == productId)
                .FirstOrDefault();

            if (product == null)
            {
                throw new ServiceException(ProductNotFoundMessage);
            }

            bool userExists = await this.userService.ExistsAsync(issuerId);

            if (!userExists)
            {
                throw new ServiceException();
            }

            if (product.InStock < quantity)
            {
                throw new ServiceException(InsufficientProductQuantityMessage);
            }

            Order order = new Order
            {
                ProductId = productId,
                Quantity = quantity,
                IssuerId = issuerId,
            };

            product.InStock -= quantity;

            this.productRepository.Update(product);
            await this.orderRepository.AddAsync(order);
            await this.orderRepository.SaveChangesAsync();

            return order.Id;
        }
    }
}