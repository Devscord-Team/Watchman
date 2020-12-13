using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using System.Collections.Generic;
using System.Reflection;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Commons;
using Watchman.Discord.Integration.DevscordFramework;
using Watchman.DomainModel.Commons.Calculators.Statistics;
using Watchman.DomainModel.Settings.Services;
using Devscord.DiscordFramework.Framework.Commands.Services;

namespace Watchman.Discord.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ResponsesService().SetGetResponsesFromDatabase(c.Resolve<IQueryBus>()))
                .As<ResponsesService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StatisticsCalculator>()
                .As<IStatisticsCalculator>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomCommandsLoader>()
                .As<ICustomCommandsLoader>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandParser>()
                .As<CommandParser>()
                .SingleInstance();

            builder.RegisterType<ConfigurationService>()
                .As<IConfigurationService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandsContainer>()
                .As<ICommandsContainer>()
                .SingleInstance();

            builder.RegisterType<CommandMethodValidator>()
                .As<ICommandMethodValidator>()
                .SingleInstance();

            builder.RegisterType<BotCommandsService>()
                .As<IBotCommandsService>()
                .SingleInstance();

            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                builder.RegisterAssemblyTypes(asm)
                    .PreserveExistingDefaults()
                    .InstancePerLifetimeScope();

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
