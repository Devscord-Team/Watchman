using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Discord.Areas.Protection.BotCommands;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AdministrationController
    {
        private readonly ChannelsService _channelsService;
        private readonly DiscordServersService _discordServersService;

        public AdministrationController(ChannelsService channelsService, DiscordServersService discordServersService)
        {
            this._channelsService = channelsService;
            this._discordServersService = discordServersService;
        }

        [AdminCommand]
        public async Task CreateChannelForComplaints(ComplaintsChannelCommand command, Contexts contexts)
        {
            var complaintsChannel = await this._channelsService.CreateNewChannelAsync(contexts.Server, "skargi");
            //this._channelsService.SetPermissions(complaintsChannel, contexts.Server, );
            // creates channel
            // set perms for @everyone to not read the channel
            // set perms for roles with Manage Server to read the channel
            // set perms for muted to read the channel
            // add the channel's ID to a base
            // assing action to ChannelRemoved to remove complaintsChannelId from the base, when the channel is deleted
        }
    }
}
