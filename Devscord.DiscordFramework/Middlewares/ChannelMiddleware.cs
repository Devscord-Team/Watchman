using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ChannelMiddleware : IMiddleware<ChannelContext>
    {
        public ChannelContext Process(SocketMessage data)
        {
            return new ChannelContext(data.Channel.Id, data.Channel.Name); ;
        }
    }
}
