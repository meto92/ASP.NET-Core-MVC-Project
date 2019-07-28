using System.Threading.Tasks;

using Metomarket.Common;
using Metomarket.Services.Data;
using Metomarket.Web.ViewModels.Users;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Administration.Controllers
{
    public class UsersController : AdministrationController
    {
        private readonly IUserService userService;
        private readonly IRoleService roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }

        public IActionResult Index()
        {
            AdminUsersIndexViewModel model = new AdminUsersIndexViewModel
            {
                Users = this.userService.All<UserViewModel>(),
            };

            foreach (var user in model.Users)
            {
                user.RoleNames = this.roleService.GetRoleNamesByIds(user.RoleIds);
            }

            return this.View(model);
        }

        public async Task<IActionResult> Promote(string id)
        {
            await this.userService.AddAdministratorRoleAsync(id);

            return this.RedirectToIndex();
        }

        public async Task<IActionResult> Demote(string id)
        {
            bool exists = await this.userService.ExistsAsync(id);

            if (!exists)
            {
                return this.RedirectToIndex();
            }

            UserViewModel user = await this.userService.FindByIdAsync<UserViewModel>(id);

            bool isRootAdmin = user.Username == GlobalConstants.RootAdministratorUsername
                && user.Email == GlobalConstants.RootAdministratorEmail;

            if (isRootAdmin)
            {
                return this.RedirectToIndex();
            }

            await this.userService.RemoveAdministratorRoleAsync(id);

            return this.RedirectToIndex();
        }

        private IActionResult RedirectToIndex()
            => this.RedirectToAction(nameof(this.Index));
    }
}