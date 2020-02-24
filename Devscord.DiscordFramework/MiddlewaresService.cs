using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework
{
    internal class MiddlewaresService
    {
        private List<IMiddleware<IDiscordContext>> _middlewares = new List<IMiddleware<IDiscordContext>>();
        public IEnumerable<IMiddleware<IDiscordContext>> Middlewares => _middlewares;

        public void AddMiddleware<T, W>()
            where T : IMiddleware<W>
            where W : IDiscordContext
        {
            if (this._middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return;
            }
            var instance = Activator.CreateInstance<IMiddleware<IDiscordContext>>();
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
