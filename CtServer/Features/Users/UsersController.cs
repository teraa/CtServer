using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;

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
    public async Task<ActionResult<IEnumerable<ReadModel>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);

    /// <summary>
    /// Create User (Register)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Create.Success), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Fail), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IOneOf> Create([FromBody] Create.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Create.Command(model), cancellationToken);

    /// <summary>
    /// Get User
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReadModel), StatusCodes.Status200OK)]
    public async Task<IOneOf> Get(int id, CancellationToken cancellationToken)
        => await _mediator.Send(new Get.Query(id), cancellationToken);

    /// <summary>
    /// Edit User
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IOneOf> Edit(int id, Edit.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Edit.Command(id, model), cancellationToken);

    /// <summary>
    /// Delete User
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IOneOf> Delete(int id, CancellationToken cancellationToken)
        => await _mediator.Send(new Delete.Command(id), cancellationToken);

    /// <summary>
    /// Create User Session (Login)
    /// </summary>
    [HttpPost(nameof(Sessions))]
    [ProducesResponseType(typeof(Sessions.Create.Success), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Fail), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IOneOf> CreateSession([FromBody] Sessions.Create.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Sessions.Create.Command(model), cancellationToken);

    /// <summary>
    /// Get User Events (Follows)
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpGet($"{{id}}/{nameof(Events)}")]
    [ProducesResponseType(typeof(Features.Events.ReadModel[]), StatusCodes.Status200OK)]
    public async Task<IOneOf> IndexEvents(int id, CancellationToken cancellationToken)
        => await _mediator.Send(new Events.Index.Query(id), cancellationToken);

    /// <summary>
    /// Add User Event (Follow)
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpPost($"{{id}}/{nameof(Events)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IOneOf> AddEvent(int id, Events.Add.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Events.Add.Command(id, model), cancellationToken);

    /// <summary>
    /// Remove User Event (Unfollow)
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpDelete($"{{id}}/{nameof(Events)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IOneOf> RemoveEvent(int id, Events.Remove.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Events.Remove.Command(id, model), cancellationToken);

    /// <summary>
    /// Get User (Push) Subscriptions
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpGet($"{{id}}/{nameof(Subscriptions)}")]
    [ProducesResponseType(typeof(Subscriptions.Index.Model[]), StatusCodes.Status200OK)]
    public async Task<IOneOf> IndexSubscriptions(int id, CancellationToken cancellationToken)
        => await _mediator.Send(new Subscriptions.Index.Query(id), cancellationToken);

    /// <summary>
    /// Create User (Push) Subscription
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpPost($"{{id}}/{nameof(Subscriptions)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IOneOf> CreateSubscription(int id, Subscriptions.Create.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Subscriptions.Create.Command(id, model), cancellationToken);

    /// <summary>
    /// Delete User (Push) Subscription
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpDelete($"{{id}}/{nameof(Subscriptions)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IOneOf> DeleteSubscription(int id, Subscriptions.Delete.Model model, CancellationToken cancellationToken)
        => await _mediator.Send(new Subscriptions.Delete.Command(id, model), cancellationToken);

    /// <summary>
    /// Get User Notifications
    /// </summary>
    /// <param name="id">User ID</param>
    [HttpGet($"{{id}}/{nameof(Notifications)}")]
    [ProducesResponseType(typeof(Notifications.Index.Model[]), StatusCodes.Status200OK)]
    public async Task<IOneOf> IndexNotifications(int id, CancellationToken cancellationToken)
        => await _mediator.Send(new Notifications.Index.Query(id), cancellationToken);
}
