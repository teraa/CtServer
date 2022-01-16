using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Events;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Get All Events
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadModel>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);

    /// <summary>
    /// Create Event
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Create.Response>> Create(WriteModel model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Create.Command(model), cancellationToken);
        return CreatedAtAction(actionName: nameof(Get), routeValues: new { id = response.Id }, value: response);
    }

    /// <summary>
    /// Get Event
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadModel>> Get(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Get.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Edit Event
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Edit(int id, WriteModel model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Edit.Command(id, model), cancellationToken);
        return result.Match<ActionResult>(
            (Success _) => NoContent(),
            (NotFound _) => NotFound()
        );
    }

    /// <summary>
    /// Delete Event
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Delete.Command(id), cancellationToken);
        return result.Match<ActionResult>(
            (Success _) => NoContent(),
            (NotFound _) => NotFound()
        );
    }

    /// <summary>
    /// Get Event Sections
    /// </summary>
    /// <param name="id">Event ID</param>
    [HttpGet($"{{id}}/{nameof(Sections)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Features.Sections.ReadModel>>> IndexSections(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Sections.Index.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Get Event Locations
    /// </summary>
    /// <param name="id">Event ID</param>
    [HttpGet($"{{id}}/{nameof(Locations)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Features.Locations.ReadModel>>> IndexLocations(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Locations.Index.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Get Event Users
    /// </summary>
    /// <param name="id">Event ID</param>
    [HttpGet($"{{id}}/{nameof(Users)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Features.Users.ReadModel>>> IndexUsers(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Users.Index.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }
}
