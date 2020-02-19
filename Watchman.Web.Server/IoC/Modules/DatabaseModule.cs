using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.Web.Server.IoC.Modules
{
    public class DatabaseModule : Autofac.Module
    {
        private readonly IConfiguration configuration;

        public DatabaseModule(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(DatabaseModule)
                .GetTypeInfo()
                .Assembly;

            builder.Register((c, p) => new MongoClient("mongodb://localhost:27017").GetDatabase("devscord"))
                .As<IMongoDatabase>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SessionFactory>()
                .As<ISessionFactory>()
                .InstancePerLifetimeScope();
        }
    }
}
