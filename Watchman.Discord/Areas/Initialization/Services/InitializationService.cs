using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Serilog;
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
        private readonly ResponsesInitService _responsesInitService;

        public InitializationService(IQueryBus queryBus, ICommandBus commandBus, MuteRoleInitService muteRoleInitService, UsersRolesService usersRolesService, ServerScanningService serverScanningService, CyclicStatisticsGeneratorService cyclicStatisticsGeneratorService, ResponsesInitService responsesInitService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _muteRoleInitService = muteRoleInitService;
            _usersRolesService = usersRolesService;
            _serverScanningService = serverScanningService;
            _cyclicStatisticsGeneratorService = cyclicStatisticsGeneratorService;
            _responsesInitService = responsesInitService;
        }

        public async Task InitServer(DiscordServerContext server)
        {
            //await ResponsesInit(server);
            await MuteRoleInit(server);
            var lastInitDate = GetLastInitDate(server);
            await ReadServerMessagesHistory(server, lastInitDate);
            await _cyclicStatisticsGeneratorService.GenerateStatsForDaysBefore(server, lastInitDate);
            await NotifyDomainAboutInit(server);
        }

        //private async Task ResponsesInit(DiscordServerContext server)
        //{
        //    //await _responsesInitService.InitServerResponses(server);
        //}

        private async Task MuteRoleInit(DiscordServerContext server)
        {
            var mutedRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, server);

            if (mutedRole == null)
            {
                await _muteRoleInitService.InitForServer(server);
            }

            Log.Information($"Mute role initialized: {server.Name}");
        }

        private async Task ReadServerMessagesHistory(DiscordServerContext server, DateTime lastInitDate)
        {
            foreach (var textChannel in server.TextChannels)
            {
                await _serverScanningService.ScanChannelHistory(server, textChannel, lastInitDate);
            }

            Log.Information($"Read messages history: {server.Name}");
        }

        private DateTime GetLastInitDate(DiscordServerContext server)
        {
            var query = new GetInitEventsQuery(server.Id);
            var initEvents = _queryBus.Execute(query).InitEvents.ToList();
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
            await _commandBus.ExecuteAsync(command);
        }
    }
}
