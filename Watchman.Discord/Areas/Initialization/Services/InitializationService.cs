using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Extensions;
using Watchman.Cqrs;
using Watchman.DomainModel.Configuration.Commands;
using Watchman.DomainModel.Configuration.Queries;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class InitializationService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MuteRoleInitService _muteRoleInitService;
        private readonly ServerScanningService _serverScanningService;

        public InitializationService(IQueryBus queryBus, ICommandBus commandBus, MuteRoleInitService muteRoleInitService, ServerScanningService serverScanningService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._muteRoleInitService = muteRoleInitService;
            this._serverScanningService = serverScanningService;
        }

        public async Task InitServer(DiscordServerContext server)
        {
            Log.Information("Initializing server: {server}", server.ToJson());
            await this.MuteRoleInit(server);
            var lastInitDate = this.GetLastInitDate(server);
            await this.ReadServerMessagesHistory(server, lastInitDate);
            await this.NotifyDomainAboutInit(server);
            Log.Information("Done server: {server}", server.ToJson());
        }

        private async Task MuteRoleInit(DiscordServerContext server)
        {
            await this._muteRoleInitService.InitForServerAsync(server);
            Log.Information("Mute role initialized: {server}", server.Name);
        }

        private async Task ReadServerMessagesHistory(DiscordServerContext server, DateTime lastInitDate)
        {
            foreach (var textChannel in server.GetTextChannels())
            {
                await this._serverScanningService.ScanChannelHistory(server, textChannel, lastInitDate);
            }
            Log.Information("Read messages history: {server}", server.Name);
        }

        private DateTime GetLastInitDate(DiscordServerContext server)
        {
            var query = new GetInitEventsQuery(server.Id);
            var initEvents = this._queryBus.Execute(query).InitEvents.ToList();
            if (!initEvents.Any())
            {
                return DateTime.UnixEpoch;
            }

            var lastInitEvent = initEvents.Max(x => x.EndedAt);
            return lastInitEvent;
        }

        private async Task NotifyDomainAboutInit(DiscordServerContext server)
        {
            var command = new AddInitEventCommand(server.Id, endedAt: DateTime.UtcNow);
            await this._commandBus.ExecuteAsync(command);
        }
    }
}
