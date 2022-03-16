using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.Models
{
    internal class FakeRole : IRole
    {
        public IGuild Guild { get; set; }

        public Color Color { get; set; }

        public bool IsHoisted { get; set; }

        public bool IsManaged { get; set; }

        public bool IsMentionable { get; set; }

        public string Name { get; set; }

        public GuildPermissions Permissions { get; set; }

        public int Position { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ulong Id { get; set; }

        public string Mention { get; set; }

        public string Icon => throw new NotImplementedException();

        public Emoji Emoji => throw new NotImplementedException();

        public RoleTags Tags => throw new NotImplementedException();

        public int CompareTo(IRole other)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string GetIconUrl()
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
