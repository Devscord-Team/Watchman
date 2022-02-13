using Autofac;
using Devscord.DiscordFramework;
using MongoDB.Driver;
using Serilog;
using Watchman.Integrations.Logging;
using Watchman.Integrations.Database.MongoDB;
using Watchman.Discord.Extensions;

namespace Watchman.Discord
{
    public class WatchmanBot
    {
        private readonly DiscordConfiguration _configuration;
        private readonly IComponentContext _context;

        public WatchmanBot(DiscordConfiguration configuration, IComponentContext context)
        {
            this._configuration = configuration;
            this._context = context;
            Log.Logger = SerilogInitializer.Initialize(this._context.Resolve<IMongoDatabase>());
            Log.Information("Bot created...");
        }

        public WorkflowBuilder GetWorkflowBuilder()
        {
            MongoConfiguration.Initialize();
            ExceptionHandlerService.DiscordConfiguration = this._configuration; //todo ioc

            return WorkflowBuilder
                .Create(this._configuration.Token, this._context.Resolve<IWorkflow>(), this._context)
                .SetDefaultMiddlewares()
                .SetOnReady()
                .SetOnUserJoined()
                .SetOnDiscordServerAddedBot()
                .SetOnWorkflowException()
                .SetOnChannelCreated()
                .SetOnChannelRemoved()
                .SetOnRoleRemoved();
        }
    }
}
