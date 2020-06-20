using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;

namespace Reactivities.Application.EntityServices.Users.Queries
{
    public class LoginQuery : IRequest<UserModel>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserModel>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;

        public LoginQueryHandler(UserManager<User> userManager, SignInManager<User> signInManager, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<UserModel> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) throw new RestException(HttpStatusCode.Unauthorized);

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                return new UserModel
                {
                    DisplayName = user.DisplayName,
                    Token = _jwtGenerator.CreateToken(user),
                    Username = user.UserName,
                    Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                    IsAdmin = user.IsAdmin
                };
            }

            throw new RestException(HttpStatusCode.Unauthorized);
        }
    }

    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.Password).NotEmpty();
        }
    }
}
