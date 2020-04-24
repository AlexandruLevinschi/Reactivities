using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Commands
{
    public class DeleteActivityCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand>
    {
        private readonly ReactivitiesDbContext _context;

        public DeleteActivityCommandHandler(ReactivitiesDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            if (activity == null) throw new Exception("Could not find activity");

            _context.Remove(activity);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return Unit.Value;

            throw new Exception("Error during save changes.");
        }
    }
}
