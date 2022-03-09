using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientMessagesService : IDiscordClientMessagesService
    {
        private readonly DiscordSocketClient client;
        public Func<IMessage, Task> MessageReceived { get; set; }

        public DiscordClientMessagesService(DiscordSocketClient client)
        {
            //todo add removed, edited etc
            this.client = client;
            this.client.MessageReceived += (message) => this.MessageReceived(message);
        }
    }
}
