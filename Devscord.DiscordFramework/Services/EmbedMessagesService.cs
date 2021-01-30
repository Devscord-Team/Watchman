using Discord;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Services
{
    public class EmbedMessagesService
    {
        internal int FooterLength => FOOTER_TEXT.Length;
        private const string FOOTER_TEXT = @"Wygenerowane przez https://github.com/Devscord-Team/Watchman";

        internal Embed Generate(string title, string description, IEnumerable<KeyValuePair<string, string>> values, EmbedColor embedColor)
        {
            var builder = this.GetDefault(embedColor);
            builder.Title = title;
            builder.Description = description;
            foreach (var value in values)
            {
                builder.AddField(value.Key, value.Value);
            }
            return builder.Build();
        }

        internal Embed Generate(string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values, EmbedColor embedColor)
        {
            var flatValues = new Dictionary<string, string>();
            foreach (var (subtitle, lines) in values)
            {
                var valuesString = lines.Aggregate(string.Empty, (a, b) => $"{a}\n{b.Key} {b.Value}");
                flatValues.Add(subtitle, valuesString);
            }
            return this.Generate(title, description, flatValues, embedColor);
        }

        private EmbedBuilder GetDefault(EmbedColor embedColor)
        {
            var color = this.GetColor(embedColor);
            return new EmbedBuilder()
                .WithThumbnailUrl(@"https://raw.githubusercontent.com/Devscord-Team/Watchman/master/avatar.png")
                .WithColor(color) //maybe there should be many colors
                .WithFooter(new EmbedFooterBuilder()
                    .WithText(FOOTER_TEXT)
                    .WithIconUrl(@"https://raw.githubusercontent.com/Devscord-Team/Watchman/master/avatar.png"));
        }

        private Color GetColor(EmbedColor embedColor)
        {
            return embedColor switch
            {
                EmbedColor.Primary => new Color(23, 147, 237),
                EmbedColor.Success => new Color(73, 235, 52),
                EmbedColor.Warning => new Color(232, 235, 52),
                EmbedColor.Danger => new Color(235, 52, 52),
                _ => throw new NotImplementedException()
            };
        }
    }
    public enum EmbedColor
    {
        Primary, //Blue
        Success, //Green
        Warning, //Yellow
        Danger //Red
    }
}
