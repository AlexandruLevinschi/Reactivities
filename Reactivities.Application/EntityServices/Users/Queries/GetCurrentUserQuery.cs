using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;

namespace Reactivities.Application.EntityServices.Users.Queries
{
    public class GetCurrentUserQuery : IRequest<UserModel>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserModel>
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUserAccessor _userAccessor;

        public GetCurrentUserQueryHandler(UserManager<User> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
        {
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
            _userAccessor = userAccessor;
        }

        public async Task<UserModel> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            return new UserModel
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Token = _jwtGenerator.CreateToken(user),
                Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                IsAdmin = user.IsAdmin
            };
        }
    }
}
