using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Commons;
using Watchman.Discord.Integration.DevscordFramework;
<<<<<<< HEAD
using Watchman.DomainModel.Settings.Services;
=======
using Watchman.DomainModel.Commons.Calculators.Statistics;
using Watchman.DomainModel.Configuration.Services;
>>>>>>> master

namespace Watchman.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ResponsesService().SetGetResponsesFromDatabase(c.Resolve<IQueryBus>()))
                .As<ResponsesService>()
                .SingleInstance();

            builder.RegisterType<CustomCommandsLoader>()
                .As<ICustomCommandsLoader>()
                .SingleInstance();

            builder.RegisterType<CommandParser>()
                .As<CommandParser>()
                .SingleInstance();

            builder.RegisterType<ConfigurationService>()
                .As<IConfigurationService>()
                .SingleInstance();

            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                builder.RegisterAssemblyTypes(asm)
<<<<<<< HEAD
                    .Where(x => x.FullName.StartsWith("Watchman") || x.FullName.StartsWith("Devscord") || x.FullName.StartsWith("Statsman"))
=======
                    .Where(x => x.FullName.StartsWith("Watchman") || x.FullName.StartsWith("Devscord"))
                    .Where(x => x.GetConstructors().Any()) // todo: AutoFac v6.0 needs this line to work / maybe possible to remove in future when they'll fix it
>>>>>>> master
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
