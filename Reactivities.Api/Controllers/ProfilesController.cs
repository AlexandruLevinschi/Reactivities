using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Profiles;
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

        [HttpGet("{username}/activities")]
        public async Task<ActionResult<List<UserActivityDto>>> GetUserActivities(string username, string predicate)
        {
            return await Mediator.Send(new ListActivities.Query {Username = username, Predicate = predicate});
        }
    }
}