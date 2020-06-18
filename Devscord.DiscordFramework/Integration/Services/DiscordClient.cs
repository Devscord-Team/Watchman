using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using Serilog;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClient : Interfaces.IDiscordClient
    {
        private bool _initialized;
        private readonly DiscordSocketClient _client;

        public IDiscordClientUsersService UsersService { get; private set; }
        public IDiscordClientChannelsService ChannelsService { get; private set; }
        public IDiscordClientRolesService RolesService { get; private set; }
        public IDiscordClientServersService ServersService { get; private set; }

        public DiscordClient(DiscordSocketClient client)
        {
            this._client = client;
            this.Initialize();
        }

        public void Initialize()
        {
            if (this._initialized)
            {
                return;
            }
            this.UsersService = new DiscordClientUsersService(this._client);
            this.ChannelsService = new DiscordClientChannelsService(this._client, this.UsersService);
            this.RolesService = new DiscordClientRolesService(this._client, this.ChannelsService);
            this.ServersService = new DiscordClientServersService(this._client);

            this._initialized = true;
            Log.Information("DiscordClient initialized");
        }
    }
}
