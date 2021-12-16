using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost(nameof(Register))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Register.Fail), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<Register.Success>> Register([FromBody] Register.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Register.Command(model), cancellationToken);
        return result.Match<ActionResult>(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpPost(nameof(Login))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Login.Fail), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<Login.Success>> Login([FromBody] Login.Model model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Login.Command(model), cancellationToken);
        return result.Match<ActionResult>(
            success => Ok(success),
            error => BadRequest(error)
        );
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Index.Model>>> Index(CancellationToken cancellationToken)
        => await _mediator.Send(new Index.Query(), cancellationToken);
}
