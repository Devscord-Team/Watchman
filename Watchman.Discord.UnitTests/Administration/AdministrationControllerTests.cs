using AutoFixture;
using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Administration.BotCommands;
using Watchman.Discord.Areas.Administration.Controllers;
using Watchman.Discord.Areas.Administration.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.UnitTests.Administration
{
    [TestFixture]
    internal class AdministrationControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new ();

        [Test]
        [TestCase(true, true, 1, typeof(InvalidArgumentsException))]
        [TestCase(false, false, 1, typeof(NotEnoughArgumentsException))]
        [TestCase(true, false, 10, typeof(InvalidArgumentsException))]
        [TestCase(true, false, 1, null)]
        public void SetRoleAsSafe_ShouldThrowExceptionWhenCommandNotMatchRules(bool safeParam, bool unsafeParam, int rolesCount, Type exceptionType)
        {
            //Arrange
            var fixture = new Fixture();
            var contexts = fixture.Create<Contexts>();

            var rolesList = Enumerable.Range(0, rolesCount).Select(x => x.ToString()).ToList();
            var command = new SetRoleCommand() 
            { 
                Safe = safeParam, 
                Unsafe = unsafeParam, 
                Roles = rolesList 
            };
            var controller = this.testControllersFactory.CreateAdministrationController();

            //Act
            if (exceptionType == null)
            {
                Assert.DoesNotThrowAsync(() => controller.SetRoleAsSafe(command, contexts));
            }
            else
            {
                Assert.ThrowsAsync(exceptionType, () => controller.SetRoleAsSafe(command, contexts));
            }

            //Assert
        }

        
    }
}
