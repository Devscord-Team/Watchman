using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Queries.Handlers
{
    public class GetUserAvatarQueryHandler : IQueryHandler<GetUserAvatarQuery, GetUserAvatarQueryResult>
    {
        public GetUserAvatarQueryResult Handle(GetUserAvatarQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
