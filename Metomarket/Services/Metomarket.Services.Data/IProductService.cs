using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IProductService
    {
        Task<bool> CreateAsync(string name, decimal price, string imageUrl, int inStock, string typeId);

        bool Exists(string id);

        TModel FindById<TModel>(string id);

        IEnumerable<TModel> All<TModel>();

        Task<bool> Update(string id, string newName, decimal newPrice, string newImageUrl, int quantityToAdd);

        Task<bool> Delete(string id);
    }
}