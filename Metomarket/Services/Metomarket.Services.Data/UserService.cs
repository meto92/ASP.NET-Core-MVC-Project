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
        private const string UserNotFoundMessage = "User with ID '{0}' could not be found.";

        private readonly UserManager<ApplicationUser> userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            bool exists = await this.userManager.FindByIdAsync(userId) != null;

            return exists;
        }

        public async Task<bool> AddAdministratorRoleAsync(string userId)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ServiceException(string.Format(
                    UserNotFoundMessage,
                    userId));
            }

            bool isAlreadyAdmin = await this.userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (isAlreadyAdmin)
            {
                return false;
            }

            await this.userManager.AddToRoleAsync(
                user,
                GlobalConstants.AdministratorRoleName);

            return true;
        }

        public async Task<bool> RemoveAdministratorRoleAsync(string userId)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ServiceException(string.Format(
                     UserNotFoundMessage,
                     userId));
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
        {
            TModel model = Mapper.Map<TModel>(await this.userManager.FindByIdAsync(userId));

            if (model == null)
            {
                throw new ServiceException(string.Format(
                    UserNotFoundMessage,
                    userId));
            }

            return model;
        }

        public async Task<bool> IsAdminAsync(string userId)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ServiceException(string.Format(
                    UserNotFoundMessage,
                    userId));
            }

            bool isAdmin = await this.userManager
                .IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            return isAdmin;
        }

        public int GetCount()
        {
            int count = this.userManager.Users.Count();

            return count;
        }
    }
}