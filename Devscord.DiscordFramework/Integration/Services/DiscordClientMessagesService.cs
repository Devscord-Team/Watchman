using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientMessagesService : IDiscordClientMessagesService
    {
        private readonly DiscordSocketClient client;
        public Func<SocketMessage, Task> MessageReceived { get; set; }

        public DiscordClientMessagesService(DiscordSocketClient client)
        {
            this.client = client;
        }
    }
}
