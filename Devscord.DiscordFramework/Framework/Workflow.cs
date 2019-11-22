using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Framework
{
    public class Workflow
    {
        private List<object> _middlewares;
        private List<object> _controllers;

        public Workflow()
        {
            _middlewares = new List<object>();
            _controllers = new List<object>();
        }

        public Workflow AddMiddleware<T>(object configuration = null /*TODO*/)
        {
            if (_middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return this;
            }
            var middleware = Activator.CreateInstance<T>();
            _middlewares.Add(middleware);
            return this;
        }

        public Workflow AddControllers(object configuration = null /*TODO*/)
        {
            var assembly = typeof(Workflow).Assembly;
            var controllers = assembly.GetTypes()
                .Where(x => x.GetInterface(nameof(IController)) != null)
                .Select(x => Activator.CreateInstance(x));
            _controllers.AddRange(controllers);
            return this;
        }

        public Task Run(SocketMessage data)
        {
            var contexts = this.RunMiddlewares(data);
            this.RunControllers(data.Content, contexts);
            return Task.CompletedTask;
        }

        private Dictionary<string, IDiscordContext> RunMiddlewares<T>(T data)
        {
            var contexts = new Dictionary<string, IDiscordContext>();
            foreach (var middleware in _middlewares)
            {
                var context = ((dynamic)middleware).Process(data);
                var contextName = ((object)context).GetType().Name;
                contexts.Add(contextName, context);
            }
            return contexts;
        }

        private void RunControllers(string message, Dictionary<string, IDiscordContext> contexts)
        {
            foreach (var controller in _controllers)
            {
                var methods = controller.GetType().GetMethods();
                var withReadAlways = methods.Where(x => x.HasAttribute<ReadAlways>());
                var withDiscordCommand = methods.Where(x => x.HasAttribute<DiscordCommand>());

                RunWithReadAlwaysMethods(controller, message, contexts, withReadAlways);
                RunWithDiscordCommandMethods(controller, message, contexts, withDiscordCommand);
            }
        }

        private void RunWithReadAlwaysMethods(object controller, string message, Dictionary<string, IDiscordContext> contexts, IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                //var arguments = method.HasParameter<DiscordCommand>() ? new object[] { message, contexts } : new object[] { };
                var arguments = new object[] { message, contexts };
                method.Invoke(controller, arguments);
            }
        }

        private void RunWithDiscordCommandMethods(object controller, string message, Dictionary<string, IDiscordContext> contexts, IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var commandArguments = method.GetCustomAttributesData()
                    .First(x => x.AttributeType.FullName == typeof(DiscordCommand).FullName)
                    .ConstructorArguments.Select(x => x.Value).ToArray();

                var command = (DiscordCommand)Activator.CreateInstance(typeof(DiscordCommand), commandArguments);
                if (message.StartsWith(command.Command))
                {
                    if (method.HasAttribute<AdminCommand>() && !((UserContext)contexts[nameof(UserContext)]).IsAdmin)
                    {
                        var channelContext = (ChannelContext)contexts[nameof(ChannelContext)];
                        var messageService = new MessagesService();
                        messageService.SendMessage("Nie masz wystarczających uprawnień do wywołania tej komendy.", channelContext.Id);
                        break;
                    }

                    method.Invoke(controller, new object[] { message, contexts });
                    break;
                }
            }
        }
    }
}
