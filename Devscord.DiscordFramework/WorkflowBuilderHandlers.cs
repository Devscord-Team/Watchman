using Autofac;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework
{
    public class WorkflowBuilderHandlers<T>
    {
        private readonly List<T> _handlers = new List<T>();
        private readonly IComponentContext _context;

        internal IEnumerable<T> Handlers => this._handlers;

        public WorkflowBuilderHandlers(IComponentContext context)
        {
            this._context = context;
        }
        public WorkflowBuilderHandlers<T> AddHandler(T handler, bool onlyOnDebug = false)
        {
            if (!this.ShouldIgnore(onlyOnDebug))
            {
                this._handlers.Add(handler);
            }

            return this;
        }

        public WorkflowBuilderHandlers<T> AddFromIoC<A>(Func<A, T> func, bool onlyOnDebug = false)
        {
            if (!this.ShouldIgnore(onlyOnDebug))
            {
                var resolved = this._context.Resolve<A>();
                var handler = func.Invoke(resolved);
                this.AddHandler(handler);
            }
            return this;
        }

        public WorkflowBuilderHandlers<T> AddFromIoC<A, B>(Func<A, B, T> func, bool onlyOnDebug = false)
        {
            if (!this.ShouldIgnore(onlyOnDebug))
            {
                var resolvedA = this._context.Resolve<A>();
                var resolvedB = this._context.Resolve<B>();
                var handler = func.Invoke(resolvedA, resolvedB);
                this.AddHandler(handler);
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
