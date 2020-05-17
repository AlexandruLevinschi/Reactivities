using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly IUserAccessor _userAccessor;
        private readonly ReactivitiesDbContext _context;

        public ProfileReader(IUserAccessor userAccessor, ReactivitiesDbContext context)
        {
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<Profile> ReadProfile(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Could not find user." });

            var currentUser =
                await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername());

            var profile = new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                Photos = user.Photos,
                Biography = user.Biography,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Followings.Count
            };

            if (currentUser.Followings.Any(f => f.TargetId == user.Id)) profile.IsFollowed = true;

            return profile;
        }
    }
}
