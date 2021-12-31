using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Users;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Get All Users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Index.Model>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);

    /// <summary>
    /// Create User (Register)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Create.Fail), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<Create.Success>> Create([FromBody] Create.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Create.Command(model), cancellationToken);
        return result.Match<ActionResult>(
            success => CreatedAtAction(actionName: nameof(Get), routeValues: new { id = success.Id }, value: success),
            error => BadRequest(error)
        );
    }

    /// <summary>
    /// Get User
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Get.Model>> Get(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Get.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Edit User
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Edit(int id, Edit.Model model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Edit.Command(id, model), cancellationToken);
        return response is null ? NotFound() : NoContent();
    }

    /// <summary>
    /// Delete User
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Delete.Command(id), cancellationToken);
        return response is null ? NotFound() : NoContent();
    }

    /// <summary>
    /// Create User Session (Login)
    /// </summary>
    [HttpPost(nameof(Sessions))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Sessions.Create.Fail), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<Sessions.Create.Success>> CreateSession([FromBody] Sessions.Create.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Sessions.Create.Command(model), cancellationToken);
        return result.Match<ActionResult>(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    /// <summary>
    /// Get User Events
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpGet($"{{id}}/{nameof(Events)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Events.Index.Model>>> GetEvents(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Events.Index.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Add User Event
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpPost($"{{id}}/{nameof(Events)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> AddEvent(int id, Events.Add.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Events.Add.Command(id, model), cancellationToken);
        return result.Match<ActionResult>(
            success => NoContent(),
            notFound => NotFound(),
            error => BadRequest(error)
        );
    }
    /// <summary>
    /// Remove User Event
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpDelete($"{{id}}/{nameof(Events)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveEvent(int id, Events.Remove.Model model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Events.Remove.Command(id, model), cancellationToken);
        return response is null ? NotFound() : NoContent();
    }

    /// <summary>
    /// Get User Subscriptions
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpGet($"{{id}}/{nameof(Subscriptions)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Subscriptions.Index.Model>>> GetSubscription(int id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new Subscriptions.Index.Query(id), cancellationToken);
        return response is null ? NotFound() : response;
    }

    /// <summary>
    /// Create User Subscription
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpPost($"{{id}}/{nameof(Subscriptions)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> CreateSubscription(int id, Subscriptions.Create.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Subscriptions.Create.Command(id, model), cancellationToken);
        return result.Match<ActionResult>(
            success => Ok(success),
            notFound => NotFound()
        );
    }

    /// <summary>
    /// Delete User Subscription
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpDelete($"{{id}}/{nameof(Subscriptions)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteSubscription(int id, Subscriptions.Delete.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Subscriptions.Delete.Command(id, model), cancellationToken);
        return result.Match<ActionResult>(
            success => NoContent(),
            notFound => NotFound()
        );
    }
}
