using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface ICreditCompanyService
    {
        Task<string> CreateAsync(string name, DateTime activeSince);

        IEnumerable<TModel> All<TModel>();

        bool Exists(string id);

        int GetCount();
    }
}