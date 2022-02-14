using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Commons;
using Watchman.Discord.Integration.DevscordFramework;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.IoC.Modules
{
    [ExcludeFromCodeCoverage]
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ResponsesCachingService(c.Resolve<DiscordServersService>()).SetGetResponsesFromDatabase(c.Resolve<IQueryBus>()))
                .PreserveExistingDefaults()
                .As<IResponsesCachingService>()
                .SingleInstance();

            var list = new List<string>();
            var stack = new Stack<Assembly>();

            var typesToRegister = new List<Type>();

            var entryAssembly = Assembly.GetEntryAssembly();
            var botDllPath = Regex.Replace(entryAssembly.Location.Replace("\\", "/"), @"[^\/]*?\.dll$", "Watchman.Discord.dll");
            var botAssembly = Assembly.LoadFrom(botDllPath);

            builder.RegisterInstance(botAssembly)
                .As<Assembly>()
                .SingleInstance();

            if (entryAssembly.FullName.ToLower().Contains("testhost"))
            {
                entryAssembly = botAssembly;
            }
            stack.Push(entryAssembly);
            do
            {
                var asm = stack.Pop();

                var types = asm.GetTypes()
                    .Where(x => x.FullName.StartsWith("Watchman") || x.FullName.StartsWith("Devscord") || x.FullName.StartsWith("Statsman"))
                    .Where(x => !x.IsInterface && !x.IsAbstract && x.GetConstructors().Any())
                    .Where(x => x.GetInterfaces().Where(x => x.Name != "IAsyncStateMachine").Any())
                    .ToArray();

                foreach (var type in types)
                { 
                    typesToRegister.Add(type);
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

            foreach (var type in typesToRegister)
            {
                builder.RegisterType(type)
                    .PreserveExistingDefaults()
                    .AsImplementedInterfaces()
                    .AsSelf()
                    .SingleInstance();
            }
        }
    }
}
