using Autofac;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Serilog.Context;
using System;
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
        private ControllersContainer _controllersContainer;

        public ControllersService(IComponentContext context, Assembly assembly)
        {
            this._context = context;
            this._assembly = assembly;
        }

        public async Task Run(DiscordRequest request, Contexts contexts)
        {
            if(this._controllersContainer == null)
            {
                this.LoadControllers();
            }

            using (LogContext.PushProperty("MessageId", Guid.NewGuid()))
            using (LogContext.PushProperty("Request", request))
            using (LogContext.PushProperty("Contexts", contexts))
            {
                var readAlwaysMethods = this._controllersContainer.WithReadAlways;
                var readAlwaysTask = Task.Run(() => RunMethods(request, contexts, readAlwaysMethods, true));
                
                Task commandsTask = null;
                if (request.IsCommandForBot)
                {
                    var discordCommandMethods = this._controllersContainer.WithDiscordCommand;
                    commandsTask = Task.Run(() => RunMethods(request, contexts, discordCommandMethods, false));
                }

                // ReadAlwaysMethods should be first in throwing exception, bcs every ReadAlways exception is Error
                await readAlwaysTask;
                if (commandsTask != null)
                {
                    await commandsTask;
                }
            }
        }

        private void LoadControllers()
        {
            var controllers = _assembly.GetTypesByInterface<IController>()
                .Select(x => (IController)_context.Resolve(x))
                .Select(x => new ControllerInfo(x))
                .ToList();
            this._controllersContainer = new ControllersContainer(controllers);
        }

        private void RunMethods(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)
        {
            Parallel.ForEach(controllers, controllerInfo => 
            {
                using (LogContext.PushProperty("Controller", controllerInfo.Controller.GetType().Name))
                {
                    foreach(var method in controllerInfo.Methods)
                    {
                        if (isReadAlways)
                        {
                            InvokeMethod(request, contexts, controllerInfo, method);
                            return;
                        }

                        var command = method.GetAttributeInstances<DiscordCommand>();
                        if (IsValid(contexts, method) && IsMatchedCommand(command, request))
                        {
                            InvokeMethod(request, contexts, controllerInfo, method);
                        }
                    }
                }
            });
        }

        private bool IsValid(Contexts contexts, MethodInfo method)
        {
            CheckPermissions(method, contexts);
            return true;
        }

        private bool IsMatchedCommand(IEnumerable<DiscordCommand> commands, DiscordRequest request)
        {
            return commands.Any(x => request.Name == x.Command || request.OriginalMessage.TrimStart(request.Prefix.ToCharArray()).StartsWith(x.Command));
        }

        private void CheckPermissions(MethodInfo method, Contexts contexts)
        {
            Log.Information("Checking permissions for user {user} for method {method}", contexts.User.ToString(), method.Name);
            if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin)
            {
                throw new NotAdminPermissionsException();
            }
            Log.Information("User {user} have permissions for method {method}", contexts.User.ToString(), method.Name);
        }

        private static void InvokeMethod(DiscordRequest request, Contexts contexts, ControllerInfo controllerInfo, MethodInfo method)
        {
            Log.Information("Invoke in controller {controller} method {method}", controllerInfo.Controller.GetType().Name, method.Name);

            using (LogContext.PushProperty("Method", method.Name))
            {
                method.Invoke(controllerInfo.Controller, new object[] { request, contexts });
            }
        }
    }
}
