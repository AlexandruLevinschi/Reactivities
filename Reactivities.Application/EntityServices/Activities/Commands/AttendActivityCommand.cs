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

namespace Reactivities.Application.EntityServices.Activities.Commands
{
    public class AttendActivityCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class AttendActivityCommandHandler : IRequestHandler<AttendActivityCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public AttendActivityCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Unit> Handle(AttendActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            if (activity == null) throw new RestException(HttpStatusCode.NotFound, new { activity = "Could not find activity." });

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                cancellationToken);

            var attendance =
                await _context.UserActivities.SingleOrDefaultAsync(
                    x => x.ActivityId == activity.Id && x.UserId == user.Id, cancellationToken);

            if (attendance != null)
            {
                throw new RestException(HttpStatusCode.BadRequest,
                    new {Attendance = "Already attending this activity."});
            }

            attendance = new UserActivity
            {
                Activity = activity,
                User = user,
                IsHost = false,
                DateJoined = DateTime.Now
            };

            _context.UserActivities.Add(attendance);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Problem saving changes");
        }
    }

    public class AttendActivityCommandValidator : AbstractValidator<AttendActivityCommand>
    {
        public AttendActivityCommandValidator()
        {
            RuleFor(request => request.Id).NotEmpty();
        }
    }
}
