using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backoffice.Services;
using Infrastructure.Data;
using Infrastructure.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using ApplicationCore.Interfaces;
using Backoffice.Interfaces;
using Infrastructure.Services;
using ApplicationCore;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Logging;

namespace Backoffice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppIdentityDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddDbContext<DamaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DamaConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireUppercase = false;
                })
               .AddEntityFrameworkStores<AppIdentityDbContext>()
               .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
            });


            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {                    
                    options.Conventions.AuthorizeFolder("/Account/Manage", "RequireAdministratorRole");
                    options.Conventions.AuthorizePage("/Account/Logout", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/Category", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/Illustrations", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/IllustrationsTypes", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/Products", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/ProductType", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/ShopConfig", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/Orders", "RequireAdministratorRole");
                    options.Conventions.AuthorizeFolder("/Sage", "RequireAdministratorRole");
                    options.Conventions.AuthorizePage("/Index", "RequireAdministratorRole");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);            

            services.AddAutoMapper();

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.Configure<BackofficeSettings>(Configuration);
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped<IBackofficeService, BackofficeService>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton<IEmailSender>(new EmailSender(Configuration.Get<BackofficeSettings>()));
            services.AddScoped<ISageService, SageService>();
            services.AddScoped<IAuthConfigRepository, AuthConfigRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-PT"),
                // Formatting numbers, dates, etc.
                SupportedCultures = new[] { new CultureInfo("pt-PT") },
                // UI strings that we have localized.
                SupportedUICultures = new[] { new CultureInfo("pt-PT") }
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
