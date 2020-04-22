using Autofac;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Devscord.DiscordFramework.Middlewares;

namespace Devscord.DiscordFramework
{
    public class WorkflowBuilder
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly IComponentContext context;
        private readonly Workflow _workflow;

        private WorkflowBuilder(string token, IComponentContext context, Assembly botAssembly)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._token = token;
            this.context = context;
            this._workflow = new Workflow(botAssembly, context);
        }

        public static WorkflowBuilder Create(string token, IComponentContext context, Assembly botAssembly) => new WorkflowBuilder(token, context, botAssembly);

        public WorkflowBuilder SetMessageHandler(Func<SocketMessage, Task> action)
        {
            _client.MessageReceived += action;
            return this;
        }

        public WorkflowBuilder SetDefaultMiddlewares()
        {
            this._workflow
                .AddMiddleware<ChannelMiddleware>()
                .AddMiddleware<ServerMiddleware>()
                .AddMiddleware<UserMiddleware>();
            return this;
        }

        public WorkflowBuilder AddOnReadyHandlers(Action<WorkflowBuilderHandlers<Func<Task>>> action)
        {
            AddHandlers(action, this._workflow.OnReady.Add);
            return this;
        }

        public WorkflowBuilder AddOnUserJoinedHandlers(Action<WorkflowBuilderHandlers<Func<Contexts, Task>>> action)
        {
            AddHandlers(action, this._workflow.OnUserJoined.Add);
            return this;
        }

        public WorkflowBuilder AddOnMessageReceivedHandlers(Action<WorkflowBuilderHandlers<Func<SocketMessage, Task>>> action)
        {
            AddHandlers(action, this._workflow.OnMessageReceived.Add);
            return this;
        }

        public WorkflowBuilder AddOnDiscordServerAddedBot(Action<WorkflowBuilderHandlers<Func<DiscordServerContext, Task>>> action)
        {
            AddHandlers(action, this._workflow.OnDiscordServerAddedBot.Add);
            return this;
        }

        public WorkflowBuilder AddOnWorkflowExceptionHandlers(Action<WorkflowBuilderHandlers<Action<Exception, Contexts>>> action)
        {
            AddHandlers(action, this._workflow.OnWorkflowException.Add);
            return this;
        }

        private void AddHandlers<T>(Action<WorkflowBuilderHandlers<T>> action, Action<T> workflowAction)
        {
            var workflowBuilderHandlers = new WorkflowBuilderHandlers<T>(this.context);
            action.Invoke(workflowBuilderHandlers);
            foreach (var exceptionHandler in workflowBuilderHandlers.Handlers)
            {
                workflowAction.Invoke(exceptionHandler);
            }
        }

        public WorkflowBuilder Build()
        {
            _workflow.Initialize();
            _workflow.MapHandlers(_client);

            _client.LoginAsync(TokenType.Bot, _token).Wait();
            _client.StartAsync().Wait();

            ServerInitializer.Initialize(_client);
            return this;
        }

        public async Task Run()
        {
            await Task.Delay(-1);
        }
    }
}
