using Autofac;
using Devscord.DiscordFramework.Architecture.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Watchman.Discord.IoC.Modules
{
    public class ControllerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());
            do
            {
                var asm = stack.Pop();

                if (asm.FullName.Contains("Watchman.Discord"))
                {
                    var controllers = asm.GetTypes()
                        .Where(type => typeof(IController).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        .ToList();

                    foreach (var controller in controllers)
                    {
                        builder.RegisterType(controller)
                            .As(controller)
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
        }
    }
}
