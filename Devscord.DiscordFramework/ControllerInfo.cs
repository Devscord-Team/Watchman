using Devscord.DiscordFramework.Architecture.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Devscord.DiscordFramework
{
    internal class ControllerInfo
    {
        public IController Controller { get; private set; }
        public MethodInfo[] Methods { get; private set; }

        public ControllerInfo(IController controller, MethodInfo[] methods = null)
        {
            this.Controller = controller;
            this.Methods = methods ?? this.GetMethods(controller);
        }

        private MethodInfo[] GetMethods(IController controller)
        {
            return controller.GetType().GetMethods().ToArray();
        }
    }
}
