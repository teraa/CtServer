using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Presentations;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
[Authorize]
public class PresentationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PresentationsController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Get All Presentations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadModel>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);

    /// <summary>
    /// Create Presentation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Create.Response>> Create(WriteModel model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Create.Command(model), cancellationToken);
        return CreatedAtAction(actionName: nameof(Get), routeValues: new { id = response.Id }, value: response);
    }

    /// <summary>
    /// Get Presentation
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadModel>> Get(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Get.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Edit Presentation
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
    /// Delete Presentation
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
    /// Upload Presentation Attachment (File Upload)
    /// </summary>
    [HttpPost($"{{id}}/{nameof(Attachments)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UploadAttachment(int id, IFormFile attachment, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Attachments.Upload.Command(id, attachment), cancellationToken);
        return result.Match<ActionResult>(
            (Success _) => NoContent(),
            (Fail x) => BadRequest(x)
        );
    }

    /// <summary>
    /// Get Presentation Attachment (File Download)
    /// </summary>
    [HttpGet($"{{id}}/{nameof(Attachments)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAttachment(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Attachments.Get.Query(id), cancellationToken);
        return result.Match<ActionResult>(
            (Attachments.Get.Success x) => PhysicalFile(x.FilePath, "application/octet-stream", x.FileName),
            (NotFound _) => NotFound()
        );
    }

    /// <summary>
    /// Delete Presentation Attachment
    /// </summary>
    [HttpDelete($"{{id}}/{nameof(Attachments)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAttachment(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Attachments.Delete.Command(id), cancellationToken);
        return result.Match<ActionResult>(
            (Success _) => NoContent(),
            (NotFound _) => NotFound()
        );
    }
}
