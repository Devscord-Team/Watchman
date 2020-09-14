using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using System.Collections.Generic;
using System.Reflection;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Commons;
using Watchman.Discord.Integration.DevscordFramework;
using Watchman.DomainModel.Commons.Calculators.Statistics;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ResponsesService().SetGetResponsesFromDatabase(c.Resolve<IQueryBus>()))
                .As<ResponsesService>()
                .SingleInstance();

            builder.RegisterType<StatisticsCalculator>()
                .As<IStatisticsCalculator>()
                .SingleInstance();

            builder.RegisterType<CustomCommandsLoader>()
                .As<ICustomCommandsLoader>()
                .SingleInstance();

            builder.RegisterType<CommandParser>()
                .As<CommandParser>()
                .SingleInstance();

            builder.RegisterType<ConfigurationService>()
                .As<IConfigurationService>()
                .InstancePerLifetimeScope();

            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                builder.RegisterAssemblyTypes(asm)
                    .Where(x => x.FullName.StartsWith("Watchman") || x.FullName.StartsWith("Devscord"))
                    .PreserveExistingDefaults()
                    .SingleInstance();

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
        }
    }
}
