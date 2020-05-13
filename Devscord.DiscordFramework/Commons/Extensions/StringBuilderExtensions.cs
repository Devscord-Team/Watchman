using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder PrintManyLines(this StringBuilder builder, string header, string[] lines, bool contentStyleBox = true, bool spacesBetweenLines = false, string formatName = "")
        {
            builder.AppendLine(header);
            PrintManyLines(builder, lines, contentStyleBox, spacesBetweenLines, formatName);
            return builder;
        }

        public static StringBuilder PrintManyLines(this StringBuilder builder, string[] lines, bool contentStyleBox = true, bool spacesBetweenLines = false, string formatName = "")
        {
            builder.AppendLine();//can fix message if response looks like "test {outputFromPrintManyLines}"
            if (contentStyleBox)
            {
                builder.AppendLine($"```{formatName}");
            }
            foreach (var line in lines)
            {
                builder.AppendLine(line);
                if (spacesBetweenLines && line.GetHashCode() != lines.Last().GetHashCode())
                {
                    builder.AppendLine();
                }
            }
            if (contentStyleBox)
            {
                builder.AppendLine("```");
            }
            return builder;
        }

        public static StringBuilder PrintManyLines(this StringBuilder builder, Dictionary<string, string> lines, bool contentStyleBox = true, bool spacesBetweenLines = false)
        {
            builder.AppendLine();
            if (contentStyleBox)
            {
                builder.AppendLine("```");
            }
            foreach (var line in lines)
            {
                builder.Append(line.Key).Append(" => ").AppendLine(line.Value);
                if (spacesBetweenLines && line.GetHashCode() != lines.Last().GetHashCode())
                {
                    builder.AppendLine();
                }
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
