using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.EntityServices.Activities.Commands;
using Reactivities.Application.EntityServices.Activities.Queries;

namespace Reactivities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var request = await _mediator.Send(new GetActivitiesQuery());

            if (request != null) return Ok(request);

            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var request = await _mediator.Send(new GetActivityDetailsQuery {Id = id});

            if (request != null) return Ok(request);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateActivityCommand command)
        {
            var request = await _mediator.Send(command);

            return Ok();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateActivityCommand command)
        {
            command.Id = id;
            var request = await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = await _mediator.Send(new DeleteActivityCommand {Id = id});

            return Ok();
        }
    }
}