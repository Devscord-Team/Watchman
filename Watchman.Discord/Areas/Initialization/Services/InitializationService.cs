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
    public interface IInitializationService
    {
        Task InitServer(DiscordServerContext server);
    }

    public class InitializationService : IInitializationService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly IMuteRoleInitService _muteRoleInitService;

        public InitializationService(IQueryBus queryBus, ICommandBus commandBus, IMuteRoleInitService muteRoleInitService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._muteRoleInitService = muteRoleInitService;
        }

        public async Task InitServer(DiscordServerContext server)
        {
            Log.Information("Initializing server: {server}", server.ToJson());
            await this._muteRoleInitService.InitForServerAsync(server);
            await this._commandBus.ExecuteAsync(new AddInitEventCommand(server.Id, endedAt: DateTime.UtcNow));
            Log.Information("Done server: {server}", server.ToJson());
        }
    }
}
