using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Ioc;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.StartupConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _environment;
        private IContainer _applicationContainer;

        public Startup(IHostingEnvironment env)
        {
            _environment = env;
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());

            //if (env.IsDevelopment())
            //{
            //    builder.AddJsonFile($"appsettings.{Environment.UserName}.json");
            //}
            //else
            //{
                builder.AddJsonFile("appsettings.json");
            //}

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var authSettings = _config.GetConfigSection<AuthenticationSettings>();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = 524_288_000;
                x.MultipartBodyLengthLimit = 524_288_000;
                x.MultipartBoundaryLengthLimit = 524_288_000;
            });

            if (!authSettings.Enabled)
            {
                services.AddMvc(options =>
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                });
            }
            else
            {
                services.AddMvc();
            }

            services.AddSession();

            // Custom services
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
            services.AddAndConfigureDataAccess(_config);
            services.AddAndConfigureAuthorisation();
            services.AddMvc().AddControllersAsServices();
            services.AddAndConfigureAuthentication(authSettings);

            return ConfigureAutofac(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();

                app.UseSession();
                app.UseAuthentication();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private IServiceProvider ConfigureAutofac(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupConfigurations(_config);

            containerBuilder.RegisterModule<ServiceRegistrations>();
            containerBuilder.RegisterModule<AuthorizationHandlerRegistrations>();

            containerBuilder.Populate(services);
            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }
    }
}