using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using Serilog;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;
using System.Collections;
using System.Collections.Generic;

namespace Devscord.DiscordFramework
{
    internal class Workflow
    {
        private readonly CommandParser _commandParser = new CommandParser();
        private readonly MiddlewaresService _middlewaresService = new MiddlewaresService();
        private readonly ControllersService _controllersService;

        public List<Func<Task>> OnReady { get; set; } = new List<Func<Task>>();
        public List<Func<Contexts, Task>> OnUserJoined { get; set; } = new List<Func<Contexts, Task>>();
        public List<Func<SocketMessage, Task>> OnMessageReceived { get; set; } = new List<Func<SocketMessage, Task>>();
        public List<Action<Exception, Contexts>> OnWorkflowException { get; set; } = new List<Action<Exception, Contexts>>();

        internal Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._controllersService = new ControllersService(context, botAssembly);
        }

        internal Workflow AddMiddleware<T, W>() 
            where T : IMiddleware<W>
            where W : IDiscordContext
        {
            this._middlewaresService.AddMiddleware<T, W>();
            Log.Debug("Added Middleware: {middlewareName} with DiscordContext: {contextName}", nameof(T), nameof(W));
            return this;
        }

        internal void Initialize()
        {
            this.OnMessageReceived.Add(this.MessageReceived);
        }

        internal void MapHandlers(DiscordSocketClient client)
        {
            this.OnReady.ForEach(x => client.Ready += x);
            this.OnMessageReceived.ForEach(x => client.MessageReceived += x);
            Server.UserJoined += CallUserJoined;
        }

        private async Task MessageReceived(SocketMessage socketMessage)
        {
            if(ShouldIgnoreMessage(socketMessage))
            {
                return;
            }

            Log.Information("Processing message: {content} from user {user} started", socketMessage.Content, socketMessage.Author);
            var request = _commandParser.Parse(socketMessage.Content, socketMessage.Timestamp.UtcDateTime);
            var contexts = this._middlewaresService.RunMiddlewares(socketMessage);
            try
            {
                await this._controllersService.Run(request, contexts);
            }
            catch (Exception e)
            {
                Log.Error(e, e.StackTrace);
                OnWorkflowException.ForEach(x => x.Invoke(e, contexts));
            }
        }

        private bool ShouldIgnoreMessage(SocketMessage socketMessage)
        {
#if DEBUG
            if (!socketMessage.Channel.Name.Contains("test"))
            {
                return true;
            }
#endif
            if (socketMessage.Author.IsBot || socketMessage.Author.IsWebhook)
            {
                return true;
            }
            if (socketMessage.Channel.Name.Contains("logs"))
            {
                return true;
            }
                
            return false;
        }

        private Task CallUserJoined(SocketGuildUser guildUser)
        {
            var userFactory = new UserContextsFactory();
            var serverFactory = new DiscordServerContextFactory();

            var userContext = userFactory.Create(guildUser);
            var discordServerContext = serverFactory.Create(guildUser.Guild);
            var landingChannel = discordServerContext.LandingChannel;

            var contexts = new Contexts();
            contexts.SetContext(userContext);
            contexts.SetContext(discordServerContext);
            contexts.SetContext(landingChannel);

            OnUserJoined.ForEach(x => x.Invoke(contexts));
            return Task.CompletedTask;
        }
    }
}
