using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Architecture.Middlewares
{
    public interface IMiddleware
    {
        IDiscordContext Process(IMessage data);
    }
}
