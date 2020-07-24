﻿using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam
{
    public interface IOverallSpamDetector
    {
        public SpamProbability GetOverallSpamProbability(DiscordRequest request, Contexts contexts);
    }
}
