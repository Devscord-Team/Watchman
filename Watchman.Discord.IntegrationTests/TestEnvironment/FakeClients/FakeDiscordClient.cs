using Autofac;
using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Integration.Services;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Factories;
using Serilog;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClient : IDiscordClient
    {
        public IDiscordClientMessagesService MessagesService { get; set; }
        public IDiscordClientUsersService UsersService { get; set; }
        public IDiscordClientChannelsService ChannelsService { get; set; }
        public IDiscordClientRolesService RolesService { get; set; }
        public IDiscordClientServersService ServersService { get; set; }

        public FakeDiscordClient(IDiscordClientMessagesService messagesService, IDiscordClientUsersService usersService, IDiscordClientChannelsService channelsService, 
            IDiscordClientRolesService rolesService, IDiscordClientServersService serversService)
        {
            this.MessagesService = messagesService;
            this.UsersService = usersService;
            this.ChannelsService = channelsService;
            this.RolesService = rolesService;
            this.ServersService = serversService;
        }
    }
}
