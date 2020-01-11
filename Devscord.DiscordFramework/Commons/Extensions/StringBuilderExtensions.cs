using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder PrintManyLines(this StringBuilder builder, string header, string[] lines, bool contentStyleBox = true, string formatName = "")
        {
            builder.AppendLine(header);
            PrintManyLines(builder, lines, contentStyleBox, formatName);
            return builder;
        }

        public static StringBuilder PrintManyLines(this StringBuilder builder, string[] lines, bool contentStyleBox = true, string formatName = "")
        {
            if(contentStyleBox)
            {
                builder.AppendLine($"```{formatName}");
            }
            foreach (var line in lines)
            {
                builder.AppendLine(line);
            }
            if (contentStyleBox)
            {
                builder.AppendLine("```");
            }
            return builder;
        }

        public static StringBuilder PrintManyLines(this StringBuilder builder, Dictionary<string, string> lines, bool contentStyleBox = true)
        {
            if (contentStyleBox)
            {
                builder.AppendLine("```");
            }
            foreach (var line in lines)
            {
                builder.Append(line.Key).Append(" => ").AppendLine(line.Value);
            }
            if (contentStyleBox)
            {
                builder.AppendLine("```");
            }
            return builder;
        }

        public static StringBuilder FormatMessageIntoBlock(this StringBuilder builder, string formatName = "")
        {
            builder.Insert(0, $"```{formatName}\n");
            builder.AppendLine("```");
            return builder;
        }
    }
}
