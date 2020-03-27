using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Watchman.Cqrs;

namespace Watchman.Discord.Ioc.Modules
{
    public class CommandModule : Autofac.Module
    { 
        protected override void Load(ContainerBuilder builder)
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                var handlers = asm.GetTypes()
                    .Where(type => typeof(ICommandHandler).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .ToList();

                foreach (var handler in handlers)
                {
                    builder.RegisterType(handler)
                        .As(handler.GetInterfaces().First())
                        .InstancePerLifetimeScope();
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
             

            builder.RegisterType<CommandBus>()
                .As<ICommandBus>()
                .InstancePerLifetimeScope();
        }
        
    }
}
