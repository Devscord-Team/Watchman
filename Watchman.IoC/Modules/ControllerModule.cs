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

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly.FullName.ToLower().Contains("testhost"))
            {
                var botDllPath = entryAssembly.Location.Replace("testhost.dll", "Watchman.Discord.dll");
                entryAssembly = Assembly.LoadFrom(botDllPath);
            }
            stack.Push(entryAssembly);
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
                        .PreserveExistingDefaults()
                        .SingleInstance();
                }

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    if (!reference.FullName.Contains("Watchman") && !reference.FullName.Contains("Devscord"))
                    {
                        continue;
                    }
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
