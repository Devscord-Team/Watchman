using Autofac;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Commons;

namespace Watchman.Discord.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ResponsesService().SetGetResponsesFromDatabase(c.Resolve<IQueryBus>()))
                .As<ResponsesService>()
                .InstancePerLifetimeScope();


            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                builder.RegisterAssemblyTypes(asm)
                    .Where(x => x.Name.EndsWith("Service") || x.Name.EndsWith("Factory"))
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
