using System;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    internal class SpamDetectorsTestsService<T> where T : ISpamDetector
    {
        private readonly IConfigurationService _configurationService;
        private readonly AntiSpamTestsService _antiSpamTestsService;

        public SpamDetectorsTestsService()
        {
            this._antiSpamTestsService = new AntiSpamTestsService();
            this._configurationService = AntiSpamTestsService.GetConfigurationsMock().Object;
        }

        public SpamProbability GetSpamProbability(bool isUserSafe, params string[] messagesContent)
        {
            var smallMessages = messagesContent.Select(x => new SmallMessage(x, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow, GetMessagesQuery.GET_ALL_SERVERS));
            return this.GetSpamProbability(isUserSafe, smallMessages.ToArray());
        }

        public SpamProbability GetSpamProbability(bool isUserSafe, params SmallMessage[] smallMessages)
        {
            // Arrange
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);
            
            var (request, contexts) = this._antiSpamTestsService.CreateRequestAndContexts(smallMessages.Last());
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(smallMessages.SkipLast(1));

            var needsConfiguration = typeof(T).GetConstructors().Any(x => x.GetParameters().Any(x => x.ParameterType == typeof(IConfigurationService)));
            var spamDetector = needsConfiguration 
                ? (T)Activator.CreateInstance(typeof(T), userSafetyChecker.Object, this._configurationService)
                : (T)Activator.CreateInstance(typeof(T), userSafetyChecker.Object);

            // Act
            return spamDetector!.GetSpamProbability(serverMessages, request, contexts);
        }
    }
}
