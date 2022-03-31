using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Commands.Responses;
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
using Watchman.Discord.Areas.Muting.Controllers;
using Watchman.Discord.Areas.Muting.Services;
using Watchman.Discord.Areas.Users.Controllers;
using Watchman.Discord.Areas.Responses.Controllers;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.Discord.Areas.Users.Services;
using Watchman.DomainModel.Configuration.Services;
using Watchman.Discord.Areas.AntiSpam.Controllers;
using Watchman.Discord.Areas.AntiSpam.Services;
using Watchman.Discord.Areas.AntiSpam.Strategies;

namespace Watchman.Discord.UnitTests.TestObjectFactories
{
    internal class TestControllersFactory
    {
        internal AdministrationController CreateAdministrationController(
            Mock<IQueryBus> queryBusMock = null, Mock<IUsersService> usersServiceMock = null, 
            Mock<IDirectMessagesService> directMessagesServiceMock = null, Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null, 
            Mock<IRolesService> rolesServiceMock = null, Mock<ITrustRolesService> trustRolesServiceMock = null,
            /*Mock<ICheckUserSafetyService> checkUserSafetyServiceMock = null,*/ Mock<IUsersRolesService> usersRolesServiceMock = null, 
            Mock<IConfigurationService> configurationServiceMock = null, Mock<IComplaintsChannelService> complaintsChannelServiceMock = null)
        {
            queryBusMock ??= new Mock<IQueryBus>();
            usersServiceMock ??= new Mock<IUsersService>();
            directMessagesServiceMock ??= new Mock<IDirectMessagesService>();
            messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();
            rolesServiceMock ??= new Mock<IRolesService>();
            trustRolesServiceMock ??= new Mock<ITrustRolesService>();
            //checkUserSafetyServiceMock ??= new Mock<ICheckUserSafetyService>();
            usersRolesServiceMock ??= new Mock<IUsersRolesService>();
            configurationServiceMock ??= new Mock<IConfigurationService>();
            complaintsChannelServiceMock ??= new Mock<IComplaintsChannelService>();

            return new AdministrationController(
                queryBusMock.Object,
                usersServiceMock.Object,
                directMessagesServiceMock.Object,
                messagesServiceFactoryMock.Object,
                rolesServiceMock.Object,
                trustRolesServiceMock.Object,
                //checkUserSafetyServiceMock.Object,
                usersRolesServiceMock.Object,
                configurationServiceMock.Object,
                complaintsChannelServiceMock.Object);
        }

        internal MuteUserController CreateMuteUserController(
            Mock<ICommandBus> commandBusMock = null, Mock<IUnmutingService> unmutingServiceMock = null, 
            Mock<IUsersService> usersServiceMock = null, Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null)
        {
            commandBusMock ??= new Mock<ICommandBus>();
            unmutingServiceMock ??= new Mock<IUnmutingService>();
            usersServiceMock ??= new Mock<IUsersService>();
            messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();

            return new MuteUserController(
                commandBusMock.Object, 
                unmutingServiceMock.Object, 
                usersServiceMock.Object, 
                messagesServiceFactoryMock.Object);
        }

        internal AntiSpamController CreateAntiSpamController(
            Mock<IServerMessagesCacheService> serverMessagesCacheServiceMock = null, //Mock<ICheckUserSafetyService> checkUserSafetyServiceMock = null,
            Mock<IPunishmentsCachingService> punishmentsCachingServiceMock = null, Mock<IAntiSpamService> antiSpamServiceMock = null,
            Mock<IConfigurationService> configurationServiceMock = null, Mock<ISpamPunishmentStrategy> spamPunishmentStrategyMock = null,
            Mock<IOverallSpamDetectorStrategyFactory> overallSpamDetectorStrategyFactoryMock = null)
        {
            serverMessagesCacheServiceMock ??= new Mock<IServerMessagesCacheService>();
            //checkUserSafetyServiceMock ??= new Mock<ICheckUserSafetyService>();
            punishmentsCachingServiceMock ??= new Mock<IPunishmentsCachingService>();
            antiSpamServiceMock ??= new Mock<IAntiSpamService>();
            configurationServiceMock ??= new Mock<IConfigurationService>();
            spamPunishmentStrategyMock ??= new Mock<ISpamPunishmentStrategy>();
            overallSpamDetectorStrategyFactoryMock ??= new Mock<IOverallSpamDetectorStrategyFactory>();

            return new AntiSpamController(
                serverMessagesCacheServiceMock.Object,
                //checkUserSafetyServiceMock.Object,
                punishmentsCachingServiceMock.Object,
                antiSpamServiceMock.Object,
                configurationServiceMock.Object,
                spamPunishmentStrategyMock.Object,
                overallSpamDetectorStrategyFactoryMock.Object);
        }
        internal WarnsController CreateWarnsController(
            Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null, Mock<IUsersService> usersServiceMock = null,
            Mock<IWarnsService> warnsServiceMock = null)
        {
            messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();
            usersServiceMock ??= new Mock<IUsersService>();
            warnsServiceMock ??= new Mock<IWarnsService>();

            return new WarnsController(
                messagesServiceFactoryMock.Object,
                usersServiceMock.Object,
                warnsServiceMock.Object);
        }

        internal UsersController CreateUsersController(
            Mock<IQueryBus> queryBusMock = null, 
            Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null, 
            Mock<IRolesService> rolesServiceMock = null, 
            Mock<IUsersService> usersServiceMock = null, 
            Mock<Devscord.DiscordFramework.Commands.Responses.IResponsesService> responsesServiceMock = null)
        {
            queryBusMock ??= new Mock<IQueryBus>();
            messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();
            rolesServiceMock ??= new Mock<IRolesService>();
            usersServiceMock ??= new Mock<IUsersService>();
            responsesServiceMock ??= new Mock<Devscord.DiscordFramework.Commands.Responses.IResponsesService>();

            return new UsersController(
                queryBusMock.Object,
                messagesServiceFactoryMock.Object,
                rolesServiceMock.Object,
                usersServiceMock.Object,
                responsesServiceMock.Object);
        }

        internal ResponsesController CreateResponsesController(
            Mock<IMessagesServiceFactory> messagesServiceFactoryMock = null,
            Mock<Areas.Responses.Services.IResponsesService> responsesServiceMock = null,
            Mock<IResponsesMessageService> responsesMessageServiceMock = null)
        {
            messagesServiceFactoryMock ??= new Mock<IMessagesServiceFactory>();
            responsesServiceMock ??= new Mock<Areas.Responses.Services.IResponsesService>();
            responsesMessageServiceMock ??= new Mock<IResponsesMessageService>();
          
            return new ResponsesController(
                messagesServiceFactoryMock.Object,
                responsesServiceMock.Object,
                responsesMessageServiceMock.Object);
        }
    }
}
