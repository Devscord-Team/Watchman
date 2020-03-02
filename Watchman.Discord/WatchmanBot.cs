using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services;
using Watchman.Integrations.MongoDB;
using Watchman.Discord.Ioc;
using Devscord.DiscordFramework;
using Autofac;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Help.Services;
using System.Text;
using System.Collections.Generic;
using Devscord.DiscordFramework.Commons.Extensions;
using System.Linq;
using Devscord.DiscordFramework.Commons.Exceptions;
using Watchman.Integrations.Logging;
using MongoDB.Driver;
using Serilog;

namespace Watchman.Discord
{
    public class WatchmanBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly Workflow _workflow;
        private readonly DiscordConfiguration _configuration;
        private readonly IContainer _container;

        public WatchmanBot(DiscordConfiguration configuration)
        {
            this._configuration = configuration;
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._client.MessageReceived += this.MessageReceived;

            this._container = GetAutofacContainer(configuration);
            Log.Logger = SerilogInitializer.Initialize(this._container.Resolve<IMongoDatabase>(), this.LogOnChannel);
            Log.Information("Bot created...");
            this._workflow = GetWorkflow(configuration, _container);
        }

        public async Task Start()
        {
            MongoConfiguration.Initialize();
            ServerInitializer.Initialize(_client, _container.Resolve<MessagesServiceFactory>());

            _ = Task.Run(DefaultHelpInit);
            
            await _client.LoginAsync(TokenType.Bot, this._configuration.Token);
            await _client.StartAsync();
            _client.Ready += UnmuteUsers;
            _client.Ready += () => Task.Run(() => Log.Information("Bot started and logged in..."));

            await Task.Delay(-1);
        }

        private void DefaultHelpInit()
        {
            var dataCollector = _container.Resolve<HelpDataCollectorService>();
            var helpService = _container.Resolve<HelpDBGeneratorService>();
            helpService.FillDatabase(dataCollector.GetCommandsInfo(typeof(WatchmanBot).Assembly));
        }

        private async Task UnmuteUsers()
        {
            var unmutingService = _container.Resolve<UnmutingExpiredMuteEventsService>();
            var serversService = _container.Resolve<DiscordServersService>();
            var servers = (await serversService.GetDiscordServers()).ToList();
            servers.ForEach(x => unmutingService.UnmuteUsersInit(x));
        }

        private Task MessageReceived(SocketMessage message)
        {
#if DEBUG
            if (!message.Channel.Name.Contains("test"))
                return Task.CompletedTask;
#endif
            return message.Author.IsBot ? Task.CompletedTask : this._workflow.Run(message);
        }

        private Workflow GetWorkflow(DiscordConfiguration configuration, IContainer context)
        {
            var workflow = new Workflow(typeof(WatchmanBot).Assembly, context)
                .AddMiddleware<ChannelMiddleware, ChannelContext>()
                .AddMiddleware<ServerMiddleware, DiscordServerContext>()
                .AddMiddleware<UserMiddleware, UserContext>();
            workflow.WorkflowException += this.LogException;
            workflow.WorkflowException += this.PrintExceptionOnConsole;
#if DEBUG
            workflow.WorkflowException += this.PrintDebugExceptionInfo;
#endif
            return workflow;
        }

        private void LogOnChannel(string message)
        {
            if(this._workflow != null)
            {
                try
                {
#if DEBUG
                    this._workflow.LogOnChannel(message, 684119569962631249);
#else
                    this._workflow.LogOnChannel(message, 681974777686261802);
                    this._workflow.LogOnChannel(message, 681990585837813796);
#endif
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Cannot find logs channel");
                }
            }
        }

        private void LogException(Exception e, Contexts contexts)
        {
            var messagesService = _container.Resolve<MessagesServiceFactory>().Create(contexts);

            var mostInnerException = e.InnerException ?? e;
            
            while (mostInnerException.InnerException != null)
            {
                mostInnerException = mostInnerException.InnerException;
            }

            switch (mostInnerException)
            {
                case NotAdminPermissionsException _:
                    messagesService.SendResponse(x => x.UserIsNotAdmin(), contexts);
                    break;
                case RoleNotFoundException roleExc:
                    messagesService.SendResponse(x => x.RoleNotFound(roleExc.RoleName), contexts);
                    break;
                case UserDidntMentionedAnyUserToMuteException _:
                    messagesService.SendResponse(x => x.UserDidntMentionedAnyUserToMute(), contexts);
                    break;
                case UserNotFoundException notFoundExc:
                    messagesService.SendResponse(x => x.UserNotFound(notFoundExc.Mention), contexts);
                    break;
                default:
                    messagesService.SendMessage("Wystąpił nieznany wyjątek");
                    break;
            }
        }

        private void PrintDebugExceptionInfo(Exception e, Contexts contexts)
        {
            var exceptionMessage = BuildExceptionMessage(e).ToString();

            var messagesService = _container.Resolve<MessagesServiceFactory>().Create(contexts);
            messagesService.SendMessage(exceptionMessage);
        }

        private void PrintExceptionOnConsole(Exception e, Contexts contexts)
        {
            var exceptionMessage = BuildExceptionMessage(e).ToString();
            Console.WriteLine(exceptionMessage);
        }

        private StringBuilder BuildExceptionMessage(Exception e)
        {
            return new StringBuilder($"{e.Message}\r\n\r\n{e.InnerException}\r\n\r\n{e.StackTrace}").FormatMessageIntoBlock();
        }

        private IContainer GetAutofacContainer(DiscordConfiguration configuration)
        {
            return new ContainerModule(configuration)
                .GetBuilder()
                .Build();
        }

        public void Dispose()
        {
            this._client.Dispose();
        }
    }
}
