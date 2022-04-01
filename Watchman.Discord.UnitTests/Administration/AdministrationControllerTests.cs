using AutoFixture;
using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using FluentAssertions;
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
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Configuration.Services;
using Watchman.DomainModel.DiscordServer.Queries;

namespace Watchman.Discord.UnitTests.Administration
{
    [TestFixture]
    internal class AdministrationControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new ();
        private readonly TestContextsFactory testContextsFactory = new();

        [Test]
        [TestCase(true, true, 1, typeof(InvalidArgumentsException))]
        [TestCase(false, false, 1, typeof(NotEnoughArgumentsException))]
        [TestCase(true, false, 6, typeof(InvalidArgumentsException))]
        [TestCase(true, false, 0, typeof(InvalidArgumentsException))]
        [TestCase(true, false, 5, null)]
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
            command.Roles.Should().HaveCount(rolesCount);
        }

        [Test]
        [TestCase(true, false, 1)]
        [TestCase(false, true, 1)]
        [TestCase(false, true, 5)]
        [TestCase(true, false, 5)]
        public async Task SetRoleAsSafe_ProperCommandShouldInvokeRolesService(bool safeParam, bool unsafeParam, int rolesCount)
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
            var rolesServiceMock = new Mock<IRolesService>();
            var controller = this.testControllersFactory.CreateAdministrationController(rolesServiceMock: rolesServiceMock);

            //Act
            await controller.SetRoleAsSafe(command, contexts);

            //Assert
            rolesServiceMock.Verify(x => x.SetRolesAsSafe(contexts, rolesList, safeParam), Times.Once);
        }

        [Test, AutoData]
        public async Task SetRoleAsTrusted_ShouldInvokeTrustRolesService(TrustCommand command, Contexts contexts)
        {
            //Arrange
            var trustRolesServiceMock = new Mock<ITrustRolesService>();
            var controller = this.testControllersFactory.CreateAdministrationController(trustRolesServiceMock: trustRolesServiceMock);

            //Act
            await controller.SetRoleAsTrusted(command, contexts);

            //Assert
            trustRolesServiceMock.Verify(x => x.TrustThisRole(command.Role, contexts), Times.Once);
        }

        [Test, AutoData]
        public async Task SetRoleAsUntrusted_ShouldInvokeTrustRolesService(UntrustCommand command, Contexts contexts)
        {
            //Arrange
            var trustRolesServiceMock = new Mock<ITrustRolesService>();
            var controller = this.testControllersFactory.CreateAdministrationController(trustRolesServiceMock: trustRolesServiceMock);

            //Act
            await controller.SetRoleAsUntrusted(command, contexts);

            //Assert
            trustRolesServiceMock.Verify(x => x.StopTrustingRole(command.Role, contexts), Times.Once);
        }

        [Test]
        public async Task GetRoleTrustedRoles_ShouldGetTrustedRoles()
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var trustedRoles = new List<ulong>() {1ul, 2ul};
            var command = new TrustedRolesCommand();
            
            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>(); 
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var queryBusMock = new Mock<IQueryBus>();
            queryBusMock.Setup(x => x.Execute(It.IsAny<GetServerTrustedRolesQuery>()))
                .Returns(new GetServerTrustedRolesQueryResult(trustedRoles));

            var controller = this.testControllersFactory.CreateAdministrationController(
                queryBusMock : queryBusMock,
                messagesServiceFactoryMock : messagesServiceFactoryMock);

            //Act
            await controller.GetTrustedRoles(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(It.IsAny<Contexts>()), Times.Once);
            queryBusMock.Verify(x => x.Execute(It.IsAny<GetServerTrustedRolesQuery>()), Times.Once);
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<Devscord.DiscordFramework.Commands.Responses.IResponsesService, string>>()), Times.Never);
            messagesServiceMock.Verify(x => x.SendEmbedMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Once);
        }

        [Test]
        public async Task GetRoleTrustedRoles_ShouldNotGetTrustedRolesIfThereIsNoTrustedRoles()
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var trustedRoles = new List<ulong>();
            var command = new TrustedRolesCommand();

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var queryBusMock = new Mock<IQueryBus>();
            queryBusMock.Setup(x => x.Execute(It.IsAny<GetServerTrustedRolesQuery>()))
                .Returns(new GetServerTrustedRolesQueryResult(trustedRoles));

            var controller = this.testControllersFactory.CreateAdministrationController(
                queryBusMock: queryBusMock,
                messagesServiceFactoryMock: messagesServiceFactoryMock);

            //Act
            await controller.GetTrustedRoles(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(It.IsAny<Contexts>()), Times.Once);
            queryBusMock.Verify(x => x.Execute(It.IsAny<GetServerTrustedRolesQuery>()), Times.Once);
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<Devscord.DiscordFramework.Commands.Responses.IResponsesService, string>>()), Times.Once);
            messagesServiceMock.Verify(x => x.SendEmbedMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never);
        }
    }
}
