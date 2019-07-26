using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using Metomarket.Data.Models;
using Metomarket.Services.Mapping;

namespace Metomarket.Web.ViewModels.Users
{
    public class UserViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> RoleIds { get; set; }

        public IEnumerable<string> RoleNames { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserViewModel>().ForMember(
                m => m.RoleIds,
                opt => opt.MapFrom(u => u.Roles.Select(r => r.RoleId)));
        }
    }
}