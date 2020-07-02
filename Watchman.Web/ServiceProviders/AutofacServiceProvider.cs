using Autofac;
using System;

namespace Watchman.Web.ServiceProviders
{
    public class AutofacServiceProvider : IServiceProvider
    {
        private readonly IContainer container;
        public AutofacServiceProvider(IContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return this.container.Resolve(serviceType);
        }
    }
}
