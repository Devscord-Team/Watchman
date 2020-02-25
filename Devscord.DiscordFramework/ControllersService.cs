using Autofac;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        private void LoadControllers()
        {
            var controllers = _assembly.GetTypesByInterface<IController>()
                .Select(x => (IController)_context.Resolve(x))
                .Select(x => new ControllerInfo(x));
            this._controllersContainer = new ControllersContainer(controllers);
        }

        public void Run(DiscordRequest request, Contexts contexts)
        {
            if(this._controllersContainer == null)
            {
                this.LoadControllers();
            }
            var readAlwaysMethods = this._controllersContainer.WithReadAlways;
            RunMethods(request, contexts, readAlwaysMethods, true);
            if(!request.IsCommandForBot)
            {
                var discordCommandMethods = this._controllersContainer.WithDiscordCommand;
                RunMethods(request, contexts, discordCommandMethods, false);
            }
            
        }

        //todo parallel and async
        private void RunMethods(DiscordRequest request, Contexts contexts, IEnumerable<ControllerInfo> controllers, bool isReadAlways)
        {
            foreach (var controllerInfo in controllers)
            {
                foreach (var method in controllerInfo.Methods)
                {
                    if (!isReadAlways && !IsValid(request, contexts, method))
                    {
                        continue;
                    }
                    Log.Information("Invoke in controller {controller} method {method}", nameof(controllerInfo.Controller), method.Name);
                    method.Invoke(controllerInfo.Controller, new object[] { request, contexts });
                }
            }
        }

        private bool IsValid(DiscordRequest request, Contexts contexts, MethodInfo method)
        {
            if (!IsMatchedCommand(method.GetAttributeInstances<DiscordCommand>(), request))
            {
                Log.Warning("Command {command} is parsed correctly, but not recognized as a command", request.OriginalMessage);
                return false;
            }
            CheckPermissions(method, contexts);
            return true;
        }

        private bool IsMatchedCommand(IEnumerable<DiscordCommand> commands, DiscordRequest request)
        {
            return commands.Any(x => request.Name == x.Command || request.OriginalMessage.TrimStart(request.Prefix.ToCharArray()).StartsWith(x.Command));
        }

        private void CheckPermissions(MethodInfo method, Contexts contexts)
        {
            Log.Information("Checking permissions for user {user}", contexts.User.ToString());
            if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin)
            {
                throw new NotAdminPermissionsException();
            }
        }

    }
}
