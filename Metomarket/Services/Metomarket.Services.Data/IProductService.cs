﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface IProductService
    {
        Task<bool> CreateAsync(string name, decimal price, string imageUrl, int inStock, string typeId);
    }
}