using System.Reflection;

using Metomarket.Data;
using Metomarket.Data.Common;
using Metomarket.Data.Common.Repositories;
using Metomarket.Data.Models;
using Metomarket.Data.Repositories;
using Metomarket.Data.Seeding;
using Metomarket.Services.Data;
using Metomarket.Services.Mapping;
using Metomarket.Services.Messaging;
using Metomarket.Services.Messaging.SendGrid;
using Metomarket.Web.Hubs;
using Metomarket.Web.Infrastructure.ComponentViewModels.ProductTypes;
using Metomarket.Web.Infrastructure.Filters;
using Metomarket.Web.MLModels;
using Metomarket.Web.ViewModels;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;

namespace Metomarket.Web
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Framework services
            // TODO: Add pooling when this bug is fixed: https://github.com/aspnet/EntityFrameworkCore/issues/9741
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;

                    options.User.RequireUniqueEmail = true;
                    //options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4);

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = this.configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = this.configuration["Authentication:Facebook:AppSecret"];
                });

            services.AddSignalR();
            services.AddPredictionEnginePool<ModelInput, ModelOutput>()
                .FromFile("MLModels/MLModel.zip");

            services.AddMemoryCache();

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(typeof(ExceptionFilter));
                    options.Filters.Add(typeof(AutoValidateAntiforgeryTokenAttribute));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Identity/Account/Login";
                    options.LogoutPath = "/Identity/Account/Logout";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                    options.ConsentCookie.Name = ".AspNetCore.ConsentCookie";
                });

            services.AddSingleton(this.configuration);

            // Identity stores
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // SendGrid auth
            services.Configure<AuthSendGridOptions>(this.configuration);

            services.Configure<RootAdministratorOptions>(this.configuration);

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            //services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<ISmsSender, NullMessageSender>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IProductTypeService, ProductTypeService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<ICreditCompanyService, CreditCompanyService>();
            services.AddTransient<IContractService, ContractService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(ProductTypeOptionViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<DashboardHub>("/dashboard");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}