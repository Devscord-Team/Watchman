using Devscord.DiscordFramework.Framework.Commands.Properties;
using System;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsTemplateRenderingService
    {
        public string RenderTextTemplate(BotCommandTemplate template)
        {
            var output = new StringBuilder();
            output.Append($"{{{{prefix}}}}[[{template.CommandName}]]");
            foreach (var commandProperty in template.Properties)
            {
                output.Append($" {{{{prefix}}}}[[{commandProperty.Name}]] (({Enum.GetName(typeof(BotCommandPropertyType), commandProperty.GeneralType)}))");
                if (commandProperty.IsOptional)
                {
                    output.Append("<<optional>>");
                }
            }
            return output.ToString();
        }
    }
}
