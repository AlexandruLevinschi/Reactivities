using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities
{
    public class GetActivitiesQuery : IRequest<List<Activity>>
    {
    }

    public class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, List<Activity>>
    {
        private readonly ReactivitiesDbContext _context;

        public GetActivitiesQueryHandler(ReactivitiesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Activity>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
        {
            var activities = await _context.Activities.ToListAsync(cancellationToken);

            return activities;
        }
    }
}
