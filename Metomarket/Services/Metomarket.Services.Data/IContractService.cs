using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IContractService
    {
        Task<bool> CreateAsync(
            string userId,
            string creditCompanyId,
            IEnumerable<string> orderIds,
            decimal total,
            string creditCardNumber,
            int periodInMonths);
    }
}