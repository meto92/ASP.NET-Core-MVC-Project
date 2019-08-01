using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IProductTypeService
    {
        Task<string> CreateAsync(string name);

        bool Exists(string name);

        IEnumerable<TModel> All<TModel>();

        TModel FindById<TModel>(string id);

        Task<bool> UpdateAsync(string id, string newName);

        int GetCount();
    }
}