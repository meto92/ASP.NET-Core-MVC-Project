using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IUserService
    {
        Task<bool> ExistsAsync(string userId);

        Task<bool> AddAdministratorRoleAsync(string userId);

        Task<bool> RemoveAdministratorRoleAsync(string userId);

        IEnumerable<TModel> All<TModel>();

        Task<TModel> FindByIdAsync<TModel>(string userId);
    }
}