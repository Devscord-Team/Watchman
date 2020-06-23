using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.BotCommands;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.Discord.Areas.UselessFeatures.BotCommands;
using Watchman.DomainModel.CustomCommands;
using Watchman.DomainModel.CustomCommands.Commands;
using Watchman.DomainModel.CustomCommands.Queries;
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
            _queryBus = queryBus;
            _commandBus = commandBus;
            _muteRoleInitService = muteRoleInitService;
            _usersRolesService = usersRolesService;
            _serverScanningService = serverScanningService;
            _cyclicStatisticsGeneratorService = cyclicStatisticsGeneratorService;
        }

        public async Task InitServer(DiscordServerContext server)
        {
            await AddDefaultCustomCommands();
            await MuteRoleInit(server);
            var lastInitDate = GetLastInitDate(server);
            await ReadServerMessagesHistory(server, lastInitDate);
            await _cyclicStatisticsGeneratorService.GenerateStatsForDaysBefore(server, lastInitDate);
            await NotifyDomainAboutInit(server);
        }

        private async Task AddDefaultCustomCommands()
        {
            var customCommands = new List<AddCustomCommandsCommand>();
            foreach (var serverId in new ulong[] { 636238466899902504, 597066406521208852 })
            {
                customCommands.AddRange(new List<AddCustomCommandsCommand>
                {
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.AddRoleCommand", @"-add\s*role\s*(?<Roles>[\w\s\""]*)", serverId),
                    new AddCustomCommandsCommand("Watchman.Discord.Areas.Users.BotCommands.RemoveRoleCommand", @"-remove\s*role\s*(?<Roles>[\w\s\""]*)", serverId)
                });
            }

            var commandsInBase = this._queryBus.Execute(new GetCustomCommandsQuery()).CustomCommands.ToList();
            foreach (var command in customCommands)
            {
                if (commandsInBase.Any(x => x.ServerId == command.ServerId && x.CommandFullName == command.CommandFullName))
                {
                    continue;
                }
                await this._commandBus.ExecuteAsync(command);
            }
        }

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
