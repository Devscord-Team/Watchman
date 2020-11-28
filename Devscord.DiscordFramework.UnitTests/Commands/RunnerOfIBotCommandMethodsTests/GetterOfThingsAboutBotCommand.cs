using Devscord.DiscordFramework.Framework.Commands.Builders;
using Devscord.DiscordFramework.Framework.Commands.Services;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests
{
    class GetterOfThingsAboutBotCommand
    {
        public RunnerOfIBotCommandMethods GetRunner(List<CustomCommand> customCommands = null)
        {
            var botCommandsService = new BotCommandsService(
                new BotCommandsTemplateBuilder(),
                new BotCommandsParsingService(new BotCommandsPropertyConversionService(), new BotCommandsRequestValueGetterService()),
                new BotCommandsMatchingService(),
                botCommandsTemplateRenderingService: null);

            var customCommandsLoader = new Mock<ICustomCommandsLoader>();
            customCommandsLoader
                .Setup(x => x.GetCustomCommands())
                .Returns(Task.FromResult(customCommands ?? new List<CustomCommand>()));
            var commandsContainer = new CommandsContainer(customCommandsLoader.Object);

            var validatorOfCommand = new CommandMethodValidator();

            return new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validatorOfCommand);
        }

        public Contexts GetContexts(bool isOwnerOrAdmin = true)
        {
            var server = new DiscordServerContext(id: 0, null, null, null, null, null, null);
            var user = new UserContext(id: 0, "a name of a user", roles: isOwnerOrAdmin ? null : new List<UserRole>(), null, null, getIsOwner: user => isOwnerOrAdmin, null);
            return new Contexts(server, null, user);
        }

        public IEnumerable<ControllerInfo> GetListOfControllerInfo(IController controller)
        {
            return new List<ControllerInfo> { new ControllerInfo(controller) };
        }

        public List<CustomCommand> CreateCustomCommandFor<T>(string regexInText) where T : IBotCommand
        {
            return new List<CustomCommand> 
            { 
                new CustomCommand(typeof(T).FullName, new Regex(regexInText), serverId: 0) 
            };
        }

        public List<CustomCommand> CreateCustomCommands((Type type, string regexInText)[] typesAndRegexes)
        {
            return typesAndRegexes
                .Select(x => new CustomCommand(x.type.FullName, new Regex(x.regexInText), serverId: 0))
                .ToList();
        }
    }
}
