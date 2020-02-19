using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class UserMiddleware : IMiddleware<UserContext>
    {
        private readonly UserContextsFactory userContextsFactory;

        public UserMiddleware()
        {
            this.userContextsFactory = new UserContextsFactory();
        }

        public UserContext Process(SocketMessage data)
        {
            var user = (SocketGuildUser)data.Author;
            return userContextsFactory.Create(user);
        }
    }
}
