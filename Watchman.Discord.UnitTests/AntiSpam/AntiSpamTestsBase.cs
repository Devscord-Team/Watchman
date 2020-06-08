using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    internal abstract class AntiSpamTestsBase
    {
        protected readonly ServerMessagesCacheService ExampleServerMessages = new ServerMessagesCacheService();
        protected readonly Mock<IUserMessagesCounter> UserMessagesCounter = new Mock<IUserMessagesCounter>();
        protected const int DEFAULT_TEST_USER_ID = 1;
        private const int DEFAULT_COUNT_TO_BE_SAFE = 500;

        protected AntiSpamTestsBase()
        {
            var exampleSmallMessages = new List<SmallMessage>
            {
                new SmallMessage("abcde", 1, DateTime.Now.AddSeconds(-30)),
                new SmallMessage("abcdefg", 2, DateTime.Now.AddSeconds(-20)),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-20)),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-1)),
            };
            this.ExampleServerMessages.OverwriteMessages(exampleSmallMessages);
            this.UserMessagesCounter
                .Setup(x => x.UserMessagesCountToBeSafe)
                .Returns(DEFAULT_COUNT_TO_BE_SAFE);
        }

        protected (DiscordRequest request, Contexts contexts) CreateRequestAndContexts(string content)
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