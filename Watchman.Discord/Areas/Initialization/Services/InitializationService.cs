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
        private readonly IMuteRoleInitService _muteRoleInitService;
        private readonly IServerScanningService _serverScanningService;

        public InitializationService(IQueryBus queryBus, ICommandBus commandBus, IMuteRoleInitService muteRoleInitService, IServerScanningService serverScanningService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._muteRoleInitService = muteRoleInitService;
            this._serverScanningService = serverScanningService;
        }

        public async Task InitServer(DiscordServerContext server)
        {
            Log.Information("Initializing server: {server}", server.ToJson());
            await this._muteRoleInitService.InitForServerAsync(server);
            var lastInitDate = this.GetLastInitDate(server); foreach (var textChannel in server.GetTextChannels())
            {
                await this._serverScanningService.ScanChannelHistory(server, textChannel, lastInitDate);
            }
            await this._commandBus.ExecuteAsync(new AddInitEventCommand(server.Id, endedAt: DateTime.UtcNow));
            Log.Information("Done server: {server}", server.ToJson());
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
    }
}
