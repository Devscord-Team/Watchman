using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Serilog;
using Devscord.DiscordFramework.Integration.Services.Interfaces;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClient : Interfaces.IDiscordClient
    {
        private bool _initialized;
        private DiscordSocketClient _client;

        public IDiscordClientUsersService UsersService { get; private set; }
        public IDiscordClientChannelsService ChannelsService { get; private set; }
        public IDiscordClientRolesService RolesService { get; private set; }
        public IDiscordClientServersService ServersService { get; private set; }

        public DiscordClient(DiscordSocketClient client)
        {
            _client = client;
            this.Initialize();
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            UsersService = new DiscordClientUsersService(_client);
            ChannelsService = new DiscordClientChannelsService(_client, UsersService);
            RolesService = new DiscordClientRolesService(_client, ChannelsService);
            ServersService = new DiscordClientServersService(_client);

            _initialized = true;
            Log.Information("DiscordClient initialized");
        }
    }
}
