using Metomarket.Data.Models;
using Metomarket.Web.Tests.Mocks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyTested.AspNetCore.Mvc;

namespace Metomarket.Web.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration)
            : base(configuration)
        {
        }

        public void ConfigureTestServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.ReplaceDbContext();
            services.ReplaceScoped<UserManager<ApplicationUser>, UserManagerMock<ApplicationUser>>();
        }
    }
}