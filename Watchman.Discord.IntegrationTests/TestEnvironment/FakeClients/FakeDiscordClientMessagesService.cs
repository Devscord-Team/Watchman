using System;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientMessagesService : IDiscordClientMessagesService
    {
        public Func<SocketMessage, Task> MessageReceived { get; set; }
    }
}
