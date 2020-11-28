using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    internal class ControllersService
    {
        private readonly IComponentContext _context;
        private readonly Assembly _assembly;
        private readonly RunnerOfIBotCommandMethods _runnerOfIBotCommandMethods;
        private readonly CommandMethodValidator _commandMethodValidator;
        private ControllersContainer _controllersContainer;

        public ControllersService(IComponentContext context, Assembly assembly, RunnerOfIBotCommandMethods runnerOfIBotCommandMethods, CommandMethodValidator commandMethodValidator)
        {
            this._context = context;
            this._assembly = assembly;
            this._runnerOfIBotCommandMethods = runnerOfIBotCommandMethods;
            this._commandMethodValidator = commandMethodValidator;
        }

        public async Task Run(ulong messageId, DiscordRequest request, Contexts contexts)
        {
            if (this._controllersContainer == null)
            {
                this.LoadControllers();
            }

            using (LogContext.PushProperty("MessageId", messageId))
            using (LogContext.PushProperty("Request", JsonConvert.SerializeObject(request)))
            using (LogContext.PushProperty("SendByUserId", contexts.User.Id))
            using (LogContext.PushProperty("OnChannelId", contexts.Channel.Id))
            using (LogContext.PushProperty("OnServerId", contexts.Server.Id))
            {
                var readAlwaysMethods = this._controllersContainer.WithReadAlways;
                var readAlwaysTask = Task.Run(() => this.RunMethods(request, contexts, readAlwaysMethods, true));

                Task commandsTask = null;
                Task botCommandsTask = null;
                if (request.IsCommandForBot)
                {
                    var discordCommandMethods = this._controllersContainer.WithDiscordCommand;
                    commandsTask = Task.Run(() => this.RunMethods(request, contexts, discordCommandMethods, false));
                    var discordBotCommandMethods = this._controllersContainer.WithIBotCommand;
                    //TODO zoptymalizować
                    botCommandsTask = Task.Run(() => this._runnerOfIBotCommandMethods.RunMethodsIBotCommand(request, contexts, discordBotCommandMethods));
                }

                // ReadAlwaysMethods should be first in throwing exception, bcs every ReadAlways exception is Error
                await readAlwaysTask;
                if (commandsTask != null)
                {
                    await commandsTask;
                }
                if (botCommandsTask != null)
                {
                    await botCommandsTask;
                }
            }
        }

        private void LoadControllers()
        {
            var controllers = this._assembly.GetTypesByInterface<IController>()
                .Select(x => (IController)this._context.Resolve(x))
                .Select(x => new ControllerInfo(x))
                .ToList();
            this._controllersContainer = new ControllersContainer(controllers);
        }

        private void RunMethods(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)
        {
            var tasks = new List<Task>();
            foreach (var controllerInfo in controllers)
            {
                using (LogContext.PushProperty("Controller", controllerInfo.Controller.GetType().Name))
                {
                    foreach (var method in controllerInfo.Methods)
                    {
                        if (isReadAlways)
                        {
                            var task = InvokeMethod(request, contexts, controllerInfo, method);
                            tasks.Add(task);
                            continue;
                        }

                        var command = method.GetAttributeInstances<DiscordCommand>();
                        if (this.IsMatchedCommand(command, request) && this._commandMethodValidator.IsValid(contexts, method))
                        {
                            var task = InvokeMethod(request, contexts, controllerInfo, method);
                            tasks.Add(task);
                        }
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
        }

        private bool IsMatchedCommand(IEnumerable<DiscordCommand> commands, DiscordRequest request)
        {
            return commands.Any(x =>
            {
                var withoutPrefix = request.OriginalMessage.CutStart(request.Prefix);
                return withoutPrefix.StartsWith(x.Command)
                       && (withoutPrefix.Length == x.Command.Length || withoutPrefix[x.Command.Length] == ' ');
            });
        }

        private Task InvokeMethod(DiscordRequest request, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
        {
            Log.Information("Invoke in controller {controller} method {method}", controllerInfo.Controller.GetType().Name, method.Name);

            using (LogContext.PushProperty("Method", method.Name))
            {
                var runningMethod = method.Invoke(controllerInfo.Controller, new object[] { request, contexts });
                if (runningMethod is Task task)
                {
                    return task;
                }
            }
            return Task.CompletedTask;
        }
    }
}
