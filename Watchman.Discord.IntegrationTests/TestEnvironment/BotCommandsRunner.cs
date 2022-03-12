using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment.Models;
using IDiscordClient = Devscord.DiscordFramework.Integration.Services.Interfaces.IDiscordClient;

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
                Content = text,
                CreatedAt = DateTime.UtcNow.AddMilliseconds(-10),
                Author = new FakeUser() 
                { 
                    Id = 1,
                    IsBot = false,
                    IsWebhook = false,
                    Username = "TestUser",
                    Mention = "<@12345678>",
                    Status = UserStatus.Online,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                },
                Channel = new FakeChannel()
                {
                    Id = 1,
                    Name = "Test",
                    GuildId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    Guild = new FakeGuild()
                    {
                        Id = 1,
                        Name = "TestServer",
                        OwnerId = 5,
                        CreatedAt = DateTime.UtcNow.AddDays(-30),
                        Roles = new List<IRole>()
                        { 
                            new FakeRole() { Id = 1, Name = "First", CreatedAt = DateTime.UtcNow.AddDays(-5), },
                            new FakeRole() { Id = 2, Name = "Second", CreatedAt = DateTime.UtcNow.AddDays(-5), },
                            new FakeRole() { Id = 3, Name = "Third", CreatedAt = DateTime.UtcNow.AddDays(-5), },
                        }
                    }
                },
            };
            await client.MessagesService.MessageReceived.Invoke(message);
        }
    }
}
