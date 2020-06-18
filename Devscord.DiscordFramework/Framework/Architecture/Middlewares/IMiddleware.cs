using Discord.WebSocket;

namespace Devscord.DiscordFramework.Framework.Architecture.Middlewares
{
    public interface IMiddleware
    {
        IDiscordContext Process(SocketMessage data);
    }
}
