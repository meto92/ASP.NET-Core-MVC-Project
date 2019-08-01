using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IContractService
    {
        Task<string> CreateAsync(
            string userId,
            string creditCompanyId,
            IEnumerable<string> orderIds,
            decimal total,
            string creditCardNumber,
            int periodInMonths);

        int GetCount();
    }
}