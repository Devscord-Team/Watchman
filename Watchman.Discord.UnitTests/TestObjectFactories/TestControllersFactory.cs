using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Administration.Controllers;
using Watchman.Discord.Areas.Administration.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Users.Services;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.UnitTests.TestObjectFactories
{
    internal class TestControllersFactory
    {
        internal AdministrationController CreateAdministrationController(
            Mock<IQueryBus> queryBusMock = null, Mock<IUsersService> usersServiceMock = null, 
            Mock<IDirectMessagesService> directMessagesServiceMock = null, Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null, 
            Mock<IRolesService> rolesServiceMock = null, Mock<ITrustRolesService> trustRolesServiceMock = null,
            Mock<ICheckUserSafetyService> checkUserSafetyServiceMock = null, Mock<IUsersRolesService> usersRolesServiceMock = null, 
            Mock<IConfigurationService> configurationServiceMock = null)
        {
            queryBusMock ??= new Mock<IQueryBus>();
            usersServiceMock ??= new Mock<IUsersService>();
            directMessagesServiceMock ??= new Mock<IDirectMessagesService>();
            messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();
            rolesServiceMock ??= new Mock<IRolesService>();
            trustRolesServiceMock ??= new Mock<ITrustRolesService>();
            checkUserSafetyServiceMock ??= new Mock<ICheckUserSafetyService>();
            usersRolesServiceMock ??= new Mock<IUsersRolesService>();
            configurationServiceMock ??= new Mock<IConfigurationService>();

            return new AdministrationController(
                queryBusMock.Object,
                usersServiceMock.Object,
                directMessagesServiceMock.Object,
                messagesServiceFactoryMock.Object,
                rolesServiceMock.Object,
                trustRolesServiceMock.Object,
                checkUserSafetyServiceMock.Object,
                usersRolesServiceMock.Object,
                configurationServiceMock.Object);
        }
    }
}
