using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.EntityServices.Activities.Commands;
using Reactivities.Application.EntityServices.Activities.Queries;

namespace Reactivities.Api.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var request = await Mediator.Send(new GetActivitiesQuery());

            if (request != null) return Ok(request);

            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var request = await Mediator.Send(new GetActivityDetailsQuery {Id = id});

            if (request != null) return Ok(request);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateActivityCommand command)
        {
            var request = await Mediator.Send(command);

            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateActivityCommand command)
        {
            command.Id = id;
            var request = await Mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = await Mediator.Send(new DeleteActivityCommand {Id = id});

            return Ok();
        }
    }
}