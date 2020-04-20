using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Serilog;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Statistics.Services;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private readonly MuteRoleInitService _muteRoleInitService;
        private readonly UsersRolesService _usersRolesService;
        private readonly ServerScanningService _serverScanningService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly CyclicStatisticsGeneratorService _cyclicStatisticsGeneratorService;
        private readonly ResponsesInitService _responsesInitService;

        public InitializationController(MuteRoleInitService muteRoleInitService, UsersRolesService usersRolesService, ServerScanningService serverScanningService, MessagesServiceFactory messagesServiceFactory, CyclicStatisticsGeneratorService cyclicStatisticsGeneratorService, ResponsesInitService responsesInitService)
        {
            this._muteRoleInitService = muteRoleInitService;
            this._usersRolesService = usersRolesService;
            _serverScanningService = serverScanningService;
            _messagesServiceFactory = messagesServiceFactory;
            _cyclicStatisticsGeneratorService = cyclicStatisticsGeneratorService;
            _responsesInitService = responsesInitService;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO //TODO co to za TODO?
        public async Task Init(DiscordRequest request, Contexts contexts)
        {
            await ResponsesInit(contexts.Server);
            await MuteRoleInit(contexts);
            await ReadServerMessagesHistory(contexts);
            await _cyclicStatisticsGeneratorService.GenerateStatsForDaysBefore(contexts.Server);
        }

        private async Task ResponsesInit(DiscordServerContext server)
        {
            await _responsesInitService.InitServerResponses(server);
        }

        private async Task MuteRoleInit(Contexts contexts)
        {
            var mutedRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, contexts.Server);

            if (mutedRole == null)
            {
                await _muteRoleInitService.InitForServer(contexts);
            }
            Log.Information("Mute role initialized");
        }

        private async Task ReadServerMessagesHistory(Contexts contexts)
        {
            Log.Information("Reading messages started");

            foreach (var textChannel in contexts.Server.TextChannels)
            {
                await _serverScanningService.ScanChannelHistory(contexts.Server, textChannel);
            }

            Log.Information("Read messages history");
            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.ReadingHistoryDone(), contexts);
        }
    }
}
