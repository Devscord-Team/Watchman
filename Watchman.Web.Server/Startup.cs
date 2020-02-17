using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Watchman.Integrations.MongoDB;

namespace Watchman.Web.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            MongoConfiguration.Initialize();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAny",
            //        x =>
            //        {
            //            x.AllowAnyOrigin();
            //            x.AllowAnyMethod();
            //            x.AllowAnyHeader();
            //            x.AllowCredentials();
            //        });
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Watchman API", Description = "Watchman management tool", Version = "Alpha 1.0" });
            });

            services.AddControllers();
            services.AddMvc()
                .AddNewtonsoftJson(options => 
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Watchman API");
                c.RoutePrefix = "";
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            //app.UseCors("AllowAny");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
