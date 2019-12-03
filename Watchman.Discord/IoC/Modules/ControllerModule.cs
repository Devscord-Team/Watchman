using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Watchman.Discord.IoC.Modules
{
    public class ControllerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(ControllerModule)
                .GetTypeInfo()
                .Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.IsAssignableTo<IController>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
