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
using Discord;
using Devscord.DiscordFramework.Middlewares;
using System.Collections;
using System.Collections.Generic;

namespace Devscord.DiscordFramework
{
    internal class Workflow
    {
        private readonly CommandParser _commandParser = new CommandParser();
        private readonly MiddlewaresService _middlewaresService = new MiddlewaresService();
        private readonly ControllersService _controllersService;

        public Func<Task> OnReady { get; set; }
        public Func<Contexts, Task> OnUserJoined { get; set; }
        public Func<SocketMessage, Task> OnMessageReceived { get; set; }
        public Action<Exception, Contexts> OnWorkflowException { get; set; }

        internal Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._controllersService = new ControllersService(context, botAssembly);
            Server.UserJoined += CallUserJoined;
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
            this.OnMessageReceived += MessageReceived;
        }

        internal void MapHandlers(DiscordSocketClient client)
        {
            client.Ready += this.OnReady;
            client.UserJoined += this.CallUserJoined;
            client.MessageReceived += this.OnMessageReceived;
        }

        private async Task MessageReceived(SocketMessage socketMessage)
        {
#if DEBUG
            if (!socketMessage.Channel.Name.Contains("test"))
                return;
#endif
            if (socketMessage.Author.IsBot || socketMessage.Author.IsWebhook)
                return;
            if (socketMessage.Channel.Name.Contains("logs"))
                return;

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
                OnWorkflowException.Invoke(e, contexts);
            }
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

            OnUserJoined.Invoke(contexts);
            return Task.CompletedTask;
        }
    }

    public class WorkflowBuilder
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly IContainer _container;
        private readonly Workflow _workflow;

        private WorkflowBuilder(string token, IContainer container, Assembly botAssembly)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._token = token;
            this._container = container;
            this._workflow = new Workflow(botAssembly, container);
            
        }

        public static WorkflowBuilder Create(string token, IContainer container, Assembly botAssembly) => new WorkflowBuilder(token, container, botAssembly);

        public WorkflowBuilder SetMessageHandler(Func<SocketMessage, Task> action)
        {
            _client.MessageReceived += action;
            return this;
        }

        public WorkflowBuilder SetDefaultMiddlewares()
        {
            this._workflow
                .AddMiddleware<ChannelMiddleware, ChannelContext>()
                .AddMiddleware<ServerMiddleware, DiscordServerContext>()
                .AddMiddleware<UserMiddleware, UserContext>();
            return this;
        }

        public WorkflowBuilder AddOnReadyHandlers(Action<WorkflowBuilderHandlers<Func<Task>>> action)
        {
            AddHandlers(action, this._workflow.OnReady);
            return this;
        }

        public WorkflowBuilder AddOnUserJoinedHandlers(Action<WorkflowBuilderHandlers<Func<Contexts, Task>>> action)
        {
            AddHandlers(action, this._workflow.OnUserJoined);
            return this;
        }

        public WorkflowBuilder AddOnMessageReceivedHandlers(Action<WorkflowBuilderHandlers<Func<SocketMessage, Task>>> action)
        {
            AddHandlers(action, this._workflow.OnMessageReceived);
            return this;
        }

        public WorkflowBuilder AddOnWorkflowExceptionHandlers(Action<WorkflowBuilderHandlers<Action<Exception, Contexts>>> action)
        {
            AddHandlers(action, this._workflow.OnWorkflowException);
            return this;
        }

        private void AddHandlers<T>(Action<WorkflowBuilderHandlers<T>> action, T workflowAction)
        {
            var workflowBuilderHandlers = new WorkflowBuilderHandlers<T>(this._container);
            action.Invoke(workflowBuilderHandlers);
            foreach (var exceptionHandler in workflowBuilderHandlers.Handlers)
            {
                workflowAction += (dynamic)exceptionHandler;
            }
        }

        public async Task Run()
        {
            _workflow.Initialize();
            _workflow.MapHandlers(_client);
            ServerInitializer.Initialize(_client);

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }
    }

    public class WorkflowBuilderHandlers<T>
    {
        private List<T> _handlers = new List<T>();
        private readonly IContainer _container;

        internal IEnumerable<T> Handlers => _handlers;

        public WorkflowBuilderHandlers(IContainer container)
        {
            this._container = container;
        }

        public WorkflowBuilderHandlers<T> AddHandler(T handler, bool onlyOnDebug = false)
        {
            var isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            if(!onlyOnDebug || (onlyOnDebug && isDebug))
            {
                _handlers.Add(handler);
            }
            return this;
        }

        public WorkflowBuilderHandlers<T> AddFromIoC<W>(Func<W, T> func)
        {
            var resolved = _container.Resolve<W>();
            var handler = func.Invoke(resolved);
            AddHandler(handler);
            return this;
        }
    }
}
