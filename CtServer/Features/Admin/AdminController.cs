using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Admin;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Export Data
    /// </summary>
    [HttpGet("Data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Model>> Export(CancellationToken cancellationToken)
        => await _mediator.Send(new Export.Query(), cancellationToken);

    /// <summary>
    /// Import Data
    /// </summary>
    /// <remarks>
    /// This will overwrite current data!
    /// </remarks>
    [HttpPost("Data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Import(Import.Command command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
