using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Exceptions;
using Reactivities.Application.Interfaces;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Photos.Commands
{
    public class SetMainPhotoCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class SetMainPhotoCommandHandler : IRequestHandler<SetMainPhotoCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public SetMainPhotoCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Unit> Handle(SetMainPhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                cancellationToken);

            var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);
            if (photo == null) throw new RestException(HttpStatusCode.NotFound, new { Photo = "Could not find photo." });

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success) return Unit.Value;

            throw new Exception("Problem saving changes.");
        }
    }
}
