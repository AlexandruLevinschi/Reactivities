using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Comments.Commands
{
    public class CreateCommentCommand : IRequest<CommentDto>
    {
        public string Body { get; set; }

        public Guid ActivityId { get; set; }

        public string Username { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IMapper _mapper;

        public CreateCommentCommandHandler(ReactivitiesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities.FindAsync(request.ActivityId);

            if (activity == null)
                throw new RestException(HttpStatusCode.NotFound, new {Activity = "Could not find activity."});

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.Username,
                cancellationToken);

            var comment = new Comment
            {
                Author = user,
                Activity = activity,
                Body = request.Body,
                CreatedAt = DateTime.Now
            };

            activity.Comments.Add(comment);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return _mapper.Map<Comment, CommentDto>(comment);

            throw new Exception("Problem saving changes.");
        }
    }

    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(request => request.ActivityId).NotEmpty();
            
            RuleFor(request => request.Username).NotEmpty();
            
            RuleFor(request => request.Body).NotEmpty();
        }
    }
}
