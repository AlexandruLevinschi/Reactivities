using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Followers.Commands
{
    public class DeleteFollowerCommand : IRequest
    {
        public string Username { get; set; }
    }

    public class DeleteFollowerCommandHandler : IRequestHandler<DeleteFollowerCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public DeleteFollowerCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Unit> Handle(DeleteFollowerCommand request, CancellationToken cancellationToken)
        {
            var observer =
                await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                    cancellationToken);

            var target = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);

            if (target == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Could not found user." });

            var following =
                await _context.Followings.SingleOrDefaultAsync(f =>
                    f.ObserverId == observer.Id && f.TargetId == target.Id, cancellationToken);

            if (following == null)
                throw new RestException(HttpStatusCode.BadRequest, new { User = "You are not following this user." });
            
            _context.Followings.Remove(following);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Problem saving changes.");
        }
    }

    public class DeleteFollowerCommandValidator : AbstractValidator<DeleteFollowerCommand>
    {
        public DeleteFollowerCommandValidator()
        {
            RuleFor(request => request.Username).NotEmpty();
        }
    }
}
