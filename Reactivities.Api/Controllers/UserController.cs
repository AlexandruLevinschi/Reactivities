using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.EntityServices.Users;
using Reactivities.Application.EntityServices.Users.Commands;
using Reactivities.Application.EntityServices.Users.Queries;

namespace Reactivities.Api.Controllers
{
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> Login(LoginQuery query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register(RegisterUserCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<UserModel>> CurrentUser()
        {
            return await Mediator.Send(new GetCurrentUserQuery());
        }
    }
}