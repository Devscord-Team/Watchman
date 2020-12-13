using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public interface IBotCommandsService
    {
        bool IsDefaultCommand(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments, bool isCommandMatchedWithCustom);
        bool AreDefaultCommandArgumentsCorrect(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments);
        bool AreCustomCommandArgumentsCorrect(BotCommandTemplate template, Regex customTemplate, string input);
        BotCommandTemplate GetCommandTemplate(Type commandType);
        IBotCommand ParseRequestToCommand(Type commandType, DiscordRequest request, BotCommandTemplate template);
        IBotCommand ParseCustomTemplate(Type commandType, BotCommandTemplate template, Regex customTemplate, string input);
    }
}
