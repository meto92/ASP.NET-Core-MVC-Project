using System;
using System.Linq;
using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Metomarket.Data.Seeding
{
    internal class RootAdministratorSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (userManager.Users.Count() > 0)
            {
                return;
            }

            ApplicationUser rootAdmin = new ApplicationUser
            {
                UserName = GlobalConstants.RootAdministratorUsername,
                Email = GlobalConstants.RootAdministratorEmail,
                EmailConfirmed = true,
                Address = GlobalConstants.RootAdministratorAddress,
            };

            RootAdministratorOptions rootAdministratorOptions = serviceProvider.GetRequiredService<IOptions<RootAdministratorOptions>>().Value;

            IdentityResult createResult = await userManager.CreateAsync(rootAdmin, rootAdministratorOptions.RootAdministratorPassword);

            if (!createResult.Succeeded)
            {
                throw new Exception(string.Join(Environment.NewLine, createResult.Errors.Select(e => e.Description)));
            }

            IdentityResult addToRoleResult = await userManager.AddToRoleAsync(rootAdmin, GlobalConstants.AdministratorRoleName);

            if (!addToRoleResult.Succeeded)
            {
                throw new Exception(string.Join(Environment.NewLine, addToRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}