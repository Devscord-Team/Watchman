using Autofac;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;
using Serilog;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClient : Interfaces.IDiscordClient
    {
        private bool _initialized;
        private readonly DiscordSocketClient _client;
        private readonly IComponentContext _context;

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
            var serverContextFactory = this._context.Resolve<DiscordServerContextFactory>();
            this.UsersService = new DiscordClientUsersService(this._client);
            this.ChannelsService = new DiscordClientChannelsService(this._client, this.UsersService);
            this.RolesService = new DiscordClientRolesService(this._client, this.ChannelsService);
            this.ServersService = new DiscordClientServersService(this._client, serverContextFactory);

            this._initialized = true;
            Log.Information("DiscordClient initialized");
        }
    }
}
