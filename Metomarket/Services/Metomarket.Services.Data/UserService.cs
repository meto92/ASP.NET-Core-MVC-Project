﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Metomarket.Common;
using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

using Microsoft.AspNetCore.Identity;

namespace Metomarket.Services.Data
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<bool> Exists(string userId)
        {
            bool exists = await this.userManager.FindByIdAsync(userId) != null;

            return exists;
        }

        public async Task<bool> AddAdministratorRoleAsync(string userId)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            bool isAlreadyAdmin = await this.userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (isAlreadyAdmin)
            {
                return false;
            }

            await this.userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);

            return true;
        }

        public async Task<bool> RemoveAdministratorRoleAsync(string userId)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            bool isAdmin = await this.userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (!isAdmin)
            {
                return false;
            }

            await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.AdministratorRoleName);

            return true;
        }

        public IEnumerable<TModel> All<TModel>()
            => this.userManager.Users.To<TModel>().ToArray();

        public async Task<TModel> FindByIdAsync<TModel>(string userId)
            => Mapper.Map<TModel>(await this.userManager.FindByIdAsync(userId));
    }
}