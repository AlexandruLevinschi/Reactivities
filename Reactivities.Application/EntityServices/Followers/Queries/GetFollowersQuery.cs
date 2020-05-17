using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.EntityServices.Profiles;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Followers.Queries
{
    public class GetFollowersQuery : IRequest<List<Profile>>
    {
        public string Username { get; set; }

        public string Predicate { get; set; }
    }

    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, List<Profile>>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IProfileReader _profileReader;

        public GetFollowersQueryHandler(ReactivitiesDbContext context, IProfileReader profileReader)
        {
            _context = context;
            _profileReader = profileReader;
        }

        public async Task<List<Profile>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.Followings.AsQueryable();

            var userFollowings = new List<UserFollowing>();
            var profiles = new List<Profile>();

            switch (request.Predicate)
            {
                case "followers":
                    userFollowings = await queryable.Where(x => x.Target.UserName == request.Username)
                        .ToListAsync(cancellationToken);

                    foreach (var follower in userFollowings)
                    {
                        profiles.Add(await _profileReader.ReadProfile(follower.Observer.UserName));
                    }

                    break;
                case "followings":
                    userFollowings = await queryable.Where(x => x.Observer.UserName == request.Username)
                        .ToListAsync(cancellationToken);

                    foreach (var follower in userFollowings)
                    {
                        profiles.Add(await _profileReader.ReadProfile(follower.Target.UserName));
                    }

                    break;
            }

            return profiles;
        }
    }
}
