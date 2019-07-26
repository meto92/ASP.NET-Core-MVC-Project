using System.Collections.Generic;

namespace Metomarket.Web.ViewModels.Users
{
    public class AdminUsersIndexViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
    }
}