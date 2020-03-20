using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services.Models
{
    public class Message
    {
        public ulong Id { get; set; }
        public DiscordRequest Request { get; set; }
        public Contexts Contexts { get; set; }

        public Message(ulong id, DiscordRequest request, Contexts contexts)
        {
            Id = id;
            Request = request;
            Contexts = contexts;
        }
    }
}
