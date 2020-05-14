using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Profiles.Queries
{
    public class GetUserProfileQuery : IRequest<Profile>
    {
        public string Username { get; set; }
    }

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Profile>
    {
        private readonly ReactivitiesDbContext _context;

        public GetUserProfileQueryHandler(ReactivitiesDbContext context)
        {
            _context = context;
        }

        public async Task<Profile> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username,
                cancellationToken);

            return new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                Photos = user.Photos,
                Biography = user.Biography
            };
        }
    }
}
