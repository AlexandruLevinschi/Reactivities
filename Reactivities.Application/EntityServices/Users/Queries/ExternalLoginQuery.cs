using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;

namespace Reactivities.Application.EntityServices.Users.Queries
{
    public class ExternalLoginQuery : IRequest<UserModel>
    {
        public string AccessToken { get; set; }
    }

    public class ExternalLoginQueryHandler : IRequestHandler<ExternalLoginQuery, UserModel>
    {
        private readonly UserManager<User> _userManager;
        private readonly IFacebookAccessor _facebookAccessor;
        private readonly IJwtGenerator _jwtGenerator;

        public ExternalLoginQueryHandler(UserManager<User> userManager, IFacebookAccessor facebookAccessor, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _facebookAccessor = facebookAccessor;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<UserModel> Handle(ExternalLoginQuery request, CancellationToken cancellationToken)
        {
            var userInfo = await _facebookAccessor.FacebookLogin(request.AccessToken);
            if (userInfo == null) throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem validating token." });

            var user = await _userManager.FindByEmailAsync(userInfo.Email);
            if (user == null)
            {
                user = new User
                {
                    DisplayName = userInfo.Name,
                    Id = userInfo.Id,
                    Email = userInfo.Email,
                    UserName = "fb_" + userInfo.Id,
                };

                var photo = new Photo
                {
                    Id = "fb_" + userInfo.Id,
                    Url = userInfo.Picture.Data.Url,
                    IsMain = true
                };

                user.Photos.Add(photo);

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded) throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user." });
            }

            return new UserModel
            {
                DisplayName = user.DisplayName,
                Token = _jwtGenerator.CreateToken(user),
                Username = user.UserName,
                Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
            };
        }
    }
}
