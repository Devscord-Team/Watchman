using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.CustomCommands.Commands
{
    public class AddCustomCommandsCommand : ICommand
    {
        public string CommandFullName { get; private set; }
        public string CustomTemplateRegex { get; private set; }
        public ulong ServerId { get; private set; }

        public AddCustomCommandsCommand(string commandFullName, string customTemplateRegex, ulong serverId)
        {
            CommandFullName = commandFullName;
            CustomTemplateRegex = customTemplateRegex;
            ServerId = serverId;
        }
    }
}
