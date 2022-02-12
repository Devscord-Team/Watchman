using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ChannelMiddleware : IMiddleware
    {
        public IDiscordContext Process(SocketMessage data)
        {
            return new ChannelContext(data.Channel.Id, data.Channel.Name);
        }
    }
}
