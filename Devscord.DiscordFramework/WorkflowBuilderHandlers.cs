using Autofac;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public class WorkflowBuilderHandlers<T>
    {
        private List<T> _handlers = new List<T>();
        private readonly IComponentContext context;

        internal IEnumerable<T> Handlers => _handlers;

        public WorkflowBuilderHandlers(IComponentContext context)
        {
            this.context = context;
        }

        public WorkflowBuilderHandlers<T> AddHandler(T handler, bool onlyOnDebug = false)
        {
            if(!ShouldIgnore(onlyOnDebug))
            {
                _handlers.Add(handler);
            }
            
            return this;
        }

        public WorkflowBuilderHandlers<T> AddFromIoC<A>(Func<A, T> func, bool onlyOnDebug = false)
        {
            if (!ShouldIgnore(onlyOnDebug))
            {
                var resolved = context.Resolve<A>();
                var handler = func.Invoke(resolved);
                AddHandler(handler);
            }
            return this;
        }

        public WorkflowBuilderHandlers<T> AddFromIoC<A, B>(Func<A, B, T> func, bool onlyOnDebug = false)
        {
            if (!ShouldIgnore(onlyOnDebug))
            {
                var resolvedA = this.context.Resolve<A>();
                var resolvedB = this.context.Resolve<B>();
                var handler = func.Invoke(resolvedA, resolvedB);
                AddHandler(handler);
            }
            return this;
        }

        private bool ShouldIgnore(bool onlyOnDebug)
        {
            var isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            if (onlyOnDebug && !isDebug)
            {
                return true;
            }
            return false;
        }
    }
}
