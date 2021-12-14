using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
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
            bool exists = await ctx.Users.AnyAsync(x => x.Username == request.Model.Username, cancellationToken)
                .ConfigureAwait(false);

            if (exists)
                return new Fail(new[] { "User already exists." });

            var user = new User
            {
                Username = request.Model.Username,
                Password = request.Model.Password,
            };

            ctx.Add(user);
            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var token = await _mediator.Send(new CreateToken.Command(new CreateToken.Model(user.Username)), cancellationToken)
                .ConfigureAwait(false);

            return new Success(token);
        }
    }
}
