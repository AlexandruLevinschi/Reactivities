using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reactivities.Application.Interfaces;
using Reactivities.Domain.Entities;
using Reactivities.Persistence;

namespace Reactivities.Application.EntityServices.Photos.Commands
{
    public class AddPhotoCommand : IRequest<Photo>
    {
        public IFormFile File { get; set; }
    }

    public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, Photo>
    {
        private readonly ReactivitiesDbContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IPhotoAccessor _photoAccessor;

        public AddPhotoCommandHandler(ReactivitiesDbContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
            _photoAccessor = photoAccessor;
        }

        public async Task<Photo> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            var photoUploadResult = _photoAccessor.AddPhoto(request.File);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(), cancellationToken);

            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                Id = photoUploadResult.PublicId
            };

            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

            user.Photos.Add(photo);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success) return photo;

            throw new Exception("Problem saving changes.");
        }
    }
}
