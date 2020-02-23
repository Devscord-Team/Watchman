using Autofac;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Calculators.Statistics;

namespace Watchman.Web.Server.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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

            builder.RegisterType<StatisticsCalculator>()
                .As<IStatisticsCalculator>()
                .InstancePerLifetimeScope();
        }
    }
}
