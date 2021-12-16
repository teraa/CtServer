using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Events;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    public async Task<ActionResult<Create.Response>> Create(Create.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Create.Command(model), cancellationToken);
        return CreatedAtAction(actionName: nameof(Get), routeValues: new { id = result.Id }, value: result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Get.Model>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Get.Query(id), cancellationToken);
        return result is null ? NotFound() : result;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Edit(int id, Edit.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Edit.Command(id, model), cancellationToken);
        return result is null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Delete.Command(id), cancellationToken);
        return result is null ? NotFound() : NoContent();
    }

    [HttpGet($"{{id}}/{nameof(Sections)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetSections.Model>>> GetSections(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSections.Query(id), cancellationToken);
        return result is null ? NotFound() : result;
    }

    [HttpGet($"{{id}}/{nameof(Locations)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetLocations.Model>>> GetLocations(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLocations.Query(id), cancellationToken);
        return result is null ? NotFound() : result;
    }
}
