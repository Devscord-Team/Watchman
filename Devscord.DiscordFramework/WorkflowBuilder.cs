using Autofac;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public class WorkflowBuilder
    {
        public static List<DateTime> ConnectedTimes => Server.ConnectedTimes;
        public static List<DateTime> DisconnectedTimes => Server.DisconnectedTimes;

        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly IComponentContext _context;
        private readonly Workflow _workflow;

        private WorkflowBuilder(string token, IComponentContext context, Assembly botAssembly)
        {
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                TotalShards = 1
            });
            this._token = token;
            this._context = context;
            this._workflow = new Workflow(botAssembly, context);
        }

        public static WorkflowBuilder Create(string token, IComponentContext context, Assembly botAssembly)
        {
            return new WorkflowBuilder(token, context, botAssembly);
        }

        public WorkflowBuilder SetMessageHandler(Func<SocketMessage, Task> action)
        {
            this._client.MessageReceived += action;
            return this;
        }

        public WorkflowBuilder SetDefaultMiddlewares()
        {
            this._workflow
                .AddMiddleware<ChannelMiddleware>()
                .AddMiddleware<ServerMiddleware>()
                .AddMiddleware<UserMiddleware>();
            Log.Debug("Default middlewares added");
            return this;
        }

        public WorkflowBuilder AddCustomMiddleware<T>() where T : IMiddleware
        {
            this._workflow.AddMiddleware<T>();
            Log.Debug("Custom middleware {middleware} added", nameof(T));
            return this;
        }

        public WorkflowBuilder AddOnReadyHandlers(Action<WorkflowBuilderHandlers<Func<Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnReady.Add);
            Log.Debug("OnReady handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnUserJoinedHandlers(Action<WorkflowBuilderHandlers<Func<Contexts, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnUserJoined.Add);
            Log.Debug("OnUserJoined handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnMessageReceivedHandlers(Action<WorkflowBuilderHandlers<Func<SocketMessage, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnMessageReceived.Add);
            Log.Debug("OnMessageReceived handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnDiscordServerAddedBotHandlers(Action<WorkflowBuilderHandlers<Func<DiscordServerContext, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnDiscordServerAddedBot.Add);
            Log.Debug("OnDiscordServerAddedBot handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnChannelCreatedHandlers(Action<WorkflowBuilderHandlers<Func<ChannelContext, DiscordServerContext, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnChannelCreated.Add);
            Log.Debug("OnChannelCreated handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnRoleUpdatedHandlers(Action<WorkflowBuilderHandlers<Func<UserRole, UserRole, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnRoleUpdated.Add);
            Log.Debug("OnRoleUpdated handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnRoleCreatedHandlers(Action<WorkflowBuilderHandlers<Func<UserRole, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnRoleCreated.Add);
            Log.Debug("OnRoleCreated handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnRoleRemovedHandlers(Action<WorkflowBuilderHandlers<Func<UserRole, Task>>> action)
        {
            this.AddHandlers(action, this._workflow.OnRoleRemoved.Add);
            Log.Debug("OnRoleRemoved handlers have been set");
            return this;
        }

        public WorkflowBuilder AddOnWorkflowExceptionHandlers(Action<WorkflowBuilderHandlers<Action<Exception, Contexts>>> action)
        {
            this.AddHandlers(action, this._workflow.OnWorkflowException.Add);
            Log.Debug("OnWorkflowException handlers have been set");
            return this;
        }

        private void AddHandlers<T>(Action<WorkflowBuilderHandlers<T>> action, Action<T> workflowAction)
        {
            var workflowBuilderHandlers = new WorkflowBuilderHandlers<T>(this._context);
            action.Invoke(workflowBuilderHandlers);
            foreach (var handler in workflowBuilderHandlers.Handlers)
            {
                workflowAction.Invoke(handler);
            }
        }

        public WorkflowBuilder Build()
        {
            this._workflow.Initialize();
            this._workflow.MapHandlers(this._client);

            this._client.LoginAsync(TokenType.Bot, this._token).Wait();
            this._client.StartAsync().Wait();

            ServerInitializer.Initialize(this._client);
            return this;
        }
    }
}
