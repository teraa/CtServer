using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using CtServer.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace CtServer.Features.Auth;

public static class Register
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

    public record Success(string Token);
    public record Fail(IEnumerable<string> Errors);

    public class Handler : IRequestHandler<Command, OneOf<Success, Fail>>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMediator _mediator;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public Handler(
            IServiceScopeFactory scopeFactory,
            IMediator mediator,
            PasswordService passwordService,
            TokenService tokenService)
        {
            _scopeFactory = scopeFactory;
            _mediator = mediator;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<OneOf<Success, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            string username = request.Model.Username.ToLowerInvariant();

            bool exists = await ctx.Users
                .AnyAsync(x => x.Username == username, cancellationToken)
                .ConfigureAwait(false);

            if (exists)
                return new Fail(new[] { "User already exists." });

            var password = _passwordService.Hash(request.Model.Password);

            var user = new User
            {
                Username = username,
                PasswordHash = password.hash,
                PasswordSalt = password.salt,
            };

            ctx.Add(user);
            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var token = _tokenService.CreateToken(user.Id);

            return new Success(token);
        }
    }
}
