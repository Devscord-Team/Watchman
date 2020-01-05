using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Common.Strings
{
    public static class StringExtensions
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

        public static StringBuilder FormatOneMessage(this StringBuilder builder, string message, bool contentStyleBox = true, string formatName = "")
        {
            if (contentStyleBox)
            {
                builder.AppendLine($"```{formatName}");
            }

            builder.AppendLine(message);

            if (contentStyleBox)
            {
                builder.AppendLine("```");
            }

            return builder;
        }
    }
}
