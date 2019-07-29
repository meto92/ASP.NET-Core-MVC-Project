using System;
using System.Threading.Tasks;

namespace Metomarket.Services.Data
{
    public interface ICreditCompanyService
    {
        Task<bool> CreateAsync(string name, DateTime activeSince);
    }
}