using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;

namespace Metomarket.Services.Data
{
    public class ProductService : IProductService
    {
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IRepository<ProductType> productTypeRepository;

        public ProductService(
            IDeletableEntityRepository<Product> productRepository,
            IRepository<ProductType> productTypeRepository)
        {
            this.productRepository = productRepository;
            this.productTypeRepository = productTypeRepository;
        }

        public async Task<bool> CreateAsync(string name, decimal price, string imageUrl, int inStock, string typeId)
        {
            bool productTypeExists = this.productTypeRepository.All()
                .Where(pt => pt.Id == typeId)
                .FirstOrDefault() != null;

            if (!productTypeExists)
            {
                return false;
            }

            Product product = new Product
            {
                Name = name,
                Price = price,
                ImageUrl = imageUrl,
                InStock = inStock,
                TypeId = typeId,
            };

            await this.productRepository.AddAsync(product);
            await this.productRepository.SaveChangesAsync();

            return true;
        }
    }
}