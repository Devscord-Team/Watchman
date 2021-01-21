using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Services;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Moq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System.Reflection;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests
{
    class RunnerOfIBotCommandMethodsTestsService
    {
        public static IBotCommandsService GetBotCommandsServiceMock()
        {
            var mock = new Mock<IBotCommandsService>();
            var numberParser = new UniversalNumberParser();

            mock.Setup(m => m.IsDefaultCommand(It.IsAny<BotCommandTemplate>(), It.IsAny<IEnumerable<DiscordRequestArgument>>(), It.IsAny<bool>()))
                .Returns<BotCommandTemplate, IEnumerable<DiscordRequestArgument>, bool>((template, args, isCommandMatchedWithCustom) => 
                    new BotCommandsMatchingService(numberParser).IsDefaultCommand(template, args, isCommandMatchedWithCustom));

            mock.Setup(m => m.AreDefaultCommandArgumentsCorrect(It.IsAny<BotCommandTemplate>(), It.IsAny<IEnumerable<DiscordRequestArgument>>()))
                .Returns<BotCommandTemplate, IEnumerable<DiscordRequestArgument>>((template, args) => 
                    new BotCommandsMatchingService(numberParser).AreDefaultCommandArgumentsCorrect(template, args));

            mock.Setup(m => m.AreCustomCommandArgumentsCorrect(It.IsAny<BotCommandTemplate>(), It.IsAny<Regex>(), It.IsAny<string>()))
                .Returns<BotCommandTemplate, Regex, string>((template, customTemplate, input) =>
                    new BotCommandsMatchingService(numberParser).AreCustomCommandArgumentsCorrect(template, customTemplate, input));

            mock.Setup(m => m.GetCommandTemplate(It.IsAny<Type>()))
                .Returns<Type>(commandType => new BotCommandsTemplateBuilder().GetCommandTemplate(commandType));

            var botCommandsParsingService = new BotCommandsParsingService(new BotCommandsPropertyConversionService(numberParser), new BotCommandsRequestValueGetterService());
            mock.Setup(m => m.ParseRequestToCommand(It.IsAny<Type>(), It.IsAny<DiscordRequest>(), It.IsAny<BotCommandTemplate>()))
                .Returns<Type, DiscordRequest, BotCommandTemplate>((commandType, request, template) => 
                    botCommandsParsingService.ParseRequestToCommand(commandType, request, template));

            mock.Setup(m => m.ParseCustomTemplate(It.IsAny<Type>(), It.IsAny<BotCommandTemplate>(), It.IsAny<Regex>(), It.IsAny<string>()))
                .Returns<Type, BotCommandTemplate, Regex, string>((commandType, template, customTemplate, input) => 
                    botCommandsParsingService.ParseCustomTemplate(commandType, template, customTemplate, input));

            return mock.Object;
        }

        public static ICommandsContainer GetCommandsContainerMock(string regexInText, Type type)
        {
            var mock = new Mock<ICommandsContainer>();
            mock.Setup(m => m.GetCommand(It.IsAny<DiscordRequest>(), type, It.IsAny<Contexts>()))
                .Returns(Task.FromResult(new CustomCommand(type.FullName, new Regex(regexInText), serverId: 0)));
            return mock.Object;
        }

        public static ICommandMethodValidator GetCommandMethodValidatorMock()
        {
            var mock = new Mock<ICommandMethodValidator>();
            mock.Setup(m => m.IsValid(It.IsAny<Contexts>(), It.IsAny<MethodInfo>()))
                .Returns(true);
            return mock.Object;
        }
    }
}
