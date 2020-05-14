using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.EntityServices.Photos.Commands;
using Reactivities.Domain.Entities;

namespace Reactivities.Api.Controllers
{
    public class PhotosController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Photo>> Add([FromForm] AddPhotoCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(string id)
        {
            return await Mediator.Send(new DeletePhotoCommand {Id = id});
        }

        [HttpPost("setMain/{id}")]
        public async Task<ActionResult<Unit>> SetMain(string id)
        {
            return await Mediator.Send(new SetMainPhotoCommand {Id = id});
        }
    }
}