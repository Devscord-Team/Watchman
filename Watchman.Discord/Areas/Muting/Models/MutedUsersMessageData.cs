﻿using System.Collections.Generic;

namespace Watchman.Discord.Areas.Muting.Models
{
    public class MutedUsersMessageData
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, Dictionary<string, string>> Values { get; private set; }

        public MutedUsersMessageData(string title, string description, Dictionary<string, Dictionary<string, string>> values)
        {
            this.Title = title;
            this.Description = description;
            this.Values = values;
        }
    }
}
