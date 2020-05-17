using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Followers.Commands
{
    public class CreateFollowerCommand : IRequest
    {
        public string Username { get; set; }
    }

    public class CreateFollowerCommandHandler : IRequestHandler<CreateFollowerCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public CreateFollowerCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Unit> Handle(CreateFollowerCommand request, CancellationToken cancellationToken)
        {
            var observer =
                await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                    cancellationToken);

            var target = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);

            if (target == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Could not found user." });

            var following =
                await _context.Followings.SingleOrDefaultAsync(f =>
                    f.ObserverId == observer.Id && f.TargetId == target.Id, cancellationToken);

            if (following != null)
                throw new RestException(HttpStatusCode.BadRequest, new {User = "You are already following this user."});
            
            following = new UserFollowing
            {
                Observer = observer,
                Target = target
            };

            _context.Followings.Add(following);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Problem saving changes.");
        }
    }

    public class CreateFollowerCommandValidator : AbstractValidator<CreateFollowerCommand>
    {
        public CreateFollowerCommandValidator()
        {
            RuleFor(request => request.Username).NotEmpty();
        }
    }
}
