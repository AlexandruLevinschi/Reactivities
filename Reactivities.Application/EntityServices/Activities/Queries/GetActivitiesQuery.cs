using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Queries
{
    public class GetActivitiesQuery : IRequest<List<ActivityDto>>
    {
    }

    public class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, List<ActivityDto>>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IMapper _mapper;

        public GetActivitiesQueryHandler(ReactivitiesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ActivityDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
        {
            var activities = await _context.Activities.ToListAsync(cancellationToken);

            return _mapper.Map<List<Activity>, List<ActivityDto>>(activities);
        }
    }
}
