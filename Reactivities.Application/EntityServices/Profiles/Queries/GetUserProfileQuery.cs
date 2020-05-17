using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Reactivities.Application.EntityServices.Profiles.Queries
{
    public class GetUserProfileQuery : IRequest<Profile>
    {
        public string Username { get; set; }
    }

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Profile>
    {
        private readonly IProfileReader _profileReader;

        public GetUserProfileQueryHandler(IProfileReader profileReader)
        {
            _profileReader = profileReader;
        }

        public async Task<Profile> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            return await _profileReader.ReadProfile(request.Username);
        }
    }
}
