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

namespace Reactivities.Application.EntityServices.Activities.Commands
{
    public class UnattendActivityCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class UnatendActivityCommandHandler : IRequestHandler<UnattendActivityCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public UnatendActivityCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Unit> Handle(UnattendActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            if (activity == null) throw new RestException(HttpStatusCode.NotFound, new { activity = "Could not find activity." });

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                cancellationToken);

            var attendance =
                await _context.UserActivities.SingleOrDefaultAsync(
                    x => x.ActivityId == activity.Id && x.UserId == user.Id, cancellationToken);

            if (attendance == null) return Unit.Value;

            if (attendance.IsHost) throw new RestException(HttpStatusCode.BadRequest, new { attendance = "You cannot remove yourself as host." });

            _context.UserActivities.Remove(attendance);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Problem saving changes.");
        }
    }

    public class UnattendActivityCommandValidator : AbstractValidator<UnattendActivityCommand>
    {
        public UnattendActivityCommandValidator()
        {
            RuleFor(request => request.Id).NotEmpty();
        }
    }
}
