using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

        public CreateActivityCommandHandler(ReactivitiesDbContext context)
        {
            _context = context;
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

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Error during save changes.");
        }
    }
}
