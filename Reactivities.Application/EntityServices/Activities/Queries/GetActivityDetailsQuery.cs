using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Queries
{
    public class GetActivityDetailsQuery : IRequest<Activity>
    {
        public Guid Id { get; set; }
    }

    public class GetActivityDetailsQueryHandler : IRequestHandler<GetActivityDetailsQuery, Activity>
    {
        private readonly ReactivitiesDbContext _context;

        public GetActivityDetailsQueryHandler(ReactivitiesDbContext context)
        {
            _context = context;
        }

        public async Task<Activity> Handle(GetActivityDetailsQuery request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            return activity;
        }
    }
}
