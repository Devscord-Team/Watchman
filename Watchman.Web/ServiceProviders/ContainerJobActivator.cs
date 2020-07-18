using System;
using Autofac;
using Hangfire;

namespace Watchman.Web.ServiceProviders
{
    public class ContainerJobActivator : JobActivator
    {
        private readonly IContainer _container;

        public ContainerJobActivator(IContainer container)
        {
            this._container = container;
        }

        public override object ActivateJob(Type type)
        {
            return this._container.Resolve(type);
        }
    }
}
