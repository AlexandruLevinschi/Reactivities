using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

    public class UpdateActivityCommandRequestHandler : IRequestHandler<UpdateActivityCommand>
    {
        private readonly ReactivitiesDbContext _context;

        public UpdateActivityCommandRequestHandler(ReactivitiesDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            if (activity == null) throw new Exception("Could not find activity.");

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
}
