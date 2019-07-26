using System;
using System.Collections.Generic;
using System.Linq;

using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;

namespace Metomarket.Services.Data
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public IEnumerable<string> GetRoleNamesByIds(IEnumerable<string> roleIds)
            => this.roleManager.Roles
                .Where(role => roleIds.Contains(role.Id))
                .Select(role => role.Name)
                .ToArray();
    }
}