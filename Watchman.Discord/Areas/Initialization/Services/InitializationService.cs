using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.DomainModel.Settings.Commands;
using Watchman.DomainModel.Settings.Queries;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class InitializationService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MuteRoleInitService _muteRoleInitService;
        private readonly UsersRolesService _usersRolesService;
        private readonly ServerScanningService _serverScanningService;
        private readonly CyclicStatisticsGeneratorService _cyclicStatisticsGeneratorService;

        public InitializationService(IQueryBus queryBus, ICommandBus commandBus, MuteRoleInitService muteRoleInitService, UsersRolesService usersRolesService, ServerScanningService serverScanningService, CyclicStatisticsGeneratorService cyclicStatisticsGeneratorService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._muteRoleInitService = muteRoleInitService;
            this._usersRolesService = usersRolesService;
            this._serverScanningService = serverScanningService;
            this._cyclicStatisticsGeneratorService = cyclicStatisticsGeneratorService;
        }

        public async Task InitServer(DiscordServerContext server)
        {
            await this.MuteRoleInit(server);
            var lastInitDate = this.GetLastInitDate(server);
            await this.ReadServerMessagesHistory(server, lastInitDate);
            await this._cyclicStatisticsGeneratorService.GenerateStatsForDaysBefore(server, lastInitDate);
            await this.NotifyDomainAboutInit(server);
        }

        private async Task MuteRoleInit(DiscordServerContext server)
        {
            var mutedRole = this._usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (mutedRole == null)
            {
                await this._muteRoleInitService.InitForServer(server);
            }

            Log.Information("Mute role initialized: {server}", server.Name);
        }

        private async Task ReadServerMessagesHistory(DiscordServerContext server, DateTime lastInitDate)
        {
            foreach (var textChannel in server.TextChannels)
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
