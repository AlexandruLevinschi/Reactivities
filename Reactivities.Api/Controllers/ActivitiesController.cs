using System;
using System.Threading.Tasks;
using MediatR;
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
        [Authorize(Policy = "IsActivityHost")]
        public async Task<IActionResult> Update(Guid id, UpdateActivityCommand command)
        {
            command.Id = id;
            var request = await Mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = await Mediator.Send(new DeleteActivityCommand {Id = id});

            return Ok();
        }

        [HttpPost("attend/{id}")]
        public async Task<ActionResult<Unit>> Attend(Guid id)
        {
            return await Mediator.Send(new AttendActivityCommand {Id = id});
        }

        [HttpDelete("deleteAttendance/{id}")]
        public async Task<IActionResult> Unattend(Guid id)
        {
            var request = await Mediator.Send(new UnattendActivityCommand {Id = id});

            return Ok();
        }
    }
}