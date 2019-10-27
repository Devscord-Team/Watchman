using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord.Framework.Architecture.Controllers;

namespace Watchman.Discord.Framework
{
    public class Workflow
    {
        private List<object> _middlewares;

        public Workflow()
        {
            this._middlewares = new List<object>();
        }

        public Workflow AddMiddleware<T>(object configuration = null /*TODO*/)
        {
            if(this._middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return this;
            }
            var middleware = Activator.CreateInstance<T>();
            this._middlewares.Add(middleware);
            return this;
        }

        public async Task Run<T>(T data)
        {
            await Task.WhenAll(this._middlewares
                .Where(x => x.GetType()
                    .GetInterfaces().First()
                    .GenericTypeArguments
                    .First().FullName == typeof(T).FullName)
                .ToList()
                .Select(x => (Task)((dynamic)x).Process(data)));

            await Task.CompletedTask;
        }
    }

    public interface IMiddleware<T>
    {
        Task Process(T data);
    }

    

    
}
