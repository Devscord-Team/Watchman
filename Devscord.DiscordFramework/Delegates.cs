using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework
{
    public abstract class Delegates
    {
        public Func<Contexts, Task> UserJoined { get; set; }

        protected Delegates()
        {
            Server.UserJoined += CallUserJoined;
        }

        private Task CallUserJoined(SocketGuildUser guildUser)
        {
            var userFactory = new UserContextsFactory();
            var serverFactory = new DiscordServerContextFactory();

            var userContext = userFactory.Create(guildUser);
            var discordServerContext = serverFactory.Create(guildUser.Guild);
            var landingChannel = discordServerContext.LandingChannel;

            var contexts = new Contexts();
            contexts.SetContext(userContext);
            contexts.SetContext(discordServerContext);
            contexts.SetContext(landingChannel);

            UserJoined.Invoke(contexts);
            return Task.CompletedTask;
        }
    }
}
