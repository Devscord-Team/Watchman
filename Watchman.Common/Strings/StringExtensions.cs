using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Common.Strings
{
    public static class StringExtensions
    {
        public static StringBuilder PrintManyLines(this StringBuilder builder, string header, string[] lines, bool contentStyleBox = true)
        {
            builder.AppendLine(header);
            PrintManyLines(builder, lines, contentStyleBox);
            return builder;
        }

        public static StringBuilder PrintManyLines(this StringBuilder builder, string[] lines, bool contentStyleBox = true)
        {
            if(contentStyleBox)
            {
                builder.AppendLine("```");
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
    }
}
