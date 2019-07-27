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
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IRepository<ProductType> productTypeRepository;

        public ProductService(
            IDeletableEntityRepository<Product> productRepository,
            IRepository<ProductType> productTypeRepository)
        {
            this.productRepository = productRepository;
            this.productTypeRepository = productTypeRepository;
        }

        public IEnumerable<TModel> All<TModel>()
        {
            IEnumerable<TModel> models = this.productRepository.AllAsNoTracking()
                .To<TModel>()
                .ToArray();

            return models;
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

        public async Task<bool> Delete(string id)
        {
            Product product = this.productRepository.All()
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
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

            return model;
        }

        public async Task<bool> Update(string id, string newName, decimal newPrice, string newImageUrl, int quantityToAdd)
        {
            Product product = this.productRepository.All()
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                return false;
            }

            product.Name = newName;
            product.Price = newPrice;
            product.ImageUrl = newImageUrl;
            product.InStock = Math.Max(
                product.InStock,
                Math.Max(
                    product.InStock + quantityToAdd,
                    Math.Min(quantityToAdd, int.MaxValue)));

            this.productRepository.Update(product);
            await this.productRepository.SaveChangesAsync();

            return true;
        }
    }
}