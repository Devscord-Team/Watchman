﻿using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Services
{
    public interface IEmbedMessagesService
    {
        Embed Generate(string title, string description, IEnumerable<KeyValuePair<string, string>> values);
        Embed Generate(string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values);
    }

    public class EmbedMessagesService : IEmbedMessagesService
    {
        internal int FooterLength => FOOTER_TEXT.Length;
        private const string FOOTER_TEXT = @"Wygenerowane przez https://github.com/Devscord-Team/Watchman";

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

        public Embed Generate(string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values)
        {
            var flatValues = new Dictionary<string, string>();
            foreach (var (subtitle, lines) in values)
            {
                var valuesString = lines.Aggregate(string.Empty, (a, b) => $"{a}\n{b.Key} {b.Value}");
                flatValues.Add(subtitle, valuesString);
            }
            return this.Generate(title, description, flatValues);
        }

        private EmbedBuilder GetDefault()
        {
            return new EmbedBuilder()
                .WithThumbnailUrl(@"https://raw.githubusercontent.com/Devscord-Team/Watchman/master/avatar.png")
                .WithFooter(new EmbedFooterBuilder()
                    .WithText(FOOTER_TEXT)
                    .WithIconUrl(@"https://raw.githubusercontent.com/Devscord-Team/Watchman/master/avatar.png"));
        }
    }
}
