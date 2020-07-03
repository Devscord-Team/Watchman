using Devscord.DiscordFramework.Integration;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class RestGuildGetterService
    {
        public RestGuild GetRestGuild(ISocketMessageChannel data)
        {
            var serverId = (data as SocketGuildChannel).Guild.Id;
            var guild = Server.GetGuild(serverId).Result;
            return guild;
        }
    }
}
