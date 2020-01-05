using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;
using Watchman.Web.Areas.Responses.Services;
using Watchman.Web.Areas.Statistics.Services;

namespace Watchman.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(x => Register(x))) 
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //todo move to other class
        private static void Register(ContainerBuilder builder)
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                //TODO move to IoC namespace with modules, normal configuration etc
                builder.Register((c, p) => new MongoClient("mongodb://localhost:27017").GetDatabase("devscord"))
                    .As<IMongoDatabase>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<SessionFactory>()
                    .As<ISessionFactory>()
                    .InstancePerLifetimeScope();

                var asm = stack.Pop();

                if (asm.FullName.Contains("Watchman.DomainModel"))
                {
                    var handlers = asm.GetTypes()
                        .Where(type => typeof(IQueryHandler).IsAssignableFrom(type) || typeof(ICommandHandler).IsAssignableFrom(type)
                            && !type.IsInterface && !type.IsAbstract)
                        .ToList();

                    foreach (var handler in handlers)
                    {
                        builder.RegisterType(handler)
                            .As(handler.GetInterfaces().First())
                            .InstancePerLifetimeScope();
                    }
                }

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
                }
            }
            while (stack.Count > 0);


            builder.RegisterType<QueryBus>()
                .As<IQueryBus>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandBus>()
                .As<ICommandBus>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ResponsesService>()
                .As<ResponsesService>()
                .InstancePerLifetimeScope();            
            
            builder.RegisterType<StatisticsService>()
                .As<StatisticsService>()
                .InstancePerLifetimeScope();
        }
    }
}
