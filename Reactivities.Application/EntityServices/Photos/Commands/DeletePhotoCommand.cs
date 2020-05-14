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
    public class DeletePhotoCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;

        public DeletePhotoCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
        }

        public async Task<Unit> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                cancellationToken);

            var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);
            if (photo == null) throw new RestException(HttpStatusCode.NotFound, new { photo = "Could not find photo." });
            if (photo.IsMain) throw new RestException(HttpStatusCode.BadRequest, new { photo = "You cannot delete your main photo." });

            var result = _photoAccessor.DeletePhoto(photo.Id);
            if (result == null) throw new Exception("Problem deleting the photo.");

            user.Photos.Remove(photo);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success) return Unit.Value;
            
            throw new Exception("Problem saving changes.");
        }
    }
}
