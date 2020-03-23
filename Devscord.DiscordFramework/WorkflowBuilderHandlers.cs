using Autofac;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework
{
    public class WorkflowBuilderHandlers<T>
    {
        private List<T> _handlers = new List<T>();
        private readonly IContainer _container;

        internal IEnumerable<T> Handlers => _handlers;

        public WorkflowBuilderHandlers(IContainer container)
        {
            this._container = container;
        }

        public WorkflowBuilderHandlers<T> AddHandler(T handler, bool onlyOnDebug = false)
        {
            var isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            if(!onlyOnDebug || (onlyOnDebug && isDebug))
            {
                _handlers.Add(handler);
            }
            return this;
        }

        public WorkflowBuilderHandlers<T> AddFromIoC<W>(Func<W, T> func)
        {
            var resolved = _container.Resolve<W>();
            var handler = func.Invoke(resolved);
            AddHandler(handler);
            return this;
        }
    }
}
