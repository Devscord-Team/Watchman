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
    public class Workflow
    {
        private readonly CommandParser _commandParser = new CommandParser();
        private readonly MiddlewaresService _middlewaresService = new MiddlewaresService();
        private readonly ControllersService _controllersService;

        public Func<Contexts, Task> UserJoined { get; set; }
        public Action<Exception, Contexts> WorkflowException { get; set; }

        public Workflow(Assembly botAssembly, IComponentContext context)
        {
            this._controllersService = new ControllersService(context, botAssembly);
            Server.UserJoined += CallUserJoined;
        }

        public Workflow AddMiddleware<T, W>() 
            where T : IMiddleware<W>
            where W : IDiscordContext
        {
            this._middlewaresService.AddMiddleware<T, W>();
            Log.Debug("Added Middleware: {middlewareName} with DiscordContext: {contextName}", nameof(T), nameof(W));
            return this;
        }

        public async Task Run(SocketMessage socketMessage)
        {
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
                WorkflowException.Invoke(e, contexts);
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

            UserJoined.Invoke(contexts);
            return Task.CompletedTask;
        }
    }

    public class WorkflowBuilder
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly Workflow _workflow;
        private readonly WorkflowBuilderExceptions _workflowBuilderExceptions;

        private WorkflowBuilder(string token, IContainer container, Assembly botAssembly)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._token = token;
            this._workflowBuilderExceptions = new WorkflowBuilderExceptions(container);
            this._workflow = new Workflow(botAssembly, container);
            ServerInitializer.Initialize(_client);
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

        public WorkflowBuilder AddWorkflowExceptionHandlers(Action<WorkflowBuilderExceptions> action)
        {
            action.Invoke(_workflowBuilderExceptions);
            foreach (var exceptionHandler in _workflowBuilderExceptions.Handlers)
            {
                _workflow.WorkflowException += exceptionHandler;
            }
            _workflowBuilderExceptions.Clear();
            return this;
        }

        public async Task Run()
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }
    }

    public class WorkflowBuilderExceptions
    {
        private List<Action<Exception, Contexts>> _handlers = new List<Action<Exception, Contexts>>();
        private readonly IContainer _container;

        internal IEnumerable<Action<Exception, Contexts>> Handlers => _handlers;

        public WorkflowBuilderExceptions(IContainer container)
        {
            this._container = container;
        }

        public WorkflowBuilderExceptions AddHandler(Action<Exception, Contexts> handler, bool onlyOnDebug = false)
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

        public WorkflowBuilderExceptions AddFromIoC<T>(Func<T, Action<Exception, Contexts>> func)
        {
            var resolved = _container.Resolve<T>();
            var handler = func.Invoke(resolved);
            AddHandler(handler);
            return this;
        }

        internal void Clear()
        {
            _handlers = new List<Action<Exception, Contexts>>();
        }
    }
}
