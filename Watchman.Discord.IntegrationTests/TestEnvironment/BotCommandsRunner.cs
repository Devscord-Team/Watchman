using Devscord.DiscordFramework.Integration.Services.Interfaces;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment.Models;

namespace Watchman.Discord.IntegrationTests.TestEnvironment
{
    public class BotCommandsRunner
    {
        private readonly IDiscordClient client;

        public BotCommandsRunner(IDiscordClient client)
        {
            this.client = client;
        }

        public async Task SendMessage(string text)
        {
            var message = new FakeMessage
            {
                Content = text
            };
            await client.MessagesService.MessageReceived.Invoke(message);
        }
    }
}
