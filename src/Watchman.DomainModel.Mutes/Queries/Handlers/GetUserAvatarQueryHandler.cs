using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Mutes.Queries.Handlers
{
    public class GetUserAvatarQueryHandler : IQueryHandler<GetUserAvatarQuery, GetUserAvatarQueryResult>
    {
        public GetUserAvatarQueryResult Handle(GetUserAvatarQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
