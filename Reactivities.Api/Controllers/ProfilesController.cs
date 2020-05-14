using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.EntityServices.Profiles;
using Reactivities.Application.EntityServices.Profiles.Queries;

namespace Reactivities.Api.Controllers
{
    public class ProfilesController : BaseController
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> Get(string username)
        {
            return await Mediator.Send(new GetUserProfileQuery {Username = username});
        }
    }
}