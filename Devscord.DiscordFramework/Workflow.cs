using Autofac;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework
{
    public class Workflow
    {
        public Action<Exception, Contexts> WorkflowException { get; set; }

        private readonly List<object> _middlewares;
        private readonly Assembly _botAssembly;
        private readonly IComponentContext _context;
        private readonly CommandParser _commandParser;

        public Workflow(Assembly botAssembly, IComponentContext context)
        {
            _middlewares = new List<object>();
            _botAssembly = botAssembly;
            this._context = context;

            this._commandParser = new CommandParser();//todo maybe autofac
        }

        public Workflow AddMiddleware<T>(object configuration = null /*TODO*/)
        {
            if (_middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return this;
            }

            var middleware = Activator.CreateInstance<T>();//todo autofac
            _middlewares.Add(middleware);
            return this;
        }

        public Task Run(SocketMessage data)
        {
            var contexts = this.RunMiddlewares(data);
            try
            {
                this.RunControllers(data.Content, contexts);
            }
            catch (Exception e)
            {
                WorkflowException.Invoke(e, contexts);
            }
            return Task.CompletedTask;
        }

        private Contexts RunMiddlewares<T>(T data)
        {
            var contexts = new Contexts();
            foreach (var middleware in _middlewares)
            {
                var context = ((dynamic)middleware).Process(data);
                contexts.SetContext(context);
            }
            return contexts;
        }

        private void RunControllers(string message, Contexts contexts)
        {
            var discordRequest = _commandParser.Parse(message);
            var controllersWithMethods = GetControllersWithMethods();

            var methodsWithReadAlways = controllersWithMethods.Where(x => x.method.HasAttribute<ReadAlways>());
            this.RunWithReadAlwaysMethods(discordRequest, contexts, methodsWithReadAlways);

            if (!discordRequest.IsCommandForBot)
                return;

            var withDiscordCommand = controllersWithMethods.Where(x => x.method.HasAttribute<DiscordCommand>());
            this.RunWithDiscordCommandMethods(discordRequest, contexts, withDiscordCommand);
        }

        private List<(IController controller, MethodInfo method)> GetControllersWithMethods()
        {
            var controllers = _botAssembly.GetTypesByInterface<IController>()
                .Select(x => (IController) _context.Resolve(x));

            var controllersWithMethods = new List<(IController, MethodInfo)>();
            foreach (var controller in controllers)
            {
                var methods = controller.GetType().GetMethods();

                methods.ToList().ForEach(method =>
                {
                    controllersWithMethods.Add((controller, method));
                });
            }
            return controllersWithMethods;
        }

        private Task RunWithReadAlwaysMethods(DiscordRequest request, Contexts contexts, IEnumerable<(IController, MethodInfo)> controllersWithMethods)
        {
            foreach (var (controller, method) in controllersWithMethods)
            {
                var arguments = new object[] { request, contexts };
                method.Invoke(controller, arguments);
            }
            return Task.CompletedTask;
        }

        private Task RunWithDiscordCommandMethods(DiscordRequest request, Contexts contexts, IEnumerable<(IController, MethodInfo)> controllersWithMethods)
        {
            foreach (var (controller, method) in controllersWithMethods)
            {
                var commandArguments = method.GetCustomAttributesData()
                    .Where(x => x.AttributeType.FullName == typeof(DiscordCommand).FullName)
                    .SelectMany(x => x.ConstructorArguments, (x, arg) => arg.Value).ToArray();

                var commands = commandArguments.Select(x => (DiscordCommand)Activator.CreateInstance(typeof(DiscordCommand), x));

                //todo fix. this version is for usersController but should be changed
                if (commands.Any(x => request.Name == x.Command || request.OriginalMessage.TrimStart(request.Prefix.ToCharArray()).StartsWith(x.Command))) 
                {
                    if (method.HasAttribute<AdminCommand>() && !contexts.User.IsAdmin)
                    {
                        throw new NotAdminPermissionsException();
                    }

                    method.Invoke(controller, new object[] { request, contexts });
                    break;
                }
            }
            return Task.CompletedTask;
        }
    }
}
