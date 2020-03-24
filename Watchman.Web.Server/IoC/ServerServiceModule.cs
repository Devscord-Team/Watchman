using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Watchman.Web.Server.Areas.Commons.Integration;

namespace Watchman.Web.Server.IoC
{
    public class ServerServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var asm = typeof(ServerServiceModule).Assembly;

            var services = asm.GetTypes().Where(x => x.Name.EndsWith("Service") || x.Name.EndsWith("Factory"));

            foreach (var service in services)
            {
                builder.RegisterType(service)
                    .As(service)
                    .InstancePerLifetimeScope();
            }
        }
    }
}
