using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Services.Data
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly IRepository<ProductType> productTypeRepository;

        public ProductTypeService(IRepository<ProductType> productTypeRepository)
        {
            this.productTypeRepository = productTypeRepository;
        }

        public IEnumerable<TModel> All<TModel>()
            => this.productTypeRepository.AllAsNoTracking()
                .To<TModel>()
                .ToArray();

        public async Task<bool> CreateAsync(string name)
        {
            if (this.Exists(name))
            {
                return false;
            }

            ProductType productType = new ProductType
            {
                Name = name,
            };

            await this.productTypeRepository.AddAsync(productType);
            await this.productTypeRepository.SaveChangesAsync();

            return true;
        }

        public bool Exists(string name)
        {
            bool productTypeExists = this.productTypeRepository.AllAsNoTracking()
                .Where(pt => pt.Name == name)
                .FirstOrDefault() != null;

            return productTypeExists;
        }
    }
}