using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Activities.Queries
{
    public class GetActivityDetailsQuery : IRequest<ActivityDto>
    {
        public Guid Id { get; set; }
    }

    public class GetActivityDetailsQueryHandler : IRequestHandler<GetActivityDetailsQuery, ActivityDto>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IMapper _mapper;

        public GetActivityDetailsQueryHandler(ReactivitiesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActivityDto> Handle(GetActivityDetailsQuery request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.Id);

            if (activity == null) throw new RestException(HttpStatusCode.NotFound, "Could not find activity.");

            var result = _mapper.Map<Activity, ActivityDto>(activity);

            return result;
        }
    }
}
