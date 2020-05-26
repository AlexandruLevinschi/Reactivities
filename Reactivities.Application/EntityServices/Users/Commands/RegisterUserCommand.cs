using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Application.Validators;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Users.Commands
{
    public class RegisterUserCommand : IRequest<UserModel>
    {
        public string DisplayName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserModel>
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly ReactivitiesDbContext _context;

        public RegisterUserCommandHandler(UserManager<User> userManager, IJwtGenerator jwtGenerator, ReactivitiesDbContext context)
        {
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
            _context = context;
        }

        public async Task<UserModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Users.Where(u => u.Email == request.Email).AnyAsync(cancellationToken))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

            if (await _context.Users.Where(u => u.UserName == request.Username).AnyAsync(cancellationToken))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Username already exists" });

            var user = new User
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                UserName = request.Username
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded) return new UserModel
            {
                DisplayName = user.DisplayName,
                Token = _jwtGenerator.CreateToken(user),
                Username = user.UserName,
                Image = user.Photos?.FirstOrDefault(p => p.IsMain)?.Url
            };

            throw new Exception("Problem creating user");
        }
    }

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {

            RuleFor(request => request.DisplayName).NotEmpty();

            RuleFor(request => request.Username).NotEmpty();

            RuleFor(request => request.Email).NotEmpty().EmailAddress();

            RuleFor(request => request.Password).Password();
        }
    }
}
