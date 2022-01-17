using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Sections;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
[Authorize]
public class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Get All Sections
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadModel>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);

    /// <summary>
    /// Create Section
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Create.Success>> Create(WriteModel model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Create.Command(model), cancellationToken);
        return result.Match<ActionResult>(
            (Create.Success x) => CreatedAtAction(actionName: nameof(Get), routeValues: new { id = x.Id }, value: x),
            (Fail x) => BadRequest(x)
        );
    }

    /// <summary>
    /// Get Section
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadModel>> Get(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Get.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Edit Section
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
    /// Delete Section
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
    /// Get Section Presentations
    /// </summary>
    /// <param name="id">Section ID</param>
    [HttpGet($"{{id}}/{nameof(Presentations)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Features.Presentations.ReadModel>>> IndexPresentations(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Presentations.Index.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }
}
