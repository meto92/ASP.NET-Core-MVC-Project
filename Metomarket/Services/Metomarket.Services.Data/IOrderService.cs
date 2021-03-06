﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IOrderService
    {
        Task<string> CreateAsync(string productId, string issuerId, int quantity);

        Task<bool> DeleteAsync(string id, string userId);

        Task<bool> CompleteOrdersAsync(IEnumerable<string> ids);

        int GetCount();
    }
}