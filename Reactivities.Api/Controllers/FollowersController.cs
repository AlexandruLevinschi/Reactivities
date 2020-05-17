using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.EntityServices.Followers.Commands;
using Reactivities.Application.EntityServices.Followers.Queries;
using Reactivities.Application.EntityServices.Profiles;

namespace Reactivities.Api.Controllers
{
    [Route("api/profiles")]
    public class  FollowersController : BaseController
    {
        [HttpPost("follow/{username}")]
        public async Task<ActionResult<Unit>> Follow(string username)
        {
            return await Mediator.Send(new CreateFollowerCommand {Username = username});
        }

        [HttpDelete("unfollow/{username}")]
        public async Task<ActionResult<Unit>> Unfollow(string username)
        {
            return await Mediator.Send(new DeleteFollowerCommand {Username = username});
        }

        [HttpGet("getFollowings")]
        public async Task<ActionResult<List<Profile>>> GetFollowings(string username, string predicate)
        {
            return await Mediator.Send(new GetFollowersQuery {Username = username, Predicate = predicate});
        }
    }
}