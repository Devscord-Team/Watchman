using Autofac;
using Devscord.DiscordFramework.Architecture.Controllers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Watchman.IoC.Modules
{
    [ExcludeFromCodeCoverage]
    public class ControllerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(ControllerModule)
                .GetTypeInfo()
                .Assembly;

            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                var controllers = asm.GetTypes()
                    .Where(type => typeof(IController).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .ToArray();

                foreach (var controller in controllers)
                {
                    builder.RegisterType(controller)
                        .As(controller)
                        .SingleInstance();
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
        }
    }
}
