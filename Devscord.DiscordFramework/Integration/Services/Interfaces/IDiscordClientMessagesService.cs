using System;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClientMessagesService
    {
        Func<IMessage, Task> MessageReceived { get; set; }
    }
}
