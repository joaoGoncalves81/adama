﻿using ApplicationCore;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using AutoMapper;
using DamaWeb.Interfaces;
using DamaWeb.Services;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DamaWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; set; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DamaContext>(c =>
                c.UseMySql(Configuration.GetConnectionString("DamaShopConnection"), MariaDbServerVersion.LatestSupportedServerVersion));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnection"), MariaDbServerVersion.LatestSupportedServerVersion));


            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DamaContext>(options =>
               options.UseMySql(Configuration.GetConnectionString("DamaShopConnection"), MariaDbServerVersion.LatestSupportedServerVersion));

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnection"), MariaDbServerVersion.LatestSupportedServerVersion));


            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            });

            // The Tempdata provider cookie is not essential. Make it essential
            // so Tempdata is functional when tracking is disabled.
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "damanojornalApp";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = "/Account/Signin";
                options.LogoutPath = "/Account/Signout";
                options.Cookie.IsEssential = true;
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<ICatalogService, CachedCatalogService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IBasketViewModelService, BasketViewModelService>();
            services.AddScoped<ICustomizeViewModelService, CustomizeViewModelService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<CatalogService>();
            services.AddScoped<IShopService, CacheShopService>();
            services.AddScoped<ShopService>();
            services.Configure<CatalogSettings>(Configuration);
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.AddSingleton<IUriComposer>(new UriComposer(Configuration.Get<CatalogSettings>()));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            //services.AddScoped<IMailChimpService, MailChimpService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ISageService, SageService>();
            services.AddScoped<IAuthConfigRepository, AuthConfigRepository>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddHttpClient<IMailChimpService, MailChimpService>(options =>
            {
                options.BaseAddress = new Uri(Configuration["MailChimpBaseUrl"]);
                options.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Configuration["MailChimpBasicAuth"]);
            });

            // Add memory cache services
            services.AddMemoryCache();

            services.AddApplicationInsightsTelemetry();

            var builder = services.AddRazorPages(options =>
                {
                    options.Conventions.AuthorizeFolder("/Order");
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                    options.Conventions.AddPageRoute("/Basket/Index", "carrinho");
                    options.Conventions.AddPageRoute("/Order/Index", "as-minhas-encomendas");
                    options.Conventions.AddPageRoute("/Order/Detail", "detalhe-da-encomenda/{orderId}");
                    options.Conventions.AddPageRoute("/Basket/Checkout", "carrinho/encomendar");
                    options.Conventions.AddPageRoute("/Basket/CheckoutStep2", "carrinho/encomendar/passo2");
                    //options.Conventions.AddPageRoute("/Category/Redirect", "{id}/");
                    options.Conventions.AddPageRoute("/Category/Index", "categoria/{id}/");
                    options.Conventions.AddPageRoute("/Category/Type/Redirect", "{cat}/{type}");
                    options.Conventions.AddPageRoute("/Category/Type/Index", "categoria/{cat}/{type}");
                    options.Conventions.AddPageRoute("/Product/Index", "produto/{id}");
                    options.Conventions.AddPageRoute("/Product", "{id}");
                    options.Conventions.AddPageRoute("/Tag/Index", "tag/{tagName}");
                    options.Conventions.AddPageRoute("/Search/Index", "procurar/{q?}");
                    options.Conventions.AddPageRoute("/Customize/Index", "personalizar/");
                    options.Conventions.AddPageRoute("/Customize/Step2", "personalizar/passo2");
                    options.Conventions.AddPageRoute("/Customize/Result", "personalizar/resultado");
                    options.Conventions.AddPageRoute("/Privacy", "privacidade");
                    options.Conventions.AddPageRoute("/NotFound", "pagina-nao-encontrada");
                    options.Conventions.AddPageRoute("/Error", "erro");
                    options.Conventions.AddPageRoute("/Account/Signin", "conta/entrar");
                    options.Conventions.AddPageRoute("/Account/ConfirmEmail", "conta/confirmar-email");
                    options.Conventions.AddPageRoute("/Account/ForgotPassword", "conta/esqueceu-password");
                    options.Conventions.AddPageRoute("/Account/ForgotPasswordConfirmation", "conta/confirmacao-password");
                    options.Conventions.AddPageRoute("/Account/ResetPassword", "conta/recuperar-password");
                    options.Conventions.AddPageRoute("/Account/ResetPasswordConfirmation", "conta/confirmacao-recuperacao-password");
                    options.Conventions.AddPageRoute("/Account/Manage/Index", "conta/perfil");
                })
                .AddControllersAsServices();

#if DEBUG
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UsePathBase("/loja");
                app.UseExceptionHandler(options =>
                {
                options.Run(
                    async context =>
                    {
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (!context.Request.Path.ToString().Contains("Error") && ex != null)
                        {
                            var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.Source}<hr />{context.Request.Path}<br />";
                            err += $"QueryString: {context.Request.QueryString}<hr />";

                            err += $"Stack Trace<hr />{ex.Error.StackTrace.Replace(Environment.NewLine, "<br />")}";
                            if (ex.Error.InnerException != null)
                                err +=
                                    $"Inner Exception<hr />{ex.Error.InnerException?.Message.Replace(Environment.NewLine, "<br />")}";
                            // This bit here to check for a form collection!
                            if (context.Request.HasFormContentType && context.Request.Form.Any())
                            {
                                err += "<table border=\"1\"><tr><td colspan=\"2\">Form collection:</td></tr>";
                                foreach (var form in context.Request.Form)
                                {
                                    err += $"<tr><td>{form.Key}</td><td>{form.Value}</td></tr>";
                                }
                                err += "</table>";
                            }
                            try
                            {
                                var emailSender = app.ApplicationServices.GetRequiredService<IEmailSender>();
                                await emailSender.SendEmailAsync(
                                    Configuration["Email:FromInfoEmail"],
                                    Configuration["Email:SupportEmail"],
                                    "Dama no Jornal - Ocorreu um erro na aplicação", err);
                            }
                            catch (Exception)
                            {
                                //Ignore
                            }

                            context.Response.Redirect("/loja/Error");
                        }
                    });
            });
                //app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/NotFound";
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
