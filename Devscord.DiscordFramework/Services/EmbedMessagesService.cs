using System;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Services
{
    public class EmbedMessagesService
    {
        public Embed Generate(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var builder = this.GetDefault();
            builder.Title = title;
            builder.Description = description;
            foreach (var value in values)
            {
                builder.AddField(value.Key, value.Value);
            }
            return builder.Build();
        }

        public Embed Generate(string title, string description, Dictionary<string, Dictionary<string, string>> values)
        {
            var flatValues = new Dictionary<string, string>();
            foreach (var value in values)
            {
                var valuesString = value.Value.Aggregate(string.Empty,(a, b) => $"{a}\n{b.Key} {b.Value}");
                flatValues.Add(value.Key, valuesString);
            }
            return this.Generate(title, description, flatValues);
        }

        private EmbedBuilder GetDefault()
        {
            return new EmbedBuilder()
                .WithThumbnailUrl(@"https://raw.githubusercontent.com/Devscord-Team/Watchman/master/avatar.png")
                .WithFooter(new EmbedFooterBuilder()
                .WithText(@"Wygenerowane przez https://github.com/Devscord-Team/Watchman")
                .WithIconUrl(@"https://raw.githubusercontent.com/Devscord-Team/Watchman/master/avatar.png"));
        }
    }
}
