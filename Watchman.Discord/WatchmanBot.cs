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
using Watchman.Discord.Areas.Help.Services;
using System.Text;

namespace Watchman.Discord
{
    public class WatchmanBot : IDisposable
    {
        private readonly DiscordSocketClient _client;
        private readonly Workflow _workflow;
        private readonly DiscordConfiguration _configuration;

        public WatchmanBot(DiscordConfiguration configuration)
        {
            this._configuration = configuration;
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._client.MessageReceived += this.MessageReceived;

            var autofacContainer = GetAutofacContainer(configuration);
            this._workflow = GetWorkflow(configuration, autofacContainer);

            var dataCollector = autofacContainer.Resolve<HelpDataCollectorService>();
            var helpService = autofacContainer.Resolve<HelpDBGeneratorService>();

            helpService.FillDatabase(dataCollector.GetCommandsInfo(typeof(WatchmanBot).Assembly));
        }

        public async Task Start()
        {
            MongoConfiguration.Initialize();
            ServerInitializer.Initialize(this._client);

            await _client.LoginAsync(TokenType.Bot, this._configuration.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task MessageReceived(SocketMessage message)
        {
#if DEBUG
            if (!message.Channel.Name.Contains("test"))
                return Task.CompletedTask;
#endif
            return message.Author.IsBot ? Task.CompletedTask : this._workflow.Run(message);
        }

        private void LogException(Exception e, SocketMessage socketMessage)
        {
            var messagesService = new MessagesService()
            {
                ChannelId = socketMessage.Channel.Id
            };

            if (e is System.Reflection.TargetInvocationException)
            {
                messagesService.SendMessage("Wystąpił wyjątek");
            }
#if DEBUG
            var exceptionMessageBuilder = new StringBuilder();
            exceptionMessageBuilder.AppendLine($"```Command: {socketMessage.Content}```");
            exceptionMessageBuilder.AppendLine($"```Message: {e.Message}```");
            exceptionMessageBuilder.AppendLine($"```InnerException message: {e.InnerException?.Message}```");
            exceptionMessageBuilder.AppendLine($"```InnerException2 message: {e.InnerException?.InnerException?.Message}```");
            messagesService.SendMessage(exceptionMessageBuilder.ToString());
#endif
        }

        private Workflow GetWorkflow(DiscordConfiguration configuration, IContainer context)
        {
            var workflow = new Workflow(typeof(WatchmanBot).Assembly, context);
            workflow.AddMiddleware<ChannelMiddleware>()
                .AddMiddleware<ServerMiddleware>()
                .AddMiddleware<UserMiddleware>();
            workflow.WorkflowException += this.LogException;
            return workflow;
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
