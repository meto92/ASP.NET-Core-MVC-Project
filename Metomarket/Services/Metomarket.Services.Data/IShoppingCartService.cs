using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IShoppingCartService
    {
        Task<bool> AddOrderAsync(string userId, string orderId);

        TModel FindByUserId<TModel>(string userId);

        Task<bool> EmptyCartAsync(string userId, bool restoreProductQuantities = false);
    }
}