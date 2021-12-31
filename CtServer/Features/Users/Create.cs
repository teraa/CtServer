using CtServer.Data.Models;
using CtServer.Results;
using CtServer.Services;
using OneOf;

namespace CtServer.Features.Users;

public static class Create
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

    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.Username).MinimumLength(3);
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }

    public record Success
    (
        int Id,
        string Token
    );

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

            bool exists = await _ctx.Users
                .AnyAsync(x => x.Username == username, cancellationToken)
                .ConfigureAwait(false);

            if (exists)
                return new Fail("User already exists.");

            var password = _passwordService.Hash(request.Model.Password);

            var user = new User
            {
                Username = username,
                PasswordHash = password.hash,
                PasswordSalt = password.salt,
            };

            _ctx.Add(user);
            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var token = _tokenService.CreateToken(user.Id);

            return new Success(user.Id, token);
        }
    }
}
