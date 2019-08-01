using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Services.Data
{
    public class ProductService : IProductService
    {
        private const string ProductNotFoundMessage = "Product with id {0} could not be found.";
        private const string ProductTypeNotFoundMessage = "Product type with id {0} could not be found.";
        private const string InsufficientProductQuantityMessage = "Sorry. We don't have the requested quantity.";

        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IRepository<ProductType> productTypeRepository;

        public ProductService(
            IDeletableEntityRepository<Product> productRepository,
            IRepository<ProductType> productTypeRepository)
        {
            this.productRepository = productRepository;
            this.productTypeRepository = productTypeRepository;
        }

        public async Task<bool> AddQuantityAsync(string id, int quantity)
        {
            Product product = this.productRepository.All()
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                this.ThrowWithNotFoundMessage(id);
            }

            product.InStock = this.GetNewQuantity(product.InStock, quantity);

            this.productRepository.Update(product);
            await this.productRepository.SaveChangesAsync();

            return true;
        }

        public IEnumerable<TModel> All<TModel>()
        {
            IEnumerable<TModel> models = this.productRepository.AllAsNoTracking()
                .To<TModel>()
                .ToArray();

            return models;
        }

        public async Task<string> CreateAsync(string name, decimal price, string imageUrl, int inStock, string typeId)
        {
            bool productTypeExists = this.productTypeRepository.All()
                .Where(productType => productType.Id == typeId)
                .FirstOrDefault() != null;

            if (!productTypeExists)
            {
                throw new ServiceException(string.Format(
                    ProductTypeNotFoundMessage,
                    typeId));
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

            return product.Id;
        }

        public async Task<bool> Delete(string id)
        {
            Product product = this.productRepository.All()
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                this.ThrowWithNotFoundMessage(id);
            }

            if (product.IsDeleted)
            {
                return false;
            }

            this.productRepository.Delete(product);
            await this.productRepository.SaveChangesAsync();

            return true;
        }

        public bool Exists(string id)
        {
            bool productExists = this.productRepository.All()
                .Where(product => product.Id == id)
                .FirstOrDefault() != null;

            return productExists;
        }

        public TModel FindById<TModel>(string id)
        {
            TModel model = this.productRepository.All()
                .Where(p => p.Id == id)
                .To<TModel>()
                .FirstOrDefault();

            if (model == null)
            {
                this.ThrowWithNotFoundMessage(id);
            }

            return model;
        }

        public int GetCount()
        {
            int count = this.productRepository.All().Count();

            return count;
        }

        public async Task<bool> ReduceQuantityAsync(string id, int quantity)
        {
            Product product = this.productRepository.All()
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                this.ThrowWithNotFoundMessage(id);
            }

            if (product.InStock < quantity)
            {
                throw new ServiceException(InsufficientProductQuantityMessage);
            }

            product.InStock -= quantity;

            this.productRepository.Update(product);
            await this.productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(string id, string newName, decimal newPrice, string newImageUrl, int quantityToAdd)
        {
            Product product = this.productRepository.All()
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                this.ThrowWithNotFoundMessage(id);
            }

            if (product.Name == newName
                && product.Price == newPrice
                && product.ImageUrl == newImageUrl
                && quantityToAdd == 0)
            {
                return false;
            }

            product.Name = newName;
            product.Price = newPrice;
            product.ImageUrl = newImageUrl;
            product.InStock = this.GetNewQuantity(product.InStock, quantityToAdd);

            this.productRepository.Update(product);
            await this.productRepository.SaveChangesAsync();

            return true;
        }

        private int GetNewQuantity(int inStock, int quantityToAdd)
        {
            int result = Math.Max(
                inStock,
                Math.Max(
                    inStock + quantityToAdd,
                    Math.Min(quantityToAdd, int.MaxValue)));

            return result;
        }

        private void ThrowWithNotFoundMessage(string id)
        {
            throw new ServiceException(string.Format(
                ProductNotFoundMessage,
                id));
        }
    }
}