using Autofac;
using System;

namespace Watchman.Web.ServiceProviders
{
    public class AutofacServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public AutofacServiceProvider(IContainer container)
        {
            this._container = container;
        }

        public object GetService(Type serviceType)
        {
            return this._container.Resolve(serviceType);
        }
    }
}
