using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Events
{
    [ApiController]
    [Route("events")]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
            => _mediator = mediator;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Index.Model>>> Index(CancellationToken cancellationToken)
            => await _mediator.Send(new Index.Query(), cancellationToken);

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create(Create.Model model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Create.Command(model), cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Get.Model>> Get(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Get.Query(id), cancellationToken);
            if (result is null) return NotFound();
            return result;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Edit(int id, Edit.Model model, CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new Edit.Command(id, model), cancellationToken);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(new Delete.Command(id), cancellationToken);
            return success ? NoContent() : BadRequest();
        }
    }
}
