using CtServer.Services;
using OneOf;

namespace CtServer.Features.Users;

public static class CreateSession
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
        private readonly CtDbContext _ctx;
        private readonly IMediator _mediator;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public Handler(
            CtDbContext ctx,
            IMediator mediator,
            PasswordService passwordService,
            TokenService tokenService)
        {
            _ctx = ctx;
            _mediator = mediator;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<OneOf<Success, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            string username = request.Model.Username.ToLowerInvariant();

            var user = await _ctx.Users
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken)
                .ConfigureAwait(false);

            if (user is null || !_passwordService.Test(request.Model.Password, user.PasswordHash, user.PasswordSalt))
                return new Fail(new[] { "Invalid username and/or password." });

            var token = _tokenService.CreateToken(user.Id);

            return new Success(token);
        }
    }
}
