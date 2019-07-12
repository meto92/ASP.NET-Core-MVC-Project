using System;
using System.Threading.Tasks;

namespace Metomarket.Data.Common
{
    public interface IDbQueryRunner : IDisposable
    {
        Task RunQueryAsync(string query, params object[] parameters);
    }
}