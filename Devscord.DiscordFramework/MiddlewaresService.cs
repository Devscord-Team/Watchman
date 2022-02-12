using Autofac;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework
{
    public interface IMiddlewaresService
    {
        void AddMiddleware<T>() where T : IMiddleware;
        Contexts RunMiddlewares(SocketMessage socketMessage);
    }

    internal class MiddlewaresService
    {
        public IEnumerable<IMiddleware> Middlewares => this._middlewares;

        private readonly IComponentContext _context;
        private readonly List<IMiddleware> _middlewares = new List<IMiddleware>();

        public MiddlewaresService(IComponentContext context)
        {
            this._context = context;
        }

        public void AddMiddleware<T>() where T : IMiddleware
        {
            if (this._middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return;
            }
            var instance = this._context.Resolve<T>();
            this._middlewares.Add(instance);
        }

        public Contexts RunMiddlewares(SocketMessage socketMessage)
        {
            var contextsInstance = new Contexts();
            var discordContexts = this.GetMiddlewaresOutput(socketMessage);
            foreach (var discordContext in discordContexts)
            {
                contextsInstance.SetContext(discordContext);
            }
            return contextsInstance;
        }

        private IEnumerable<IDiscordContext> GetMiddlewaresOutput(SocketMessage socketMessage)
        {
            return this._middlewares.Select(x => x.Process(socketMessage));
        }
    }
}
