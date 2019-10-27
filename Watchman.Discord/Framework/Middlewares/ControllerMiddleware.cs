using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Framework.Architecture.Controllers;

namespace Watchman.Discord.Framework.Middlewares
{
    public class ControllerMiddleware : IMiddleware<SocketMessage>
    {

        private List<object> _controllers;

        public ControllerMiddleware()
        {
            this._controllers = new List<object>();
            this.Initialize().Wait();
        }

        public Task Initialize()
        {
            var assembly = typeof(Workflow).Assembly;
            var controllers = assembly.GetTypes()
                .Where(x => x.GetInterface("IController") != null)
                .Select(x => Activator.CreateInstance(x));
            this._controllers.AddRange(controllers);
            return Task.CompletedTask;
        }

        public Task Process(SocketMessage message)
        {
            //TODO create DTO objects -> we want to be independent from discord.net library
            foreach (var controller in _controllers)
            {
                var methods = controller.GetType().GetMethods();
                var withReadAll = methods.Where(x => x.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(ReadAlways).FullName));
                var withDiscordCommand = methods.Where(x => x.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(DiscordCommand).FullName));

                foreach (var method in withReadAll)
                {
                    //TODO copy to foreach with DiscordCommand
                    if(method.GetParameters().Any(p => p.ParameterType.FullName == typeof(SocketMessage).FullName))
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

                    var command = (DiscordCommand) Activator.CreateInstance(typeof(DiscordCommand), commandArguments);

                    if (message.Content.StartsWith(command.Command))
                    {
                        method.Invoke(controller, new object[] { message });
                        break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
