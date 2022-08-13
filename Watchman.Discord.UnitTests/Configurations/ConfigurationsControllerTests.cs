using AutoFixture;
using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Configurations.BotCommands;
using Watchman.Discord.Areas.Configurations.Controllers;
using Watchman.Discord.Areas.Configurations.Services;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Configuration;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.UnitTests.Configurations
{
    [TestFixture]
    internal class ConfigurationsControllerTests
    {
        private readonly TestContextsFactory _testContextsFactory = new();
        private readonly MessagesServiceMockWithResponsesFactory _messagesServiceMockFactory = new();

        private static readonly object[] _providedValues = new[]
        {
            new object[] { "asa", null, null, null, null, null, null },
            new object[] { null, "true", null, null, null, null, null },
            new object[] { null, null, 0ul, null, null, null, null },
            new object[] { null, null, null, new List<string> { string.Empty }, null, null, null },
            new object[] { null, null, null, null, 0.0, null, null },
            new object[] { null, null, null, null, null, TimeSpan.Zero, null },
            new object[] { null, null, null, null, null, null, 0ul }
        };

        [Test, AutoData]
        public async Task SetCustomConfiguration_ShouldSendResponseConfigurationItemNotFound(SetConfigurationCommand command)
        {
            // Arrange
            var contexts = this._testContextsFactory.CreateContexts(1, 1, 1);

            var responsesServiceMock = new Mock<IResponsesService>();
            var messagesServiceMock = this._messagesServiceMockFactory.Create(responsesServiceMock);
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var configurationServiceMock = new Mock<IConfigurationService>();
            var configurationValueSetterMock = new Mock<IConfigurationValueSetter>();

            var controller = new ConfigurationsController(messagesServiceFactoryMock.Object, configurationServiceMock.Object, null, configurationValueSetterMock.Object);

            // Act
            await controller.SetCustomConfiguration(command, contexts);

            // Assert
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
            responsesServiceMock.Verify(x => x.ProcessResponse("ConfigurationItemNotFound", It.IsAny<KeyValuePair<string, string>[]>()), Times.Once);
            configurationValueSetterMock.Verify(x => x.SetDefaultValueForConfiguration(It.IsAny<ConfigurationItem>(), It.IsAny<Type>()), Times.Never);
            configurationValueSetterMock
                .Verify(x => x.SetConfigurationValueFromCommand(It.IsAny<SetConfigurationCommand>(), It.IsAny<ConfigurationItem>(), It.IsAny<IEnumerable<object>>(), It.IsAny<Type>()), Times.Never);
        }

        [Test, AutoData]
        public async Task SetCustomConfiguration_ShouldSendResponseTooManyValueArgumentsForSetConfiguration_WhenUserProvidedMoreThanOneValue(SetConfigurationCommand command)
        {
            // Arrange
            var contexts = this._testContextsFactory.CreateContexts(1, 1, 1);

            var responsesServiceMock = new Mock<IResponsesService>();
            var messagesServiceMock = this._messagesServiceMockFactory.Create(responsesServiceMock);
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var testMappedConfigurationMock = new Mock<IMappedConfiguration>();
            testMappedConfigurationMock.SetupGet(x => x.Name)
                .Returns(command.Name);
            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(x => x.GetConfigurationItems(It.IsAny<ulong>()))
                .Returns(new List<IMappedConfiguration> { testMappedConfigurationMock.Object });

            var configurationValueSetterMock = new Mock<IConfigurationValueSetter>();

            var controller = new ConfigurationsController(messagesServiceFactoryMock.Object, configurationServiceMock.Object, null, configurationValueSetterMock.Object);

            // Act
            await controller.SetCustomConfiguration(command, contexts);

            // Assert
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
            responsesServiceMock.Verify(x => x.ProcessResponse("TooManyValueArgumentsForSetConfiguration"), Times.Once);
            configurationValueSetterMock.Verify(x => x.SetDefaultValueForConfiguration(It.IsAny<ConfigurationItem>(), It.IsAny<Type>()), Times.Never);
            configurationValueSetterMock
                .Verify(x => x.SetConfigurationValueFromCommand(It.IsAny<SetConfigurationCommand>(), It.IsAny<ConfigurationItem>(), It.IsAny<IEnumerable<object>>(), It.IsAny<Type>()), Times.Never);
        }

        [Test, AutoData]
        public async Task SetCustomConfiguration_ShouldSetDefaultTypeValue_WhenUserDidNotProvideAnyValue(string commandName, ConfigurationItem configurationItem)
        {
            // Arrange
            var command = new SetConfigurationCommand
            {
                Name = commandName
            };
            var contexts = this._testContextsFactory.CreateContexts(1, 1, 1);

            var responsesServiceMock = new Mock<IResponsesService>();
            var messagesServiceMock = this._messagesServiceMockFactory.Create(responsesServiceMock);
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var testMappedConfigurationMock = new Mock<IMappedConfiguration>();
            testMappedConfigurationMock.SetupGet(x => x.Name)
                .Returns(commandName);
            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(x => x.GetConfigurationItems(It.IsAny<ulong>()))
                .Returns(new List<IMappedConfiguration> { testMappedConfigurationMock.Object });

            var configurationMapperServiceMock = new Mock<IConfigurationMapperService>();
            configurationMapperServiceMock.Setup(x => x.MapIntoBaseFormat(It.IsAny<IMappedConfiguration>(), It.IsAny<ulong>()))
                .Returns(configurationItem);

            var configurationValueSetterMock = new Mock<IConfigurationValueSetter>();

            var controller = new ConfigurationsController(messagesServiceFactoryMock.Object, configurationServiceMock.Object, configurationMapperServiceMock.Object, configurationValueSetterMock.Object);

            // Act
            await controller.SetCustomConfiguration(command, contexts);

            // Assert
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
            responsesServiceMock.Verify(x => x.ProcessResponse("ConfigurationValueHasBeenSetAsDefaultOfType", It.IsAny<Contexts>(), It.IsAny<KeyValuePair<string, string>[]>()), Times.Once);
            configurationValueSetterMock.Verify(x => x.SetDefaultValueForConfiguration(It.IsAny<ConfigurationItem>(), It.IsAny<Type>()), Times.Once);
            configurationValueSetterMock
                .Verify(x => x.SetConfigurationValueFromCommand(It.IsAny<SetConfigurationCommand>(), It.IsAny<ConfigurationItem>(), It.IsAny<IEnumerable<object>>(), It.IsAny<Type>()), Times.Never);
        }
        
        [Test]
        [TestCaseSource(nameof(_providedValues))]
        public async Task SetCustomConfiguration_ShouldSetCustomValue_WhenUserProvidedOnlyOneValue(string textValue, string boolValue, ulong? channelValue, List<string> listValue, double? numberValue, TimeSpan? timeValue, ulong? userValue)
        {
            // Arrange
            var fixture = new Fixture();
            var command = new SetConfigurationCommand
            {
                Name = fixture.Create<string>(),
                TextValue = textValue,
                BoolValue = boolValue,
                ChannelValue = channelValue,
                ListValue = listValue,
                NumberValue = numberValue,
                TimeValue = timeValue,
                UserValue = userValue,
            };
            var contexts = this._testContextsFactory.CreateContexts(1, 1, 1);

            var responsesServiceMock = new Mock<IResponsesService>();
            var messagesServiceMock = this._messagesServiceMockFactory.Create(responsesServiceMock);
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var testMappedConfigurationMock = new Mock<IMappedConfiguration>();
            testMappedConfigurationMock.SetupGet(x => x.Name)
                .Returns(command.Name);
            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(x => x.GetConfigurationItems(It.IsAny<ulong>()))
                .Returns(new List<IMappedConfiguration> { testMappedConfigurationMock.Object });

            var configurationMapperServiceMock = new Mock<IConfigurationMapperService>();
            configurationMapperServiceMock.Setup(x => x.MapIntoBaseFormat(It.IsAny<IMappedConfiguration>(), It.IsAny<ulong>()))
                .Returns(fixture.Create<ConfigurationItem>());

            var configurationValueSetterMock = new Mock<IConfigurationValueSetter>();

            var controller = new ConfigurationsController(messagesServiceFactoryMock.Object, configurationServiceMock.Object, configurationMapperServiceMock.Object, configurationValueSetterMock.Object);

            // Act
            await controller.SetCustomConfiguration(command, contexts);

            // Assert
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
            responsesServiceMock.Verify(x => x.ProcessResponse("CustomConfigurationHasBeenSet", It.IsAny<Contexts>(), It.IsAny<KeyValuePair<string, string>[]>()), Times.Once);
            configurationValueSetterMock.Verify(x => x.SetDefaultValueForConfiguration(It.IsAny<ConfigurationItem>(), It.IsAny<Type>()), Times.Never);
            configurationValueSetterMock
                .Verify(x => x.SetConfigurationValueFromCommand(It.IsAny<SetConfigurationCommand>(), It.IsAny<ConfigurationItem>(), It.IsAny<IEnumerable<object>>(), It.IsAny<Type>()), Times.Once);
        }
    }
}
