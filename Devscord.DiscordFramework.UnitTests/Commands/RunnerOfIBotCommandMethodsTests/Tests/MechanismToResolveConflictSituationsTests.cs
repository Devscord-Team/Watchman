using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    // "Conflict situations" means situations in which a given command matches default and custom version of a bot command at the same time. During these situations, "mechanism to resolve conflict situations" is runned. 
    // It means a method which check what arguments are known for a bot command. Thanks to the mechanism, it is possible to determine if the command is in the default or custom version.
    // It matters for command correctness because setting of version is needed to determine how to parse the commands
    [TestFixture]
    class MechanismToResolveConflictSituationsTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _controller;
        private readonly CommandParser _commandParser;

        public MechanismToResolveConflictSituationsTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._controller = new TestController();
            this._commandParser = new CommandParser();
        }
    }
}
