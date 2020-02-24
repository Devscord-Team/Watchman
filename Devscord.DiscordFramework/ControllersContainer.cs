using Autofac;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework
{
    internal class ControllersContainer
    {
        private IEnumerable<ControllerInfo> _controllers;

        public ControllersContainer(IEnumerable<ControllerInfo> controllers)
        {
            this._controllers = controllers;
        }

        public IEnumerable<ControllerInfo> WithReadAlways
            => this._controllers.Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<ReadAlways>())));

        public IEnumerable<ControllerInfo> WithDiscordCommand
            => this._controllers.Select(x => new ControllerInfo(x.Controller, x.Methods.Where(m => m.HasAttribute<DiscordCommand>())));
    }
}
