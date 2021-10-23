using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Sections
{
    [ApiController]
    [Route("events/{eventId:int}/sections")]
    public class SectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Index.Model>>> Index(int eventId, CancellationToken cancellationToken)
            => await _mediator.Send(new Index.Query(eventId), cancellationToken);

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Create.Result>> Create(int eventId, Create.Model model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Create.Command(eventId, model), cancellationToken);
            return CreatedAtAction(nameof(Get), new { eventId = eventId, id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Get.Model>> Get(int eventId, int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Get.Query(eventId, id), cancellationToken);
            if (result is null) return NotFound();
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Edit(int eventId, int id, Edit.Model model, CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new Edit.Command(eventId, id, model), cancellationToken);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(int eventId, int id, CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new Delete.Command(eventId, id), cancellationToken);
            return success ? NoContent() : BadRequest();
        }
    }
}
