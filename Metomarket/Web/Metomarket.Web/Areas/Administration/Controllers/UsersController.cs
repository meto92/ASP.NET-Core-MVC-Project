using Metomarket.Web.Areas.Administration.ViewModels.Users;

using Microsoft.AspNetCore.Mvc;

namespace Metomarket.Web.Areas.Administration.Controllers
{
    public class UsersController : AdministrationController
    {
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel
            {
                Users = new UserViewModel[]
                {
                    new UserViewModel
                    {
                        Id = "1",
                        Username = "user1",
                        Email = "email1",
                        Roles = new string[0],
                    },
                    new UserViewModel
                    {
                        Id = "2",
                        Username = "user2",
                        Email = "email2",
                        Roles = new string[] { "Admin" },
                    },
                },
            };

            return this.View(model);
        }

        public IActionResult Promote(string id)
        {
            return this.Content(id);
        }

        public IActionResult Demote(string id)
        {
            return this.Content(id);
        }
    }
}