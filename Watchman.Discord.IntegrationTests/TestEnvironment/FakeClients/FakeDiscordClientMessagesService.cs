using System;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientMessagesService : IDiscordClientMessagesService
    {
        public Func<IMessage, Task> MessageReceived { get; set; }
    }
}
