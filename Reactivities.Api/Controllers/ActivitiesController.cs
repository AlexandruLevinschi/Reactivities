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
        public async Task<IActionResult> List(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
        {
            var request = await Mediator.Send(new GetActivitiesQuery
            {
                Limit = limit, 
                Offset = offset, 
                IsGoing = isGoing, 
                IsHost = isHost,
                StartDate = startDate ?? DateTime.Now
            });

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
        public async Task<ActionResult<Unit>> Create(CreateActivityCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Update(Guid id, UpdateActivityCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(Guid id)
        {
            return await Mediator.Send(new DeleteActivityCommand {Id = id});
        }

        [HttpPost("attend/{id}")]
        public async Task<ActionResult<Unit>> Attend(Guid id)
        {
            return await Mediator.Send(new AttendActivityCommand {Id = id});
        }

        [HttpDelete("deleteAttendance/{id}")]
        public async Task<ActionResult<Unit>> Unattend(Guid id)
        {
            return await Mediator.Send(new UnattendActivityCommand {Id = id});
        }
    }
}