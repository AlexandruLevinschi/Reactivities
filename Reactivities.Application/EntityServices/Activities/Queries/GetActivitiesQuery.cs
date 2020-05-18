using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Queries
{
    public class ActivitiesEnvelope
    {
        public List<ActivityDto> Activities { get; set; }

        public int ActivityCount { get; set; }
    }

    public class GetActivitiesQuery : IRequest<ActivitiesEnvelope>
    {
        public int? Offset { get; set; }

        public int? Limit { get; set; }

        public bool IsGoing { get; set; }

        public bool IsHost { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
    }

    public class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, ActivitiesEnvelope>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;

        public GetActivitiesQueryHandler(ReactivitiesDbContext context, IMapper mapper, IUserAccessor userAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userAccessor = userAccessor;
        }

        public async Task<ActivitiesEnvelope> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.Activities
                .Where(a => a.Date >= request.StartDate)
                .OrderBy(a => a.Date)
                .AsQueryable();

            if (request.IsGoing && !request.IsHost)
            {
                queryable = queryable.Where(q =>
                    q.UserActivities.Any(ua => ua.User.UserName == _userAccessor.GetCurrentUsername()));
            }

            if (request.IsHost && !request.IsGoing)
            {
                queryable = queryable.Where(q =>
                    q.UserActivities.Any(ua => ua.User.UserName == _userAccessor.GetCurrentUsername() && ua.IsHost));
            }

            var activities = await queryable
                .Skip(request.Offset ?? 0)
                .Take(request.Limit ?? 3)
                .ToListAsync(cancellationToken);

            return new ActivitiesEnvelope
            {
                Activities = _mapper.Map<List<Activity>, List<ActivityDto>>(activities),
                ActivityCount = queryable.Count()
            };
        }
    }
}
