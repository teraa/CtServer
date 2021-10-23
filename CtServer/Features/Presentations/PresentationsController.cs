using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Presentations
{
    [ApiController]
    [Route("events/{eventId:int}/sections/{sectionId:int}/presentations")]
    public class PresentationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PresentationsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Index.Model>>> Index(int eventId, int sectionId, CancellationToken cancellationToken)
            => await _mediator.Send(new Index.Query(eventId, sectionId), cancellationToken);

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Create.Result>> Create(int eventId, int sectionId, Create.Model model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Create.Command(eventId, sectionId, model), cancellationToken);
            return CreatedAtAction(nameof(Get), new { eventId = eventId, sectionId = sectionId, id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Get.Model>> Get(int eventId, int sectionId, int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Get.Query(eventId, sectionId, id), cancellationToken);
            if (result is null) return NotFound();
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Edit(int eventId, int sectionId, int id, Edit.Model model, CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new Edit.Command(eventId, sectionId, id, model), cancellationToken);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(int eventId, int sectionId, int id, CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new Delete.Command(eventId, sectionId, id), cancellationToken);
            return success ? NoContent() : BadRequest();
        }
    }
}
