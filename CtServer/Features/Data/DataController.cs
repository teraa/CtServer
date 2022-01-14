using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Data;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
[Authorize]
public class DataController : ControllerBase
{
    private readonly IMediator _mediator;

    public DataController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Export Data
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Export(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Export.Query(), cancellationToken);
        return File(result.Stream, result.ContentType, "export.json");
    }

    /// <summary>
    /// Import Data
    /// </summary>
    /// <remarks>
    /// This will overwrite current data!
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Import(IFormFile file, CancellationToken cancellationToken)
    {
        await _mediator.Send(new Import.Command(file), cancellationToken);
        return NoContent();
    }
}
