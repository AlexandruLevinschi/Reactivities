using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Commands
{
    public class CreateActivityCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }
    }

    public class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public CreateActivityCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Unit> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = new Activity
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                City = request.City,
                Date = request.Date,
                Venue = request.Venue
            };

            _context.Activities.Add(activity);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(), cancellationToken);
            var attendee = new UserActivity
            {
                User = user,
                Activity = activity,
                IsHost = true,
                DateJoined = DateTime.Now
            };

            _context.UserActivities.Add(attendee);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Error during save changes.");
        }
    }

    public class CreateActivityCommandValidator : AbstractValidator<CreateActivityCommand>
    {
        public CreateActivityCommandValidator()
        {

            RuleFor(a => a.Name).NotEmpty();

            RuleFor(a => a.Description).NotEmpty();

            RuleFor(a => a.Category).NotEmpty();

            RuleFor(a => a.Date).NotEmpty();

            RuleFor(a => a.City).NotEmpty();

            RuleFor(a => a.Venue).NotEmpty();
        }
    }
}
