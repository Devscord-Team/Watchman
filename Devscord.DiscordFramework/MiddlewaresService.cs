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
        private readonly List<IMiddleware> _middlewares = new List<IMiddleware>();
        public IEnumerable<IMiddleware> Middlewares => this._middlewares;

        public void AddMiddleware<T>()
            where T : IMiddleware
        {
            if (this._middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return;
            }
            var instance = Activator.CreateInstance<T>();
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

        private IEnumerable<IDiscordContext> GetMiddlewaresOutput(SocketMessage socketMessage) => this._middlewares.Select(x => x.Process(socketMessage));
    }
}
