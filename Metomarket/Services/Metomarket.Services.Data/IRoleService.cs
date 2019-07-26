using System.Collections.Generic;

namespace Metomarket.Services.Data
{
    public interface IRoleService
    {
        IEnumerable<string> GetRoleNamesByIds(IEnumerable<string> roleIds);
    }
}