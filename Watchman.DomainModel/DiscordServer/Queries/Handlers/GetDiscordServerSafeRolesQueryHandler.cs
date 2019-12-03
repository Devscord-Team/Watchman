using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries.Handlers
{
    public class GetDiscordServerSafeRolesQueryHandler : IQueryHandler<GetDiscordServerSafeRolesQuery, GetDiscordServerSafeRolesQueryResult>
    {
        public GetDiscordServerSafeRolesQueryResult Handle(GetDiscordServerSafeRolesQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
