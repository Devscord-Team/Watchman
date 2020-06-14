using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    internal class AntiSpamTestsService
    {
        public readonly ServerMessagesCacheService ExampleServerMessages = new ServerMessagesCacheService();
        public const int DEFAULT_TEST_USER_ID = 1;

        public AntiSpamTestsService()
        {
            var exampleSmallMessages = new List<SmallMessage>
            {
                new SmallMessage("abcde", 1, DateTime.Now.AddSeconds(-30)),
                new SmallMessage("abcdefg", 2, DateTime.Now.AddSeconds(-20)),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-20)),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-1)),
            };
            this.ExampleServerMessages.OverwriteMessages(exampleSmallMessages);
        }

        public (DiscordRequest request, Contexts contexts) CreateRequestAndContexts(string content)
        {
            var request = new DiscordRequest { OriginalMessage = content };
            var contexts = GetDefaultContexts();
            return (request, contexts);
        }

        private Contexts GetDefaultContexts()
        {
            var contexts = new Contexts();
            contexts.SetContext(new UserContext(DEFAULT_TEST_USER_ID, null, new List<UserRole>(), null, null));
            contexts.SetContext(new DiscordServerContext(GetMessagesQuery.GET_ALL_SERVERS, null, null, null, null));
            return contexts;
        }
    }
}