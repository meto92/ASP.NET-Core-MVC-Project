using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IProductService
    {
        Task<string> CreateAsync(string name, decimal price, string imageUrl, int inStock, string typeId);

        bool Exists(string id);

        TModel FindById<TModel>(string id);

        IEnumerable<TModel> All<TModel>();

        Task<bool> UpdateAsync(string id, string newName, decimal newPrice, string newImageUrl, int quantityToAdd);

        Task<bool> Delete(string id);

        Task<bool> ReduceQuantityAsync(string id, int quantity);

        Task<bool> AddQuantityAsync(string id, int quantity);

        int GetCount();
    }
}