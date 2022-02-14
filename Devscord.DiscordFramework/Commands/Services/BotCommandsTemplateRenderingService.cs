using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.Properties;
using System;
using System.Text;

namespace Devscord.DiscordFramework.Commands.Services
{
    public interface IBotCommandsTemplateRenderingService
    {
        string RenderTextTemplate(BotCommandTemplate template);
    }

    public class BotCommandsTemplateRenderingService : IBotCommandsTemplateRenderingService
    {
        public string RenderTextTemplate(BotCommandTemplate template)
        {
            var output = new StringBuilder();
            output.Append($"{{{{prefix}}}}[[{template.CommandName}]]");
            foreach (var commandProperty in template.Properties)
            {
                output.Append($" {{{{prefix}}}}[[{commandProperty.Name}]] (({Enum.GetName(typeof(BotCommandPropertyType), commandProperty.Type)}))");
                if (commandProperty.IsOptional)
                {
                    output.Append("<<optional>>");
                }
            }
            return output.ToString();
        }
    }
}
