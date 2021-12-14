using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace CtServer.Features.Auth;

public static class Login
{
    public record Command
    (
        Model Model
    ) : IRequest<OneOf<Success, Fail>>;

    public record Model
    (
        string Username,
        string Password
    );

    public record Success(string Token);
    public record Fail(IEnumerable<string> Errors);

    public class Handler : IRequestHandler<Command, OneOf<Success, Fail>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMediator _mediator;

        public Handler(IServiceScopeFactory scopeFactory, IMediator mediator)
        {
            _scopeFactory = scopeFactory;
            _mediator = mediator;
        }

        public async Task<OneOf<Success, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            // TODO: normalize username
            var user = await ctx.Users.FirstOrDefaultAsync(x =>
                x.Username == request.Model.Username &&
                x.Password == request.Model.Password, cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
                return new Fail(new[] { "Invalid username/password." });

            var token = await _mediator.Send(new CreateToken.Command(new CreateToken.Model(user.Username)), cancellationToken)
                .ConfigureAwait(false);

            return new Success(token);
        }
    }
}
