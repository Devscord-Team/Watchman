using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    [TestFixture]
    class OtherTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _controller;
        private readonly CommandParser _commandParser;

        public OtherTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._controller = new TestController();
            this._commandParser = new CommandParser();
        }

        [Test]
        public void ShouldThrowException_WhenUserIsNotAdminAndUserUsesCommandOnlyForAdmin()
        {
            // Arrange:
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts(isOwnerOrAdmin: false);
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse("-optionalArgs", DateTime.Now);

            // Act:
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            Assert.ThrowsAsync<NotAdminPermissionsException>(RunMethodsFunc);
        }

        [Test]
        public void ShouldDoesNotThrowException_WhenAdminUsesCommandOnlyForAdmin()
        {
            // Arrange:
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts(isOwnerOrAdmin: true);
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse("-optionalArgs", DateTime.Now);

            // Act:
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            Assert.DoesNotThrowAsync(RunMethodsFunc);
        }
    }
}
