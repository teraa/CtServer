using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Services;
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
        private readonly PasswordService _passwordService;

        public Handler(
            IServiceScopeFactory scopeFactory,
            IMediator mediator,
            PasswordService passwordService)
        {
            _scopeFactory = scopeFactory;
            _mediator = mediator;
            _passwordService = passwordService;
        }

        public async Task<OneOf<Success, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            string username = request.Model.Username.ToLowerInvariant();

            var user = await ctx.Users
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken)
                .ConfigureAwait(false);

            if (user is null || !_passwordService.Test(request.Model.Password, user.PasswordHash, user.PasswordSalt))
                return new Fail(new[] { "Invalid username and/or password." });

            var token = await _mediator.Send(new CreateToken.Command(new CreateToken.Model(user.Username)), cancellationToken)
                .ConfigureAwait(false);

            return new Success(token);
        }
    }
}
