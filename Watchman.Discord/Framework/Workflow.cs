using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord.Framework.Architecture.Controllers;

namespace Watchman.Discord.Framework
{
    public class Workflow
    {
        private List<object> _middlewares;
        private List<object> _controllers;

        public Workflow()
        {
            this._middlewares = new List<object>();
            this._controllers = new List<object>();
        }

        public Workflow AddMiddleware<T>(object configuration = null /*TODO*/)
        {
            if(this._middlewares.Any(x => x.GetType().FullName == typeof(T).FullName))
            {
                return this;
            }
            var middleware = Activator.CreateInstance<T>();
            this._middlewares.Add(middleware);
            return this;
        }

        public Workflow AddControllers(object configuration = null /*TODO*/)
        {
            var assembly = typeof(Workflow).Assembly;
            var controllers = assembly.GetTypes()
                .Where(x => x.GetInterface("IController") != null)
                .Select(x => Activator.CreateInstance(x));
            this._controllers.AddRange(controllers);
            return this;
        }

        public async Task Run<T>(T data)
        {
            if(data is SocketMessage message) //todo make controllers generics, and change SocketMessage to DTO object
            {
                await this.RunMiddlewares(message);
                await this.RunControllers(message);
            }
        }

        private async Task RunMiddlewares<T>(T data)
        {
            await Task.WhenAll(this._middlewares
               .Where(x => x.GetType()
                   .GetInterfaces().First()
                   .GenericTypeArguments
                   .First().FullName == typeof(T).FullName)
               .ToList()
               .Select(x => (Task)((dynamic)x).Process(data)));
        }

        private Task RunControllers(SocketMessage message)
        {
            //TODO create DTO objects -> we want to be independent from discord.net library
            //TODO run controllers concurrently
            foreach (var controller in _controllers)
            {
                var methods = controller.GetType().GetMethods();
                var withReadAll = methods.Where(x => x.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(ReadAlways).FullName));
                var withDiscordCommand = methods.Where(x => x.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(DiscordCommand).FullName));

                foreach (var method in withReadAll)
                {
                    //TODO copy to foreach with DiscordCommand
                    if (method.GetParameters().Any(p => p.ParameterType.FullName == typeof(SocketMessage).FullName))
                    {
                        method.Invoke(controller, new object[] { message });
                    }
                    else
                    {
                        method.Invoke(controller, new object[] { });
                    }
                }

                foreach (var method in withDiscordCommand)
                {
                    var commandArguments = method.GetCustomAttributesData()
                        .First(x => x.AttributeType.FullName == typeof(DiscordCommand).FullName)
                        .ConstructorArguments.Select(x => x.Value).ToArray();

                    var command = (DiscordCommand)Activator.CreateInstance(typeof(DiscordCommand), commandArguments);

                    if (message.Content.ToLowerInvariant().StartsWith(command.Command))
                    {
                        bool isMethodAdminOnly = method.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(AdminCommand).FullName);

                        if (isMethodAdminOnly && !HasAdminPermissions(message.Author))
                        {
                            break;
                        }

                        method.Invoke(controller, new object[] { message });
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }
        private bool HasAdminPermissions(SocketUser user)
        {
            var author = (SocketGuildUser)user;
            return author.Roles.Any(r => r.Name.ToLowerInvariant() == "admin");
        }
    }
}
