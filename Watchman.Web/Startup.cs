using System.Collections.Generic;
using System.Net;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Watchman.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
            services.Configure<ForwardedHeadersOptions>(options => options.KnownProxies.Add(IPAddress.Parse("10.0.0.100")));
            //services.AddAuthentication()
            //    .AddDiscord(x =>
            //    {
            //        //TODO - not working, but now it is not a problem
            //        x.ClientId = this.Configuration["Discord:AppId"];
            //        x.ClientSecret = this.Configuration["Discord:AppSecret"];
            //        x.Validate();
            //        x.Scope.Add("email");
            //        x.Validate();
            //    });

            services.AddControllersWithViews();

#if DEBUG
            services.AddHangfire(x => x.UseMemoryStorage());
#else
            services.AddHangfire(config =>
            {
                var storageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    }
                };
                config.UseMongoStorage(this.Configuration.GetConnectionString("Mongo"), storageOptions);
            });
#endif
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new List<IDashboardAuthorizationFilter> { new HangfireDashboardFilter() } });
            app.UseHangfireServer();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }

        public class HangfireDashboardFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize([NotNull] DashboardContext context) //TODO check auth on production
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
