using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devscord.DiscordFramework
{
    public class ControllerInfo
    {
        public IController Controller { get; private set; }
        public IEnumerable<MethodInfo> Methods { get; private set; }

        public ControllerInfo(IController controller, IEnumerable<MethodInfo> methods = null)
        {
            this.Controller = controller;
            this.Methods = methods ?? this.GetMethods(controller);
        }

        private IEnumerable<MethodInfo> GetMethods(IController controller)
        {
            return controller.GetType().GetMethods()
                .Where(x => x.GetParameters().Length == 2)
                .Where(x => typeof(IBotCommand).IsAssignableFrom(x.GetParameters()[0].ParameterType))
                .Where(x => typeof(Contexts).IsAssignableFrom(x.GetParameters()[1].ParameterType))
                .ToList();
        }
    }
}
