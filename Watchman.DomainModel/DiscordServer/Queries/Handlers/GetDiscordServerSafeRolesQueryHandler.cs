using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries.Handlers
{
    public class GetDiscordServerSafeRolesQueryHandler : IQueryHandler<GetDiscordServerSafeRolesQuery, GetDiscordServerSafeRolesQueryResult>
    {
        //todo database
        private readonly List<Role> _safeRoles = new List<Role>
        {
            new Role("csharp"),
            new Role("java"),
            new Role("cpp"),
            new Role("tester"),
            new Role("javascript"),
            new Role("python"),
            new Role("php"),
            new Role("functional master"),
            new Role("rust"),
            new Role("go"),
            new Role("ruby"),
            new Role("newbie"),
        };

        public GetDiscordServerSafeRolesQueryResult Handle(GetDiscordServerSafeRolesQuery query)
        {
            return new GetDiscordServerSafeRolesQueryResult(this._safeRoles);
        }
    }
}
