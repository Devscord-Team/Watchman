using System;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClientMessagesService
    {
        Func<SocketMessage, Task> MessageReceived { get; set; }
    }
}
