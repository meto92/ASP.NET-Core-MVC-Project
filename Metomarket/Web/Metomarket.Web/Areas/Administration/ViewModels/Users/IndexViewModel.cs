using System.Collections.Generic;

namespace Metomarket.Web.Areas.Administration.ViewModels.Users
{
    public class IndexViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
    }
}