using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IProductTypeService
    {
        Task<bool> CreateAsync(string name);

        bool Exists(string name);

        IEnumerable<TModel> All<TModel>();
    }
}