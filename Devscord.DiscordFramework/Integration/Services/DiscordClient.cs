using Autofac;
using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Devscord.DiscordFramework.Integration.Services
{
    [ExcludeFromCodeCoverage]
    internal class DiscordClient : IDiscordClient
    {
        private bool _initialized;
        private readonly DiscordSocketClient _client;
        private readonly IComponentContext _context;

        public IDiscordClientMessagesService MessagesService { get; private set; }
        public IDiscordClientUsersService UsersService { get; private set; }
        public IDiscordClientChannelsService ChannelsService { get; private set; }
        public IDiscordClientRolesService RolesService { get; private set; }
        public IDiscordClientServersService ServersService { get; private set; }

        public DiscordClient(DiscordSocketClient client, IComponentContext context)
        {
            this._client = client;
            this._context = context;
            this.Initialize();
        }

        public void Initialize()
        {
            if (this._initialized)
            {
                return;
            }
            var serverContextFactory = this._context.Resolve<IDiscordServerContextFactory>();
            var userRoleFactory = this._context.Resolve<IUserRoleFactory>();
            var userContextFactory = this._context.Resolve<IUserContextsFactory>();
            var commandParser = this._context.Resolve<ICommandParser>();
            this.MessagesService = new DiscordClientMessagesService(this._client);
            this.UsersService = new DiscordClientUsersService(this._client);
            this.ChannelsService = new DiscordClientChannelsService(this._client, this.UsersService, userContextFactory, commandParser);
            this.RolesService = new DiscordClientRolesService(this._client, userRoleFactory);
            this.ServersService = new DiscordClientServersService(this._client, serverContextFactory);

            this._initialized = true;
            Log.Information("DiscordClient initialized");
        }
    }
}
