using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Reactivities.Application.Exceptions;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Commands
{
    public class UpdateActivityCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public DateTime? Date { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }
    }

    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand>
    {
        private readonly ReactivitiesDbContext _context;

        public UpdateActivityCommandHandler(ReactivitiesDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            if (activity == null) throw new RestException(HttpStatusCode.NotFound, "Could not find activity.");

            activity.Name = request.Name ?? activity.Name;
            activity.Description = request.Description ?? activity.Description;
            activity.Category = request.Category ?? activity.Category;
            activity.Date = request.Date ?? activity.Date;
            activity.City = request.City ?? activity.City;
            activity.Venue = request.Venue ?? activity.Venue;

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Error during save changes.");
        }
    }

    public class UpdateActivityCommandValidator : AbstractValidator<UpdateActivityCommand>
    {
        public UpdateActivityCommandValidator()
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
