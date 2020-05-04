using Autofac;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using System;
using System.Collections.Generic;
using System.Reflection;
using Watchman.Cqrs;
using Watchman.Discord;
using Watchman.Discord.Areas.Commons;
using Watchman.DomainModel.Commons.Calculators.Statistics;

namespace Watchman.IoC.Modules
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

            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                builder.RegisterAssemblyTypes(asm)
                    .Where(x => x.FullName.StartsWith("Watchman") || x.FullName.StartsWith("Devscord"))
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
