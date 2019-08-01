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
        private const string ProductTypeAlreadyExistsMessage = "Product type '{0}' already exists.";
        private const string ProductTypeNotFoundMessage = "Product type with ID '{0}' could not be found.";

        private readonly IRepository<ProductType> productTypeRepository;

        public ProductTypeService(IRepository<ProductType> productTypeRepository)
        {
            this.productTypeRepository = productTypeRepository;
        }

        public IEnumerable<TModel> All<TModel>()
            => this.productTypeRepository.AllAsNoTracking()
                .To<TModel>()
                .ToArray();

        public async Task<string> CreateAsync(string name)
        {
            if (this.Exists(name))
            {
                throw new ServiceException(string.Format(
                    ProductTypeAlreadyExistsMessage,
                    name));
            }

            ProductType productType = new ProductType
            {
                Name = name,
            };

            await this.productTypeRepository.AddAsync(productType);
            await this.productTypeRepository.SaveChangesAsync();

            return productType.Id;
        }

        public bool Exists(string name)
        {
            bool productTypeExists = this.productTypeRepository.AllAsNoTracking()
                .Where(pt => pt.Name == name)
                .FirstOrDefault() != null;

            return productTypeExists;
        }

        public TModel FindById<TModel>(string id)
        {
            TModel model = this.productTypeRepository.AllAsNoTracking()
                .Where(productType => productType.Id == id)
                .To<TModel>()
                .FirstOrDefault();

            if (model == null)
            {
                throw new ServiceException(string.Format(
                    ProductTypeNotFoundMessage,
                    id));
            }

            return model;
        }

        public int GetCount()
        {
            int count = this.productTypeRepository.All().Count();

            return count;
        }

        public async Task<bool> UpdateAsync(string id, string newName)
        {
            ProductType productType = this.productTypeRepository.All()
                .Where(pt => pt.Id == id)
                .FirstOrDefault();

            if (productType == null)
            {
                throw new ServiceException(string.Format(
                    ProductTypeNotFoundMessage,
                    id));
            }

            if (productType.Name == newName)
            {
                return false;
            }

            productType.Name = newName;

            this.productTypeRepository.Update(productType);
            await this.productTypeRepository.SaveChangesAsync();

            return true;
        }
    }
}