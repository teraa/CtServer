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
    public async Task<ActionResult<IEnumerable<Index.Model>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);

    /// <summary>
    /// Create Section
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Create.Response>> Create(Create.Model model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Create.Command(model), cancellationToken);
        return CreatedAtAction(actionName: nameof(Get), routeValues: new { id = response.Id }, value: response);
    }

    /// <summary>
    /// Get Section
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Get.Model>> Get(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Get.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Edit Section
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Edit(int id, Edit.Model model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Edit.Command(id, model), cancellationToken);
        return response is null ? NotFound() : NoContent();
    }

    /// <summary>
    /// Delete Section
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Delete.Command(id), cancellationToken);
        return response is null ? NotFound() : NoContent();
    }

    /// <summary>
    /// Get Section Presentations
    /// </summary>
    [HttpGet($"{{id}}/{nameof(Presentations)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetPresentations.Model>>> GetPresentations(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetPresentations.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }
}
